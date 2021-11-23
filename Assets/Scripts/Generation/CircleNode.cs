using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using quiet;

public struct CircleNode
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Weight { get; set; }
    public List<DotNode> dots;
    public Graph graph;

    public bool CompletedCircle { get; private set; }

    public CircleNode(int x, int y, int weight, Graph graph)
    {
        (X, Y, Weight, this.graph, dots, CompletedCircle) = (x, y, weight, graph, new List<DotNode>(), false);
        CompletedCircle = PopulateDots();
    }

    public bool PopulateDots()
    {
        (int x, int y) top = (X, Y + Weight);
        (int x, int y) bottom = (X, Y - Weight);
        (int x, int y) right = (X + Weight, Y);
        (int x, int y) left = (X - Weight, Y);

        if (IsInvalidCoordinate(top) || IsInvalidCoordinate(bottom) || IsInvalidCoordinate(right) || IsInvalidCoordinate(left)) return false;

        dots = new List<DotNode>()
        {
            GetOrCreateDotAt(top.x, top.y),
            GetOrCreateDotAt(bottom.x, bottom.y),
            GetOrCreateDotAt(right.x, right.y),
            GetOrCreateDotAt(left.x, left.y),
        };

        foreach (DotNode dot in dots)
        {
            dot.connections.Add(this);
        }

        return true;
    }

    public DotNode GetOrCreateDotAt(int x, int y) => graph[x, y].Dot ?? new DotNode(x, y, graph);

    public void RemoveSelf()
    {
        graph[X, Y].Circle.Remove(Weight);
        foreach (DotNode dot in dots)
            dot.RemoveCircle(this);
    }

    private bool IsInvalidCoordinate((int x, int y) coord) => !quiet.Math.InRange(coord.x, 0, graph.Height - 1) || !quiet.Math.InRange(coord.y, 0, graph.Width - 1);
}
