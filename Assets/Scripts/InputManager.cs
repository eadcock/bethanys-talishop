using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using quiet;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum ClickState
{
    Waiting,
    Dragging,
    Drawing
}

public class InputManager : MonoBehaviour
{
    private AudioManager audioManager; 

    private bool isDrawing = false;

    private bool wasMuted = false;

    public bool Click => Input.GetAxis("Click") > 0;

    private bool isClickHeld;

    public bool IsClickHeld => isClickHeld;

    public StateManager<ClickState> ClickStateManager { get; private set; }

    public ClickState CurrentClickState => ClickStateManager.State;

    public Vector2 MousePosition;

    // Start is called before the first frame update
    void Start()
    {
        ClickStateManager = new StateManager<ClickState>(ClickState.Waiting);

        audioManager = GameMaster.Instance.Audio;
    }

    // Update is called once per frame
    void Update()
    {
        switch (GameMaster.Instance.CurrentGameState)
        {
            case GameState.Playing:
                GameUpdate();
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    BroadcastMessage("Pause");
                }
                break;
            case GameState.Paused:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    BroadcastMessage("UnPause");
                }
                break;
            case GameState.Outro:
            case GameState.Intro:
                if (Input.GetKeyDown(KeyCode.Escape))
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
        float click = Input.GetAxis("Click");
        MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition).StripZ();
        if (click > 0 && isClickHeld is false)
        {
            isClickHeld = true;
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
        else if (click is 0)
        {
            isClickHeld = false;
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
        float click = Input.GetAxis("Click");
        if(click > 0 && isClickHeld is false)
        {
            isClickHeld = true;
            GameMaster.Instance.Dialogue.DisplayNextLine();
        }
        else if (click is 0)
        {
            isClickHeld = false;
        }
    }

    const float DEFAULT_ERROR = 0.125f;

    public Dot GetDotOnMouse(float error = DEFAULT_ERROR) => GetDotAtPos(MousePosition, error);

    public Dot GetDotAtPos(Vector2 pos, float error = DEFAULT_ERROR)
    {
        GameObject[] dotObjects = GameObject.FindGameObjectsWithTag("Dot");
        Dot[] dots = dotObjects.Select(o => o.GetComponent<Dot>()).ToArray();
        foreach (Dot dot in dots)
        {
            if (quiet.Collision.BoundingCircle(new Vector3(dot.X, dot.Y, 0), error, pos.FillZDim(), error))
            {
                return dot;
            }
        }

        return null;
    }

    public Circle GetCircleOnMouse(float error = DEFAULT_ERROR)
    {
        GameObject[] circleObjects = GameObject.FindGameObjectsWithTag("Circle");
        Circle[] circles = circleObjects.Select(o => o.GetComponent<Circle>()).ToArray();
        foreach (Circle circle in circles)
        {
            //Gets displacement between mouse position and the starting dot position
            Vector2 displacement = new Vector2(MousePosition.x - circle.X, MousePosition.y - circle.Y);
            //Gets displacements angle
            float angle = Vector2.SignedAngle(new Vector2(1, 0), displacement);
            angle *= Mathf.Deg2Rad;
            float circleX = circle.X + (Mathf.Cos(angle) * circle.Radius);
            float circleY = circle.Y + (Mathf.Sin(angle) * circle.Radius);
            //Debug.Log("Circle at: "+ circleX +","+ circleY + "Mouse at: " + mousePosition.x + "," +mousePosition.y);
            if (quiet.Collision.BoundingCircle(new Vector3(circleX, circleY, 0), error, MousePosition.FillZDim(), error))
            {
                return circle;
            }
        }
        return null;
    }
}
