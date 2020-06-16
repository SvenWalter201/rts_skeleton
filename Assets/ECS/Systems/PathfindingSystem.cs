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

//Just for testing purposes

public class PathRequestSystem : ComponentSystem {
    protected override void OnUpdate() {

            Entities.ForEach((Entity e, ref AIControlled aIControlled) =>
            {
                EntityManager.AddComponent<PathRequest>(e);
                EntityManager.SetComponentData(e, new PathRequest { start = Random.Range(0, 9), end = Random.Range(0, 9) });
            });
        
     }
}



public class PathfindingSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        List<PathfindingJob> pathfindingJobs = new List<PathfindingJob>();
        NativeList<JobHandle> pathfindingJobHandles = new NativeList<JobHandle>(Allocator.Temp);
        NativeArray<PathNode> pathNodeArray = new NativeArray<PathNode>(GameWorldManager.Instance().nodes, Allocator.TempJob);
        NativeArray<Connection> connections = new NativeArray<Connection>(GameWorldManager.Instance().connections, Allocator.TempJob);

        Entities.ForEach((Entity e, ref PathRequest pathRequest)=> {
            PathfindingJob job = new PathfindingJob {
                heu = GameWorldManager.Instance().heu,
                start = pathRequest.start,
                end = pathRequest.end,
                nodes = new NativeArray<PathNode>(pathNodeArray, Allocator.TempJob),
                connections = new NativeArray<Connection>(connections, Allocator.TempJob),
        };

            pathfindingJobs.Add(job);
            pathfindingJobHandles.Add(job.Schedule());
            PostUpdateCommands.RemoveComponent<PathRequest>(e);
        });
        JobHandle.CompleteAll(pathfindingJobHandles);
        connections.Dispose();
        pathNodeArray.Dispose();
        pathfindingJobHandles.Dispose();
    }
}

[BurstCompile]
public struct PathfindingJob : IJob {
    public Heuristic heu;
    [DeallocateOnJobCompletion]
    public NativeArray<PathNode> nodes;
    [DeallocateOnJobCompletion]
    public NativeArray<Connection> connections;
    public int start;
    public int end;

    public void Execute() {
     NativeList<int> connectionIndizes = new NativeList<int>(Allocator.Temp);

    heu.endpos = nodes[end].worldPosition;
        //init all nodes
        for (int i = 0; i < nodes.Length; i++) {
            PathNode n = nodes[i];
            n.heu = heu.Get(n.worldPosition);
            n.cost = 99999;
            n.sum = 99999;
            n.closed = false;
            n.index = i;
            n.previous = -1;
            nodes[i] = n;
        }

        PathNode startNode = nodes[start];
        startNode.cost = 0;
        startNode.sum = startNode.heu;
        nodes[start] = startNode;

        PriorityHeap open = new PriorityHeap{
            contents = new NativeList<PathNode>(Allocator.Temp)
        };
        open.Add(nodes[start]);

        while(open.Size() > 0) {
            //open.Print();

            PathNode current = open.GetLowest();
            if(current.index == end) {
                //Debug.Log("Found a path. Start was " + start + " | End was " + end);
                break;
            }
            if (current.closed) {
                open.Remove();
            }
            else {
                connectionIndizes.Clear();
                for (int i = 0; i < connections.Length; i++)
                {
                    if (connections[i].to == current.index || connections[i].from == current.index)
                    {
                        connectionIndizes.Add(i);
                    }
                }
                for (int i = 0; i < connectionIndizes.Length; i++){
                    int nodeIndex;
                    if (connections[i].to == current.index){
                        nodeIndex = connections[i].from;
                    }
                    else {
                        nodeIndex = connections[i].to;
                    }
                    if (nodes[nodeIndex].closed)  {
                        continue;
                    }

                    //update NeighborDistance and previous Node, if current Distance + movementCost < NeighborDistance
                    float newcost = current.cost + connections[i].cost;
                    if (newcost < nodes[nodeIndex].cost) {
                        PathNode node = nodes[nodeIndex];
                        node.cost = newcost;
                        node.sum = newcost + nodes[nodeIndex].heu;
                        node.previous = current.index;
                        nodes[nodeIndex] = node;
                        open.Replace(nodes[nodeIndex]);
                    }

                    //add the Neighbor to the open List if they are not already part of it or of the closed List
                    if (!open.Contains(nodeIndex)) {
                        open.Add(nodes[nodeIndex]);
                    }

                }
                current.closed = true;
                open.Replace(current);
                nodes[current.index] = current;
            }
            //Debug.Log("No Path");
        }
        //Debug.Log("Didn't find path");
        connectionIndizes.Dispose();
        open.contents.Dispose();
    }
}
