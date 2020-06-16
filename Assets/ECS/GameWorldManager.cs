using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using Unity.Mathematics;

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
        PathNode a = new PathNode { worldPosition = new float3(0,0,0) };
        PathNode b = new PathNode { worldPosition = new float3(5, 5, 0) };
        PathNode c = new PathNode { worldPosition = new float3(7, 0, 0) };
        PathNode d = new PathNode { worldPosition = new float3(5, -3, 0) };
        PathNode e = new PathNode { worldPosition = new float3(1, -7, 0) };
        PathNode f = new PathNode { worldPosition = new float3(6, -6, 0) };
        PathNode g = new PathNode { worldPosition = new float3(10, -2, 0) };
        PathNode h = new PathNode { worldPosition = new float3(10, -10, 0) };
        PathNode i = new PathNode { worldPosition = new float3(6, -14, 0) };
        PathNode j = new PathNode { worldPosition = new float3(11, -15, 0) };
        PathNode k = new PathNode { worldPosition = new float3(9, -20, 0) };

        Connection ab = new Connection { cost = 7, to = 0, from = 1 };
        Connection bc = new Connection { cost = 6, to = 1, from = 2 };
        Connection cf = new Connection { cost = 6, to = 2, from = 5 };
        Connection cg = new Connection { cost = 6, to = 2, from = 6 };
        Connection ad = new Connection { cost = 6, to = 0, from = 3 };
        Connection ae = new Connection { cost = 7, to = 0, from = 4 };
        Connection ef = new Connection { cost = 6, to = 4, from = 5 };
        Connection df = new Connection { cost = 3, to = 3, from = 5 };
        Connection gh = new Connection { cost = 8, to = 6, from = 7 };
        Connection ei = new Connection { cost = 9, to = 4, from = 8 };
        Connection ik = new Connection { cost = 7, to = 8, from = 10};
        Connection jk = new Connection { cost = 6, to = 9, from = 10 };
        Connection hj = new Connection { cost = 5, to = 7, from = 9 };
        Connection ij = new Connection { cost = 5, to = 8, from = 9 };
        Connection fi = new Connection { cost = 8, to = 5, from = 8 };
        Connection ih = new Connection { cost = 6.5f, to = 8, from = 7 };

       connections = new Connection[] { ab, bc, cf, cg, ad, ae, ef, df, gh, ei, ik, jk, hj, ij, fi, ih };
       nodes = new PathNode[]{ a, b, c, d, e, f, g, h, i, j, k };
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
