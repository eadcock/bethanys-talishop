using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using quiet;
using System.Linq;

[RequireComponent(typeof(Grid))]
public class RandomPuzzleGenerator : MonoBehaviour
{
    [Flags]
    public enum GenStyle
    {
        None                = 0b_0000_0000,
        RandomPlacement     = 0b_0000_0001,
        FavorConnections    = 0b_0000_0010,
        LowSizeVariance     = 0b_0000_0100,
        RandomSizeVariance  = 0b_0000_1000,
        HighSizeVariance    = 0b_0001_0000,
        RandomNumDots       = 0b_0010_0000,
        PureRandom = RandomPlacement | RandomSizeVariance,
        Focused = FavorConnections | LowSizeVariance,
        Volatile = RandomPlacement | HighSizeVariance,
    }

    private const GenStyle PLACEMENT_FLAGS = GenStyle.RandomPlacement | GenStyle.FavorConnections;
    private const GenStyle SIZE_FLAGS = GenStyle.LowSizeVariance | GenStyle.RandomSizeVariance | GenStyle.HighSizeVariance;

    private enum Direction
    {
        NW,
        NE,
        SE,
        SW,
    }

    public const int MAX_WIDTH = 13;
    public const int MAX_HEIGHT = 13;
    public const int MAX_WEIGHT = 4;

    public int numCircles;

    public Grid layout;

    public Graph grid;

    public Dot dotPrefab;
    public Circle circlePrefab;

    public (RangeInt X, RangeInt Y) selectRegion;

    public (RangeInt X, RangeInt Y) localRegion;

    public GenStyle generationStyle;

    public AnimationCurve sizeCurve;
    private int lastWeight = 1;

    public List<PuzzleNode> nodesWithCircles = new List<PuzzleNode>();

    // Start is called before the first frame update
    void Start()
    {
        layout = GetComponent<Grid>();
        selectRegion = (new RangeInt(0, MAX_HEIGHT - 1), new RangeInt(0, MAX_WIDTH - 1));
        localRegion = selectRegion;

        grid = new Graph(MAX_HEIGHT, MAX_WIDTH, layout, dotPrefab, circlePrefab);
        grid.PopulateGrid();
        if ((generationStyle & GenStyle.RandomNumDots) != GenStyle.None)
            numCircles = UnityEngine.Random.Range(7, 25);
        CreateRandomTalisman(generationStyle);
    }

    public void CreateRandomTalisman() => CreateRandomTalisman(generationStyle);

    public void CreateRandomTalisman(GenStyle generationStyle)
    {
        grid.Clear();
        if (generationStyle == GenStyle.None) return;
        GenStyle sizeFlag = generationStyle & SIZE_FLAGS;
        GenStyle placementFlag = generationStyle & PLACEMENT_FLAGS;
        grid.PopulateGrid();
        for(int i = 0; i < numCircles; i++)
        {
            if (generationStyle == GenStyle.PureRandom)
            {
                CreateRandomCircle(localRegion);
            }
            else
            {
                (int, int) loc = (-1, -1);
                sizeCurve = sizeFlag switch
                {
                    _ when sizeFlag.HasFlag(GenStyle.LowSizeVariance) => GetLowVarianceCurve(lastWeight),
                    _ when sizeFlag.HasFlag(GenStyle.HighSizeVariance) => GetHighVarianceCurve(lastWeight),
                    _ => CreateUniformCurve(),
                };

                int weight = Mathf.FloorToInt(sizeCurve.Evaluate(UnityEngine.Random.value));

                switch (placementFlag)
                {
                    case GenStyle.RandomPlacement:
                        loc = GetRandomLoc(localRegion);
                        break;
                    case GenStyle.FavorConnections:
                        while (loc == (-1, -1))
                        {
                            loc = GetRandomLocByConnection(weight);
                        }
                        break;
                }

                if(!CreateCircleAt(loc.Item1, loc.Item2, weight, true))
                {
                    i--;
                }
            }
        }
    }

    private (PuzzleNode, CircleNode?) GetRandomCircle()
    {
        PuzzleNode p = nodesWithCircles.Count > 0 ? nodesWithCircles.RandomElement() : null;
        CircleNode? c = p?.Circle.Values.ToList()?.RandomElement();
        return (p, c);
    }

    public (int, int) GetRandomLocByConnection(int weight)
    {
        int INFINITE_LOOP = 0;
        (PuzzleNode p, CircleNode? c) = GetRandomCircle();
        if(p == null)
        {
            return GetRandomLoc(selectRegion);
        }
        else
        {
            Debug.Assert(c.HasValue);
            List<DotNode> dots = new List<DotNode>(c.Value.dots);
            while(dots.Count != 0 && INFINITE_LOOP < 100)
            {
                DotNode d = dots.RandomElement(); 
                List<Vector2Int> directions = new List<Vector2Int>
                {
                    Vector2Int.up,
                    -Vector2Int.up,
                    Vector2Int.right,
                    -Vector2Int.right
                };
                Vector2Int dir = directions.RandomElement();
                PuzzleNode newNode = null;
                int i = 0;
                for(; (newNode == null || newNode.HasCircleOfWeight(weight)) && i < 4; i++)
                {
                    if ((d.X + dir.x * weight).InRange(0, grid.Height - 1) && (d.Y + dir.y * weight).InRange(0, grid.Width - 1))
                        newNode = grid[d.X + (dir.x * weight), d.Y + (dir.y * weight)];
                    else
                        Debug.LogWarning("Node could not be created at " + d.X + dir.x * weight + ", " + d.Y + dir.y * weight);
                }
                if (i == 4)
                {
                    dots.Remove(d);
                    INFINITE_LOOP++;
                    continue;
                }

                return i == 4 ? (-1, -1) : (newNode.X, newNode.Y);
            }
            return (-1, -1);
        }
    }

    public void CreateKeyframe(float time, float value, AnimationCurve curve)
    {
        Keyframe frame = new Keyframe(time, value, 0.0f, 0.0f);
        int index = curve.AddKey(frame);
        Debug.Assert(index != -1, "Frame {" + frame + "} could not be added");
    }

    public int ConstrainFavored(int favored)
    {
        if (favored <= 0) return ConstrainFavored(MAX_WEIGHT + 1 + favored);
        if (favored > MAX_WEIGHT) return ConstrainFavored(favored - MAX_WEIGHT);
        return favored;
    }

    public AnimationCurve GetLowVarianceCurve(int favored)
    {
        AnimationCurve curve = new AnimationCurve();
        CreateKeyframe(0.0f, ConstrainFavored(favored), curve);
        CreateKeyframe(0.4f, ConstrainFavored(favored - 1), curve);
        CreateKeyframe(0.6f, ConstrainFavored(favored + 1), curve);
        CreateKeyframe(0.8f, ConstrainFavored(favored - 2), curve);
        int plusTwo = ConstrainFavored(favored + 2);
        CreateKeyframe(0.9f, plusTwo, curve);
        CreateKeyframe(1.0f, plusTwo, curve);
        return curve;
    }

    public AnimationCurve GetHighVarianceCurve(int favored)
    {
        AnimationCurve curve = new AnimationCurve();
        CreateKeyframe(0.0f, ConstrainFavored(favored + 2), curve);
        CreateKeyframe(0.3f, ConstrainFavored(favored - 2), curve);
        CreateKeyframe(0.6f, ConstrainFavored(favored + 1), curve);
        CreateKeyframe(0.75f, ConstrainFavored(favored - 1), curve);
        favored = ConstrainFavored(favored);
        CreateKeyframe(0.9f, favored, curve);
        CreateKeyframe(1.0f, favored, curve);
        return curve;
    }

    public AnimationCurve CreateUniformCurve()
    {
        return AnimationCurve.Linear(0.0f, 1.0f, 1.0f, MAX_WEIGHT + 1);
    }

    public void CreateRandomCircle((RangeInt X, RangeInt Y) region)
    {
        PuzzleNode node;
        int weight;
        do
        {
            node = GetRandomNode(region);
            weight = Mathf.Clamp(Mathf.RoundToInt(sizeCurve.Evaluate(UnityEngine.Random.value)), 0, MAX_WEIGHT);
        } while (!CreateCircleAt(node.X, node.Y, weight, true));
    }

    private (int, int) GetRandomLoc((RangeInt X, RangeInt Y) region)
    {
        GetRandomNode(region).Deconstruct(out int x, out int y);
        return (x, y);
    }

    private bool CreateCircleAt(int x, int y, int weight, bool setLocalRegion = false)
    {
        // If this circle already exists, return false
        if (grid[x, y].HasCircleOfWeight(weight)) return false;

        CircleNode newCircle = new CircleNode(x, y, weight, grid);
        if(newCircle.CompletedCircle)
        {
            grid[x, y].Circle.Add(weight, newCircle);
            nodesWithCircles.Add(grid[x, y]);
            lastWeight = weight;
            if(setLocalRegion)
                localRegion = GetLocalSelectRegion(x, y, weight + weight / 2);
            foreach(DotNode dot in grid[x, y].Circle[weight].dots)
            {
                grid.DisplayNode(grid[dot.X, dot.Y]);
            }
            return true;
        }

        return false;
    }

    private PuzzleNode GetRandomNode((RangeInt, RangeInt) region)
    {
        (float randomX, float randomY) = VectorUtils.GetRandomPoint_2D(selectRegion.X, selectRegion.Y);
        return grid[(int)randomX, (int)randomY];
    }

    public const int LOCAL_RANGE = 4;

    private (RangeInt, RangeInt) GetLocalSelectRegion(int x, int y, int range)
    {
        return (new RangeInt(x - range < 0 ? x : range, x + range > MAX_HEIGHT ? MAX_HEIGHT - x : range),
            new RangeInt(y - range < 0 ? y : range, y + range > MAX_WIDTH ? MAX_WIDTH - y : range));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
