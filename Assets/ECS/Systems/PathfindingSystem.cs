#define UPDATE_INTERVALL

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using UnityEditor.Experimental.GraphView;
using Unity.Collections;
using UnityEngine.Networking;
using UnityEditor;
using Unity.Burst;
using System.Linq;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;



//Just for testing purposes

public class PathRequestSystem : ComponentSystem {
    private const float UPDATE_TIME = 0.5f;
    private float timeUntilNextUpdate = 0.0f;
    protected override void OnUpdate()
    {
#if UPDATE_INTERVALL
        if (timeUntilNextUpdate <= 0)
        {

            timeUntilNextUpdate = UPDATE_TIME;
#endif
        Entities.ForEach((Entity e, ref AIControlled aIControlled) =>
            {
                EntityManager.AddComponent<GridPathRequest>(e);
                EntityManager.SetComponentData(e, new GridPathRequest
                {
                    start = new int2(UnityEngine.Random.Range(0, 9), UnityEngine.Random.Range(0, 9)),
                    end = new int2(UnityEngine.Random.Range(0, 9), UnityEngine.Random.Range(0, 9))
                });
            });

            //    EntityManager.AddComponent<PathRequest>(e);
            //    EntityManager.SetComponentData(e, new PathRequest { 
            //        start = UnityEngine.Random.Range(0, 99), 
            //        end = UnityEngine.Random.Range(0, 99) });
            //});
#if UPDATE_INTERVALL
        }
        else
        {
            timeUntilNextUpdate -= Time.DeltaTime;
        }
#endif
    }
}

public class GridPathfindingSystem : ComponentSystem
{
    private const int COST_MOVE_STRAIGHT = 10;
    private const int COST_MOVE_DIAGONAL = 14;
    
    protected override void OnUpdate()
    {
        //List<PathfindingJobGrid> jobs = new List<PathfindingJobGrid>();
        NativeList<JobHandle> handles = new NativeList<JobHandle>(Allocator.Temp);
        Grid<GridPathNode> grid = GameWorldManager.Instance().grid;
        NativeArray<GridPathNode> nodes = GetArrayFromGrid(grid);

        int2 gridSize = new int2(grid.Width, grid.Height);


        //double startA = UnityEngine.Time.realtimeSinceStartup;


        Entities.ForEach((Entity e, ref GridPathRequest pathRequest) => {
            NativeArray<GridPathNode> tmpNodes = new NativeArray<GridPathNode>(nodes, Allocator.TempJob);
            handles.Add(new PathfindingJobGrid
            {
                gridSize = gridSize,
                nodes = tmpNodes,
                start = pathRequest.start,
                end = pathRequest.end
            }.Schedule());
            //jobs.Add(job);
            //job.Schedule());
            PostUpdateCommands.RemoveComponent<PathRequest>(e);
        });
        //double endA = UnityEngine.Time.realtimeSinceStartup;
        //Debug.Log("A: " + (endA - startA));

        JobHandle.CompleteAll(handles);
        //jobs.Clear();
        nodes.Dispose();
        handles.Dispose();

    }

    private NativeArray<GridPathNode> GetArrayFromGrid(Grid<GridPathNode> grid) {
        NativeArray<GridPathNode> nodes = new NativeArray<GridPathNode>(grid.Width * grid.Height, Allocator.TempJob);
        for (int x = 0; x < grid.Width; x++) {
            for (int y = 0; y < grid.Height; y++) {
                nodes[x + y * grid.Width] = grid.GetValue(x, y);
            } 
        }

        return nodes;
    }

    [BurstCompile]
    public struct PathfindingJobGrid : IJob
    {
        public int2 gridSize;

        [DeallocateOnJobCompletion]
        public NativeArray<GridPathNode> nodes;

        public int2 start;
        public int2 end;

        private int CalcIndex(int2 position)  {
            return position.x + position.y * gridSize.x;
        }

        private int CalcDistanceCost(int2 posA, int2 posB) {
            int xDistance = math.abs(posA.x - posB.x);
            int yDistance = math.abs(posA.y - posB.y);
            int remaining = math.abs(xDistance - yDistance);
            return COST_MOVE_DIAGONAL * math.min(xDistance, yDistance) + COST_MOVE_STRAIGHT * remaining;
        }

        private int GetLowest(NativeList<int> open)
        {
            GridPathNode lowest = nodes[open[0]];
            for (int i = 0; i < open.Length; i++)
            {
                if (nodes[open[i]].sum < lowest.sum)
                {
                    lowest = nodes[open[i]];
                }
            }
            return lowest.index;
        }

        private bool ValidPosition(int2 position)
        {
            return
                position.x >= 0 &&
                position.y >= 0 &&
                position.x < gridSize.x &&
                position.y < gridSize.y;
        }

        public void Execute()
        {
            NativeArray<int2> neighbors = new NativeArray<int2>(8, Allocator.Temp);
            neighbors[0] = new int2(-1, 0);
            neighbors[1] = new int2(+1, 0);
            neighbors[2] = new int2(0, +1);
            neighbors[3] = new int2(0, -1);
            neighbors[4] = new int2(-1, -1);
            neighbors[5] = new int2(-1, +1);
            neighbors[6] = new int2(+1, +1);
            neighbors[7] = new int2(+1, -1);

            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i].Init(CalcDistanceCost(nodes[i].position, end), gridSize.x);
            }

            int endNodeIndex = CalcIndex(end);
            int startNodeIndex = CalcIndex(start);
            nodes[startNodeIndex].SetCost(0);

            NativeList<int> open = new NativeList<int>(Allocator.Temp)  {
            startNodeIndex
        };

            int it = 0;
            while (open.Length > 0 && it < 50)
            {
                it++;
                int current = GetLowest(open);
                GridPathNode currentN = nodes[current];
                if (current == endNodeIndex)
                {
                    //Debug.Log("Found Path");
                    break;
                }
                for (int i = 0; i < open.Length; i++)
                {
                    if (open[i] == current)
                    {
                        open.RemoveAtSwapBack(i);
                        break;
                    }
                }
                nodes[current].Close();

                for (int i = 0; i < neighbors.Length; i++)
                {
                    int2 neighbor = neighbors[i];
                    int2 neighborposition = new int2(currentN.position.x + neighbor.x, currentN.position.y + neighbor.y);

                    if (!ValidPosition(neighborposition))
                    {
                        continue;
                    }
                    int neighborIndex = CalcIndex(neighborposition);
                    if (nodes[neighborIndex].closed || !nodes[neighborIndex].walkable)
                    {
                        continue;
                    }

                    int newCost = currentN.cost + CalcDistanceCost(currentN.position, neighborposition);
                    if (newCost < nodes[neighborIndex].cost)
                    {
                        nodes[neighborIndex].Update(current, newCost);
                    }

                    if (!open.Contains(neighborIndex))
                    {
                        open.Add(neighborIndex);
                    }
                }
            }
            //Debug.Log("Didn't find a path");
            neighbors.Dispose();
            open.Dispose();
        }
    }
}

//Pathfindingsystem with Nodes and Connections as its base
//public class PathfindingSystem : ComponentSystem
//{
//    protected override void OnUpdate()
//    {
//        List<PathfindingJob> pathfindingJobs = new List<PathfindingJob>();
//        NativeList<JobHandle> pathfindingJobHandles = new NativeList<JobHandle>(Allocator.Temp);
//        NativeArray<PathNode> pathNodeArray = new NativeArray<PathNode>(GameWorldManager.Instance().nodes, Allocator.TempJob);
//        NativeArray<Connection> connections = new NativeArray<Connection>(GameWorldManager.Instance().connections, Allocator.TempJob);

//        Entities.ForEach((Entity e, ref PathRequest pathRequest) =>
//        {
//            PathfindingJob job = new PathfindingJob
//            {
//                heu = GameWorldManager.Instance().heu,
//                start = pathRequest.start,
//                end = pathRequest.end,
//                nodes = new NativeArray<PathNode>(pathNodeArray, Allocator.TempJob),
//                connections = new NativeArray<Connection>(connections, Allocator.TempJob),
//            };
//            pathfindingJobs.Add(job);
//            pathfindingJobHandles.Add(job.Schedule());
//            PostUpdateCommands.RemoveComponent<PathRequest>(e);
//        });
//        JobHandle.CompleteAll(pathfindingJobHandles);
//        connections.Dispose();
//        pathNodeArray.Dispose();
//        pathfindingJobHandles.Dispose();
//    }
//}

//[BurstCompile]
//public struct PathfindingJob : IJob
//{
//    public Heuristic heu;
//    [DeallocateOnJobCompletion]
//    public NativeArray<PathNode> nodes;
//    [DeallocateOnJobCompletion]
//    public NativeArray<Connection> connections;
//    public int start;
//    public int end;

//    public void Execute()
//    {
//        NativeList<int> connectionIndizes = new NativeList<int>(Allocator.Temp);

//        heu.endpos = nodes[end].worldPosition;
//        //init all nodes
//        for (int i = 0; i < nodes.Length; i++)
//        {
//            PathNode n = nodes[i];
//            n.heu = heu.Get(n.worldPosition);
//            n.cost = 99999;
//            n.sum = 99999;
//            n.closed = false;
//            n.index = i;
//            n.previous = -1;
//            nodes[i] = n;
//        }

//        PathNode startNode = nodes[start];
//        startNode.cost = 0;
//        startNode.sum = startNode.heu;
//        nodes[start] = startNode;

//        PriorityHeap open = new PriorityHeap
//        {
//            contents = new NativeList<PathNode>(Allocator.Temp)
//        };
//        open.Add(nodes[start]);

//        while (open.Size() > 0)
//        {
//            //open.Print();

//            PathNode current = open.GetLowest();
//            if (current.index == end)
//            {
//                //Debug.Log("Found a path. Start was " + start + " | End was " + end);
//                break;
//            }
//            if (current.closed)
//            {
//                open.Remove();
//            }
//            else
//            {
//                connectionIndizes.Clear();
//                for (int i = 0; i < connections.Length; i++)
//                {
//                    if (connections[i].to == current.index || connections[i].from == current.index)
//                    {
//                        connectionIndizes.Add(i);
//                    }
//                }
//                for (int i = 0; i < connectionIndizes.Length; i++)
//                {
//                    int nodeIndex;
//                    if (connections[i].to == current.index)
//                    {
//                        nodeIndex = connections[i].from;
//                    }
//                    else
//                    {
//                        nodeIndex = connections[i].to;
//                    }
//                    if (nodes[nodeIndex].closed)
//                    {
//                        continue;
//                    }

//                    //update NeighborDistance and previous Node, if current Distance + movementCost < NeighborDistance
//                    float newcost = current.cost + connections[i].cost;
//                    if (newcost < nodes[nodeIndex].cost)
//                    {
//                        PathNode node = nodes[nodeIndex];
//                        node.cost = newcost;
//                        node.sum = newcost + nodes[nodeIndex].heu;
//                        node.previous = current.index;
//                        nodes[nodeIndex] = node;
//                        open.Replace(nodes[nodeIndex]);
//                    }

//                    //add the Neighbor to the open List if they are not already part of it or of the closed List
//                    if (!open.Contains(nodeIndex))
//                    {
//                        open.Add(nodes[nodeIndex]);
//                    }

//                }
//                current.closed = true;
//                open.Replace(current);
//                nodes[current.index] = current;
//            }
//            //Debug.Log("No Path");
//        }
//        //Debug.Log("Didn't find path");
//        connectionIndizes.Dispose();
//        open.contents.Dispose();
//    }
//}

public struct GridPathNode
{
    public int2 position;
    public int index;
    public int cost;
    public int heu;
    public int sum;

    public bool closed;
    public bool walkable;
    public int previous;

    public void Close()
    {
        closed = true;
    }

    public void SetHeu(int cost)
    {
        heu = cost;
    }
    public void SetCost(int cost)
    {
        this.cost = cost;
        sum = cost + heu;
    }
    public void CalcIndex(int gridwidth)
    {
        index =  position.x + position.y * gridwidth;
    }

    public void Update(int previous, int cost)
    {
        this.previous = previous;
        SetCost(cost);
    }

    public void Init(int heu, int gridwidth)
    {
        CalcIndex(gridwidth);
        SetHeu(heu);
    }
}