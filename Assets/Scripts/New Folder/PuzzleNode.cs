using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class PuzzleNode
{
    public int X { get; set; }
    public int Y { get; set; }

    public PuzzleNode(int x, int y) => (X, Y) = (x, y);

    public Dictionary<int, CircleNode> Circle { get; set; } = new Dictionary<int, CircleNode>();

    public DotNode? Dot { get; set; }

    public Dot dotObject;

    public bool HasDot() => Dot != null;

    public bool HasCircleOfWeight(int weight) => Circle.TryGetValue(weight, out _);

    public void Deconstruct(out Dictionary<int, CircleNode> circle, out DotNode? dot) => (circle, dot) = (Circle, Dot);
    public void Deconstruct(out int x, out int y) => (x, y) = (X, Y);
}