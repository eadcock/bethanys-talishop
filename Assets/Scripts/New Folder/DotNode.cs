using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public struct DotNode 
{
    public int X { get; set; }
    public int Y { get; set; }

    public List<CircleNode> connections;
    public Graph graph;

    public void Deconstruct(out int x, out int y) => (x, y) = (X, Y);

    public DotNode(int x, int y, Graph graph)
    {
        Debug.Assert(!graph[x, y].HasDot(), "There's already a dot here!");

        (X, Y, this.graph) = (x, y, graph);
        connections = new List<CircleNode>();

        graph[x, y].Dot = this;
    }

    public DotNode(int x, int y, Graph graph, IEnumerable<CircleNode> connections)
        : this(x, y, graph)
    {
        this.connections = new List<CircleNode>(connections);
    }

    public DotNode(int x, int y, Graph graph, params CircleNode[] connections) : this(x, y, graph, connections.AsEnumerable()) { }

    public void RemoveCircle(CircleNode toRemove)
    {
        connections.Remove(toRemove);

        if (!connections.Any())
            RemoveSelf();
    }

    public void RemoveSelf()
    {
        graph[X, Y].Dot = null;
    }
}
