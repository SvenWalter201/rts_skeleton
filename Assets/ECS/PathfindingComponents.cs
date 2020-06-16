using System;
using System.Diagnostics;
using Unity.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using System.IO.IsolatedStorage;
using Unity.Collections.LowLevel.Unsafe;

public struct PathRequest : IComponentData
{
	//Todo: implement position to node conversion
	//public float3 requestedPosition;

	public int start;
	public int end;
}

public struct PathFollow : IComponentData
{
    //maybe buffer?
}

public struct Heuristic
{
	public float3 endpos;

	public float Get(float3 pos) {
		return math.sqrt(math.pow(endpos.x - pos.x, 2) + math.pow(endpos.y - pos.y, 2));
	}
};


public struct PathNode
{
	public float3 worldPosition;
	public float cost;
	public float heu;
	public float sum;
	public int index;
	public int previous;
	public bool closed;

	public static bool operator ==(PathNode a, PathNode b){
        return a.index == b.index;
    }

	public static bool operator !=(PathNode a, PathNode b){
		return a.index != b.index;
	}
}


public struct Connection
{
	public float cost;
    public int to;
    public int from;
}

public struct PriorityHeap
{

    public NativeList<PathNode> contents;

    public void Print() {
		UnityEngine.Debug.Log("Printing: ");
        for (int i = 0; i < contents.Length; i++) {
            UnityEngine.Debug.Log("sum: " + contents[i].sum);
        }
    }

    public void Add(PathNode n)
	{
		contents.Add(n);
		int position = contents.Length - 1;
		while (true) {
			int other = Mathf.RoundToInt(position / 2);
			if (position == 0) { 
				break; 
			}
			else if (contents[other].sum > n.sum) {
				Swap(other, position);
				position = other;
			}
			else { 
				break; 
			}
		}
	}

	//maybe use binary search for this, since the array is somewhat sorted
	public bool Contains(int nodeIndex){
        for (int i = 0; i < contents.Length; i++) {
			if (nodeIndex == contents[i].index){
				return true;
			}
		}
		return false;
	}

	public void Replace(PathNode node){
		for (int i = 0; i < contents.Length; i++){
			if (node == contents[i]){
				contents[i] = node;
			}
		}
	}

	public void Swap(int a, int b){
		PathNode aTemp = contents[a];
		contents[a] = contents[b];
		contents[b] = aTemp;
	}

	public void Remove(){
		contents.RemoveAt(0);
	}

	public PathNode GetLowest(){
		return contents[0];
	}

	public int Size(){
		return contents.Length;
	}
}

public static class NativeListExtensions
{
    public static bool Remove<T, TI>(this NativeList<T> list, TI element)
        where T : struct, IEquatable<TI>
        where TI : struct
    {
        var index = list.IndexOf(element);
        if (index < 0)
        {
            return false;
        }

        list.RemoveAt(index);
        return true;
    }

    public static void RemoveAt<T>(this NativeList<T> list, int index)
        where T : struct
    {
        list.RemoveRange(index, 1);
    }

    public static unsafe void RemoveRange<T>(this NativeList<T> list, int index, int count)
        where T : struct
    {
//#if ENABLE_UNITY_COLLECTIONS_CHECKS
//        if ((uint)index >= (uint)list.Length)
//        {
//            throw new IndexOutOfRangeException(
//                $"Index {index} is out of range in NativeList of '{list.Length}' Length.");
//        }
//#endif

        int elemSize = UnsafeUtility.SizeOf<T>();
        byte* basePtr = (byte*)list.GetUnsafePtr();

        UnsafeUtility.MemMove(basePtr + (index * elemSize), basePtr + ((index + count) * elemSize), elemSize * (list.Length - count - index));

        // No easy way to change length so we just loop this unfortunately.
        for (var i = 0; i < count; i++)
        {
            list.RemoveAtSwapBack(list.Length - 1);
        }
    }
}