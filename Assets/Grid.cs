using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class Grid<T>
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    private float cellsize;
    private Vector2 positionOffset;
    private T[,] values;

    public Grid(int width, int height, float cellsize, Vector2 positionOffset, Func<int, int, T> init)
    {
        Width = width;
        Height = height;
        this.cellsize = cellsize;
        this.positionOffset = positionOffset;

        values = new T[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                values[x, y] = init(x, y);
            }
        }
    }

    public void GetCoordinates(Vector2 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt(worldPosition.x * cellsize - positionOffset.x);
        y = Mathf.FloorToInt(worldPosition.y * cellsize - positionOffset.y);
    } 

    public Vector2 GetWorldPosition(int x, int y)
    {
        return new Vector2(positionOffset.x + x * cellsize, positionOffset.y + y * cellsize); 
    }

    public T GetValue(int x, int y)
    {
        return values[x, y];
    }

    public void SetValue(T value, int x, int y)
    {
        values[x, y] = value;
    }
}
