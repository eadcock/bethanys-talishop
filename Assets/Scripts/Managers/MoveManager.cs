using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MoveManager : MonoBehaviour
{
    /// <summary>
    /// Read: redo stack. Keeps track of deleted circles
    /// </summary>
    private List<Circle> undoneMoves;

    /// <summary>
    /// A "stack" of circles representing the player's moves. Lastest is at the end of the list
    /// </summary>
    public List<Circle> moves;

    public bool CanRedo => undoneMoves.Count != 0;

    public int Moves { get; private set; }

    /// <summary>
    /// Add a move to the moves list
    /// </summary>
    /// <param name="circle"></param>
    public void PushMove(Circle circle)
    {
        Push(moves, circle);
        Moves++;
    }

    public void Start()
    {
        undoneMoves = new List<Circle>();
        moves = new List<Circle>();
        Moves = 0;
    }

    /// <summary>
    /// Add a circle to a desired list
    /// </summary>
    /// <param name="list"></param>
    /// <param name="c"></param>
    private void Push(List<Circle> list, Circle c) => list.Add(c);

    /// <summary>
    /// Remove a circle from the desired list and return it
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    private Circle Pop(List<Circle> list)
    {
        Circle c = list.Last();
        list.RemoveAt(list.Count - 1);
        return c;
    }

    /// <summary>
    /// Retrieve the last added circle to a list
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    private Circle Peek(List<Circle> list) => list.Last();

    /// <summary>
    /// Undo the latest move, if there is one.
    /// </summary>
    public void Undo()
    {
        if (moves.Count == 0) return;
        if (!Peek(moves))
        {
            Pop(moves);
            Undo();
            return;
        }
        Push(undoneMoves, Pop(moves));
        if (Peek(undoneMoves))
            Peek(undoneMoves).PhantomDelete();
        else
        {
            Undo();
            return;
        }
        Moves++;
    }

    /// <summary>
    /// Redo the last move, if there is one.
    /// </summary>
    public void Redo()
    {
        if (undoneMoves.Count == 0) return;
        if (!Peek(undoneMoves))
        {
            Pop(undoneMoves);
            Redo();
        }
        Push(moves, Pop(undoneMoves));
        if (Peek(moves))
            Peek(moves).AddCircle(Peek(moves).ConnectedDots);
        else Redo();
        Moves++;
    }

    /// <summary>
    /// Empties the undone list
    /// </summary>
    public void ClearUndone()
    {
        undoneMoves?.Clear();
    }

    /// <summary>
    /// Empties all tracked moves
    /// </summary>
    public void Reset()
    {
        undoneMoves?.Clear();
        moves?.Clear();
        Moves = 0;
    }
}
