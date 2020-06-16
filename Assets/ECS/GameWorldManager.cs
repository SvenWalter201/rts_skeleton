using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using Unity.Mathematics;
using System.IO;
using Unity.Entities;
using Unity.Transforms;

public class GameWorldManager
{
    public PathNode[] nodes;
    public Connection[] connections;
    public Heuristic heu;
    public Grid<GridPathNode> grid;

    private static GameWorldManager gwm;

    private GameWorldManager() {
        InitializeGrid();
        //InitializeTestData();
    }

    private void InitializeGrid()
    {
        grid = new Grid<GridPathNode>(10, 10, 1.0f, new Vector2(-10, -10), (int x, int y) => new GridPathNode
        {
            position = new int2(x, y),
            previous = -1,
            walkable = true,
            closed = false,
            cost = 99999,
            sum = 99999
    });
    }

    private void InitializeTestData()
    {
        int width = 10;
        int height = 10;
        float distance = 1.0f;

        connections = new Connection[width * (height-1) + height * (width-1)];
        nodes = new PathNode[width * height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                nodes[GetIndex(width,x,y)] = new PathNode { worldPosition = new float3(x*distance, y*distance, 0) };
            }
        }
        int conIndex = 0;
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (x < width - 1) {
                    connections[conIndex] = new Connection { cost = distance, to = GetIndex(width, x, y), from = GetIndex(width, x + 1, y) };
                    conIndex++;
                }
                if(y < height - 1){
                    connections[conIndex] = new Connection { cost = distance, to = GetIndex(width, x, y), from = GetIndex(width, x, y+1) };
                    conIndex++;
                }
            }
        }
    }

    public int GetIndex(int width, int x, int y)
    {
        return y * width + x;
    }


    public static GameWorldManager Instance()
    {
        if(gwm == null)
        {
            gwm = new GameWorldManager();
        }
        return gwm;
    }

    public void GenerateNavMesh()
    {
        //Get Bounds of all gameobject hitboxes on a certain layers, then create grid
        //TODO: create a navmesh, based on the current position of all gameobjects 
    }


    //public List<PathNode> GetNodesCopy()
    //{
    //    List<PathNode> nodesCopy = new List<PathNode>(nodes.Count);
    //    for (int i = 0; i < nodes.Count; i++)
    //    {
    //        nodesCopy[i] = nodes[i];
    //    }
    //    return nodesCopy;
    //}

    //public List<Connection> GetConnectionsCopy()
    //{
    //    List<Connection> connectionsCopy = new List<Connection>(connections.Count);
    //    for (int i = 0; i < connections.Count; i++)
    //    {
    //        connectionsCopy[i] = connections[i];
    //    }
    //    return connectionsCopy;
    //}

    //public Grid CopyNavmesh()
    //{
    //    //TODO: give an agent a copy of the current grid

    //    return null;
    //}


}
