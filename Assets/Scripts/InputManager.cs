using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using quiet;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public enum States
    {
        gameState,
        pauseState,
        endState
    }
    States currentState;
    public Circle circlePrefab;

    public Circle currentCircle;
    public Dot currentDot;

    public GameObject endPanel;
    public GameObject pausePanel;
    public GameObject forward;

    [SerializeField]
    private AudioManager audioManager; 

    private bool isDrawing = false;
    private bool isButtonHeld = false;

    private bool wasMuted = false;

    // Start is called before the first frame update
    void Start()
    {
        currentState = States.gameState;
        audioManager = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case States.gameState:
                GameStateUpdate();
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    ChangeState(1);
                    if (audioManager.IsPlaying)
                    {
                        wasMuted = false;
                        audioManager.Mute();
                    }
                    else
                    {
                        wasMuted = true;
                    }
                }
                break;
            case States.pauseState:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    ChangeState(0);
                    if (!wasMuted)
                    {
                        audioManager.Play();
                    }
                }
                break;
        }
    }

    private void GameStateUpdate()
    {
        float click = Input.GetAxis("Click");
        if (click > 0 && isButtonHeld == false)
        {
            isButtonHeld = true;
            if (!isDrawing)
            {
                isDrawing = true;

                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition).StripZ();
                Circle circleToDelete;
                if ((currentDot = GetDotOnMouse(mousePosition)) != null)
                {
                    currentCircle = Instantiate(circlePrefab);
                    currentCircle.transform.position = new Vector3(currentDot.X, currentDot.Y, 0);
                    currentCircle.ResizeCircle(new Vector2(currentDot.X, currentDot.Y), mousePosition);
                    isDrawing = true;
                }
                else if((circleToDelete = GetCircleOnMouse(mousePosition)) != null)
                {
                    circleToDelete.DeleteCircle();
                    IsPuzzleComplete();
                    isDrawing = false;
                }
                else
                {
                    isDrawing = false;
                }
            }
            else
            {
                isDrawing = false;
                Dot selectedDot = GetDotOnMouse(Camera.main.ScreenToWorldPoint(Input.mousePosition).StripZ());
                if (!selectedDot)
                {
                    Destroy(currentCircle.gameObject);
                }
                else
                {
                    //Gets displacement between mouse position and the starting dot position
                    Vector2 displacement = new Vector2(selectedDot.X - currentDot.X, selectedDot.Y - currentDot.Y);
                    //Gets displacements angle
                    float angle = Vector2.SignedAngle(new Vector2(1, 0), displacement);

                    Dot dot1, dot2;
                    if (Mathf.Abs(angle) == 90)
                    {
                        dot1 = GetDotOnMouse(new Vector2(currentDot.X + displacement.y / 2, currentDot.Y + displacement.y / 2));
                        dot2 = GetDotOnMouse(new Vector2(currentDot.X - displacement.y / 2, currentDot.Y + displacement.y / 2));
                    }
                    else if (angle == 00 || angle == 180)
                    {
                        dot1 = GetDotOnMouse(new Vector2(currentDot.X + displacement.x / 2, currentDot.Y + displacement.x / 2));
                        dot2 = GetDotOnMouse(new Vector2(currentDot.X + displacement.x / 2, currentDot.Y - displacement.x / 2));
                    }
                    else
                    {
                        dot1 = dot2 = null;
                    }
                    
                    if (dot1 != null && dot2 != null)
                    {
                        Circle[] circles = GameObject.FindGameObjectsWithTag("Circle").Select(e => e.GetComponent<Circle>()).ToArray();
                        bool doesCircleExist = false;
                        foreach (Circle circle in circles)
                        {
                            if (circle.ConnectedDots != null && 
                                !(
                                    !circle.ConnectedDots.Contains(dot1) ||
                                    !circle.ConnectedDots.Contains(dot2) ||
                                    !circle.ConnectedDots.Contains(selectedDot) ||
                                    !circle.ConnectedDots.Contains(currentDot)
                                ))
                            {
                                doesCircleExist = true;
                                break;
                            }
                        }
                        if(doesCircleExist)
                        {
                            Destroy(currentCircle.gameObject);
                        }
                        else
                        {
                            List<Dot> addedDots = new List<Dot>
                            {
                                dot1,
                                dot2,
                                selectedDot,
                                currentDot
                            };

                            currentCircle.AddCircle(addedDots);

                            currentDot = null;
                            //Checks to see if puzzle is finished
                            IsPuzzleComplete();
                        }
                        
                    }
                    else
                    {
                        Destroy(currentCircle.gameObject);
                    }
                    
                }
            }
        }
        else if (click == 0)
        {
            isButtonHeld = false;
        }

        if (isDrawing)
        {
            Dot snapDot;
            if (snapDot = GetDotOnMouse(Camera.main.ScreenToWorldPoint(Input.mousePosition).StripZ(), 0.1f))
            {
                if ((snapDot.X == currentCircle.X) || (snapDot.Y == currentCircle.Y))
                {
                    currentCircle.ResizeCircle(new Vector2(currentDot.X, currentDot.Y), new Vector2(snapDot.X, snapDot.Y));
                }
                else
                {
                    currentCircle.ResizeCircle(new Vector2(currentDot.X, currentDot.Y), Camera.main.ScreenToWorldPoint(Input.mousePosition).StripZ());
                }
            }
            else
            {
                currentCircle.ResizeCircle(new Vector2(currentDot.X, currentDot.Y), Camera.main.ScreenToWorldPoint(Input.mousePosition).StripZ());
            }
        }
        if(currentCircle != null && currentCircle.IsDrawn)
        {
            currentCircle = null;
        }
    }

    private Dot GetDotOnMouse(Vector2 mousePosition, float error = 0.1f)
    {
        GameObject[] dotObjects = GameObject.FindGameObjectsWithTag("Dot");
        Dot[] dots = dotObjects.Select(o => o.GetComponent<Dot>()).ToArray();
        foreach (Dot dot in dots)
        {
            if (quiet.Collision.BoundingCircle(new Vector3(dot.X, dot.Y, 0), error, mousePosition.FillZDim(), error))
            {
                return dot;
            }
        }

        return null;
    }
    private Circle GetCircleOnMouse(Vector2 mousePosition, float error = 0.1f)
    {
        GameObject[] circleObjects = GameObject.FindGameObjectsWithTag("Circle");
        Circle[] circles = circleObjects.Select(o => o.GetComponent<Circle>()).ToArray();
        foreach (Circle circle in circles)
        {
            //Gets displacement between mouse position and the starting dot position
            Vector2 displacement = new Vector2(mousePosition.x - circle.X, mousePosition.y - circle.Y);
            //Gets displacements angle
            float angle = Vector2.SignedAngle(new Vector2(1, 0), displacement);
            angle *= Mathf.Deg2Rad;
            float circleX = circle.X + (Mathf.Cos(angle) * circle.Radius);
            float circleY = circle.Y + (Mathf.Sin(angle) * circle.Radius);
            //Debug.Log("Circle at: "+ circleX +","+ circleY + "Mouse at: " + mousePosition.x + "," +mousePosition.y);
            if (quiet.Collision.BoundingCircle(new Vector3(circleX, circleY, 0), error, mousePosition.FillZDim(), error))
            {
                return circle;
            }
        }
        return null;
    }

    private void IsPuzzleComplete()
    {
        GameObject[] dotObjects = GameObject.FindGameObjectsWithTag("Dot");
        Dot[] dots = dotObjects.Select(o => o.GetComponent<Dot>()).ToArray();
        foreach (Dot dot in dots)
        {
            if (!dot.FinishedDot)
            {
                return;
            }
        }
        //End puzzle logic here
        EndPuzzle();
    }

    private async void EndPuzzle()
    {
        //End puzzle logic here
        FindObjectOfType<SceneTrans>().FinishPuzzle();
        await Task.Delay(1000);
        endPanel.SetActive(true);
        forward.SetActive(true);
    }

    public void ChangeState(int newState)
    {
        if(newState > 2)
        {
            return;
        }
        currentState = (States)newState;
        pausePanel.SetActive(currentState == States.pauseState);
    }
    public void RestartPuzzle()
    {
        //Gets and resets all dots
        GameObject[] dotObjects = GameObject.FindGameObjectsWithTag("Dot");
        Dot[] dots = dotObjects.Select(o => o.GetComponent<Dot>()).ToArray();
        foreach (Dot dot in dots)
        {
            dot.Reset();
        }

        //Gets and destroys all circles
        GameObject[] circles = GameObject.FindGameObjectsWithTag("Circle");
        for(int i = circles.Length -1; i >= 0; i--)
        {
            Destroy(circles[i]);
        }

        //Returns to game state
        ChangeState(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
