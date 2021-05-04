using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using quiet;
using System.Linq;
using UnityEngine.InputSystem;

public enum ClickState
{
    Waiting,
    Dragging,
    Drawing
}

public class InputManager : MonoBehaviour
{ 
    public StateManager<ClickState> ClickStateManager { get; private set; }

    public ClickState CurrentClickState => ClickStateManager.State;

    public Vector2 MousePosition;

    // Start is called before the first frame update
    void Start()
    {
        ClickStateManager = new StateManager<ClickState>(ClickState.Waiting);
    }

    // Update is called once per frame
    void Update()
    {
        switch (GameMaster.Instance.CurrentGameState)
        {
            case GameState.Playing:
                GameUpdate();
                if (Keyboard.current.escapeKey.wasPressedThisFrame)
                { 
                    BroadcastMessage("Pause");
                }
                break;
            case GameState.Paused:
                if (Keyboard.current.escapeKey.wasPressedThisFrame)
                {
                    BroadcastMessage("UnPause");
                }
                break;
            case GameState.Ending:
            case GameState.Outro:
            case GameState.Intro:
                if (Keyboard.current.escapeKey.wasPressedThisFrame)
                {
                    BroadcastMessage("Pause");
                }
                else
                {
                    DialogueUpdate();
                }
                break;
        }
    }

    /// <summary>
    /// Core gameplay logic loop
    /// </summary>
    private void GameUpdate()
    {
        MousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()).StripZ();
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (!(ClickStateManager.State is ClickState.Drawing))
            {
                if(GameMaster.Instance.TryStartDrawing())
                {
                    ClickStateManager.SwapState(ClickState.Drawing);
                }
            }
            else
            {
                ClickStateManager.SwapState(ClickState.Waiting);
                GameMaster.Instance.StopDrawing();
            }
        }
        else if(Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (ClickStateManager.State is ClickState.Dragging)
                ClickStateManager.SwapState(ClickState.Waiting);
        }

        if (ClickStateManager.State is ClickState.Drawing)
        {
            GameMaster.Instance.DrawCircle();
        }

        if(GameMaster.Instance.CurrentCircle != null && GameMaster.Instance.CurrentCircle.IsDrawn)
        {
            GameMaster.Instance.CurrentCircle = null;
        }
    }

    public void DialogueUpdate()
    {
        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            GameMaster.Instance.Dialogue.DisplayNextLine();
        }
    }

    const float DEFAULT_ERROR = 0.125f;

    public Dot GetDotOnMouse(float error = DEFAULT_ERROR) => GetDotAtPos(MousePosition, error);

    public Dot GetDotAtPos(Vector2 pos, float error = DEFAULT_ERROR) =>
        GameObject.FindGameObjectsWithTag("Dot")
            .Select(o => o.GetComponent<Dot>())
            .FirstOrDefault(dot => quiet.Collision.BoundingCircle(new Vector3(dot.X, dot.Y, 0), error, pos.FillZDim(), error));

    public Circle GetCircleOnMouse(float error = DEFAULT_ERROR) => 
        GameObject.FindGameObjectsWithTag("Circle").Select(o => o.GetComponent<Circle>()).FirstOrDefault(circle =>
            {
                //Gets displacement between mouse position and the starting dot position
                Vector2 displacement = new Vector2(MousePosition.x - circle.X, MousePosition.y - circle.Y);
                //Gets displacements angle
                float angle = Vector2.SignedAngle(new Vector2(1, 0), displacement);
                angle *= Mathf.Deg2Rad;
                float circleX = circle.X + (Mathf.Cos(angle) * circle.Radius);
                float circleY = circle.Y + (Mathf.Sin(angle) * circle.Radius);
                //Debug.Log("Circle at: "+ circleX +","+ circleY + "Mouse at: " + mousePosition.x + "," +mousePosition.y);
                return quiet.Collision.BoundingCircle(new Vector3(circleX, circleY, 0), error, MousePosition.FillZDim(), error);
            });
}
