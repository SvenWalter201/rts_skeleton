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

    private static GameWorldManager gwm;

    private GameWorldManager() {
        InitializeTestData();
    }

    private void InitializeTestData()
    {
        int width = 20;
        int height = 20;
        float distance = 2f;

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
