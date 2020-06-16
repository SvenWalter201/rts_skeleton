using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugUtils
{
    public static TextMesh CreateTextMesh(string text, Vector2 worldPosition, int fontsize)
    {
        GameObject go = new GameObject("TextMesh");
        go.transform.position = worldPosition;
        go.transform.localScale = new Vector3(0.1f, 0.1f, 1);
        go.AddComponent<TextMesh>();
        TextMesh textMesh = go.GetComponent<TextMesh>();
        textMesh.text = text;
        textMesh.alignment = TextAlignment.Center;
        textMesh.fontSize = fontsize;
        textMesh.color = Color.white;
        textMesh.anchor = TextAnchor.MiddleCenter;
        return textMesh;       
    }
}
