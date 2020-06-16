using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PercentageGridVisual : MonoBehaviour
{
    [SerializeField]
    private int width = 0;
    [SerializeField]
    private int height = 0;
    [SerializeField]
    private float cellSize = 0.0f;
    private Grid<int> grid;
    private TextMesh[,] text;
    private void Start()
    {
        grid = new Grid<int>(width, height, cellSize, new Vector2(-5, -5), (int x, int y) => 0);
        text = new TextMesh[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Debug.DrawLine(grid.GetWorldPosition(x,y), grid.GetWorldPosition(x, y+1), Color.white, 100f);
                Debug.DrawLine(grid.GetWorldPosition(x, y), grid.GetWorldPosition(x+1, y), Color.white, 100f);
                
                text[x, y] = DebugUtils.CreateTextMesh("0", grid.GetWorldPosition(x, y) + new Vector2(cellSize * 0.5f, cellSize * 0.5f), 50);
            }
            Debug.DrawLine(grid.GetWorldPosition(0, height), grid.GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(grid.GetWorldPosition(width, 0), grid.GetWorldPosition(width, height), Color.white, 100f);
        }
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 position =  Camera.main.ScreenToWorldPoint(Input.mousePosition);
            grid.GetCoordinates(position, out int x, out int y);
            int value =  int.Parse(text[x, y].text);
            value ++;
            grid.SetValue(value, x, y);
            text[x, y].text = value.ToString();
        }
    }
}
