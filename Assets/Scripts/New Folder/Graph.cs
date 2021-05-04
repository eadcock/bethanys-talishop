using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class Graph
{
    private readonly PuzzleNode[,] graph;
    private readonly Grid layout;

    public int Height => graph?.GetLength(0) ?? 0;
    public int Width => graph?.GetLength(1) ?? 0;

    public Dot dotPrefab;
    public Circle circlePrefab;

    public List<Circle> circles;

    public PuzzleNode this[int i, int j] 
    {
        get => graph[i, j];
        set => graph[i, j] = value;
    }

    public Graph(int height, int width, Grid layout, Dot prefab, Circle circlePrefab)
    {
        graph = new PuzzleNode[height, width];
        this.layout = layout;
        dotPrefab = prefab;
        this.circlePrefab = circlePrefab;
        circles = new List<Circle>();
    }

    public void PopulateGrid()
    {
        for(int i = 0; i < Height; i++)
        {
            for(int j = 0; j < Width; j++)
            {
                graph[i, j] = new PuzzleNode(i, j);
            }
        }
    }

    public void DisplayGraph()
    {
        foreach(PuzzleNode node in graph)
        {
            if(node.Dot.HasValue)
            {
                if(!node.dotObject)
                {
                    Dot d = UnityEngine.Object.Instantiate(dotPrefab);
                    d.transform.position = layout.CellToWorld(new Vector3Int(node.X - Width / 2, node.Y - Height / 2, 0));
                    d.requiredCircles = node.Dot.Value.connections.Count;
                    node.dotObject = d;
                }
            }
        }
    }

    public void DisplayNode(PuzzleNode node)
    {
        if (!node.dotObject)
        {
            Dot d = UnityEngine.Object.Instantiate(dotPrefab);
            d.transform.position = layout.CellToWorld(new Vector3Int(node.X - Width / 2, node.Y - Height / 2, 0));
            Debug.Log("Creating dot at: " + d.transform.position);
            d.requiredCircles = node.Dot.Value.connections.Count;
            node.dotObject = d;
        }
        else
        {
            node.dotObject.requiredCircles = node.Dot.Value.connections.Count;
        }
    }

    public void Clear()
    {
        for(int i = 0; i < Height; i++)
        {
            for(int j = 0; j < Width; j++)
            {
                if(graph[i, j].dotObject) 
                    UnityEngine.Object.Destroy(graph[i, j].dotObject.gameObject);
                graph[i, j] = new PuzzleNode(i, j);
            }
        }

        for(int i = 0; i < circles.Count; i++)
        {
            UnityEngine.Object.Destroy(circles[i]);
        }

        circles.Clear();
    }

    public void SolveCurrent()
    {
        foreach (PuzzleNode node in graph)
        {
            foreach(CircleNode circle in node.Circle.Values)
            {

                Circle c = UnityEngine.Object.Instantiate(circlePrefab);
                Dot dot1 = graph[circle.dots[0].X, circle.dots[0].Y].dotObject;
                Dot dot2 = graph[circle.dots[1].X, circle.dots[1].Y].dotObject;
                c.transform.position = new Vector3(dot1.X, dot1.Y, 0);
                c.ResizeCircle(new Vector2(dot1.X, dot1.Y), new Vector2(dot2.X, dot2.Y));
            }
        }
    }
}
