using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RealtimeChart : Graphic
{
    public int maxDataPoints = 25;
    public float minValue = 0f;
    public float maxValue = 100f;
    
    private Queue<float> dataPoints = new Queue<float>();
    
    public void AddDataPoint(float value)
    {
        dataPoints.Enqueue(value);
        if (dataPoints.Count > maxDataPoints)
            dataPoints.Dequeue();
        
        SetVerticesDirty(); // Trigger redraw
    }
    
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        
        if (dataPoints.Count < 2) return;
        
        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;
        float pointSpacing = width / (maxDataPoints - 1);
        
        int index = 0;
        float[] points = dataPoints.ToArray();
        
        for (int i = 0; i < points.Length - 1; i++)
        {
            float x1 = i * pointSpacing;
            float y1 = Mathf.InverseLerp(minValue, maxValue, points[i]) * height;
            float x2 = (i + 1) * pointSpacing;
            float y2 = Mathf.InverseLerp(minValue, maxValue, points[i + 1]) * height;
            
            // Draw line segment (create quad)
            DrawLine(vh, new Vector2(x1, y1), new Vector2(x2, y2), 0.06f);
        }
    }
    
    void DrawLine(VertexHelper vh, Vector2 start, Vector2 end, float thickness)
    {
        Vector2 dir = (end - start).normalized;
        Vector2 perpendicular = new Vector2(-dir.y, dir.x) * thickness * 0.5f;
        
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;
        
        int startIndex = vh.currentVertCount;
        
        vertex.position = start - perpendicular;
        vh.AddVert(vertex);
        vertex.position = start + perpendicular;
        vh.AddVert(vertex);
        vertex.position = end + perpendicular;
        vh.AddVert(vertex);
        vertex.position = end - perpendicular;
        vh.AddVert(vertex);
        
        vh.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
        vh.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
    }
}