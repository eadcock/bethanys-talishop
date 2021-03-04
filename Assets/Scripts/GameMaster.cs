using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using quiet;

public enum GameState
{
    Intro,
    Playing,
    Paused,
    Outro,
    Ending
}

public class GameMaster : MonoBehaviour
{
    private static GameMaster _instance;

    public static GameMaster Instance => _instance == null ? _instance = FindObjectOfType<GameMaster>() : _instance;

    private SaveManager _save;

    public SaveManager Save => _save == null ? (_save = gameObject.AddComponent<SaveManager>()) : _save;

    private InputManager _input;

    public InputManager Input => _input == null ? (_input = gameObject.AddComponent<InputManager>()) : _input;

    private AudioManager _audio;

    public AudioManager Audio => _audio == null ? (_audio = gameObject.AddComponent<AudioManager>()) : _audio;

    private DialogueManager _dialogue;

    public DialogueManager Dialogue => _dialogue == null ? (_dialogue = gameObject.AddComponent<DialogueManager>()) : _dialogue;

    public StateManager<GameState> GameStateManager { get; private set; }
    public GameState CurrentGameState => GameStateManager.State;

    public Circle CurrentCircle = null;
    public Dot CurrentDot = null;

    public Circle circlePrefab;

    public GameObject endPanel;
    public GameObject forwardArrow;

    public int? ActiveLevel
    {
        get
        {
            string currentScene = SceneManager.GetActiveScene().name;
            if (!currentScene.Contains("Puzzle")) return null;

            if (!int.TryParse(currentScene.Substring(6), out int puzzleNumber))
            {
                puzzleNumber = 0;
            }

            return puzzleNumber;
        }
    }

    private void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("GameMaster").Length > 1)
        {
            DestroyImmediate(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance.GameStateManager = new StateManager<GameState>(GameState.Playing);
        SceneManager.sceneLoaded += InitIfNeeded;
        InitIfNeeded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }
    
    // If components don't currently exist, ensure that they do when a scene is loaded.
    public void InitIfNeeded(Scene scene, LoadSceneMode _)
    {
        if (_save is null)
            _save = gameObject.AddComponent<SaveManager>();

        if (_input is null)
            _input = gameObject.AddComponent<InputManager>();

        if (_audio is null)
            _audio = gameObject.AddComponent<AudioManager>();

        if (_dialogue is null)
        {
            _dialogue = gameObject.AddComponent<DialogueManager>();
            Dialogue.Init();
            Dialogue.LoadDialogue();
        }

        if (ActiveLevel != null)
        {
            GameObject[] endObjects = GameObject.FindGameObjectsWithTag("EndPanel");
            foreach(GameObject o in endObjects)
            {
                if (o.name == "Forward")
                {
                    forwardArrow = o;
                    if(ActiveLevel != null && Save.CurrentSaveData != null)
                    {
                        Debug.Log(Save.CurrentSaveData.currentLevel + " " + ActiveLevel);
                        forwardArrow.SetActive(Save.CurrentSaveData.currentLevel > ActiveLevel);
                    }
                    else
                    {
                        Debug.LogWarning("No save data");
                    }
                }

                if (o.name == "EndPanel")
                {
                    endPanel = o;
                    endPanel.SetActive(false);
                }
            }

            // Check if there is any intro dialogue
            if(Dialogue.ShouldStart(GameState.Intro))
            {
                Instance.GameStateManager.SwapState(GameState.Intro);
                Dialogue.StartPlayingDialogue(GameState.Playing);
            }
            else
            {
                Instance.GameStateManager.SwapState(GameState.Playing);
            }
        }
    }

    /// <summary>
    /// Attempt to start drawing a circle
    /// </summary>
    /// <returns>If we can start drawing</returns>
    public bool TryStartDrawing()
    {
        Circle circleToDelete;
        if ((CurrentDot = Input.GetDotOnMouse()) != null)
        {
            CurrentCircle = Instantiate(circlePrefab);  
            CurrentCircle.transform.position = new Vector3(CurrentDot.X, CurrentDot.Y, 0);
            CurrentCircle.ResizeCircle(new Vector2(CurrentDot.X, CurrentDot.Y), Input.MousePosition);
            return true;
        } else if ((circleToDelete = Input.GetCircleOnMouse()) != null)
        {
            circleToDelete.DeleteCircle();
            TestPuzzleComplete();
        }
        return false;
    }

    /// <summary>
    /// Stop drawing, validate, and finalize a drawn circle.
    /// </summary>
    public void StopDrawing()
    {
        Dot selectedDot = Input.GetDotOnMouse();
        if(selectedDot is null || selectedDot == CurrentDot)
        {
            Destroy(CurrentCircle.gameObject);
        }
        else
        {
            // Gets displacement between mouse position and starting dot position
            Vector2 displacement = new Vector2(selectedDot.X - CurrentDot.X, selectedDot.Y - CurrentDot.Y);
            // Gets displacement angle
            float angle = Vector2.SignedAngle(new Vector2(1, 0), displacement);
            Dot dot1, dot2;
            if(Mathf.Abs(angle) == 90)
            {
                dot1 = Input.GetDotAtPos(new Vector2(CurrentDot.X + displacement.y / 2, CurrentDot.Y + displacement.y / 2));
                dot2 = Input.GetDotAtPos(new Vector2(CurrentDot.X - displacement.y / 2, CurrentDot.Y + displacement.y / 2));
            }
            else if (angle == 0 || angle == 180)
            {
                dot1 = Input.GetDotAtPos(new Vector2(CurrentDot.X + displacement.x / 2, CurrentDot.Y + displacement.x / 2));
                dot2 = Input.GetDotAtPos(new Vector2(CurrentDot.X + displacement.x / 2, CurrentDot.Y - displacement.x / 2));
            }
            else
            {
                dot1 = dot2 = null;
            }

            if (dot1 == null || dot2 == null)
            {
                Destroy(CurrentCircle.gameObject);
            }
            else
            {
                IEnumerable<Circle> circles = GameObject.FindGameObjectsWithTag("Circle").Select(e => e.GetComponent<Circle>());
                bool doesCircleExist = false;
                foreach (Circle circle in circles)
                {
                    if (circle.ConnectedDots != null &&
                                !(
                                    !circle.ConnectedDots.Contains(dot1) ||
                                    !circle.ConnectedDots.Contains(dot2) ||
                                    !circle.ConnectedDots.Contains(selectedDot) ||
                                    !circle.ConnectedDots.Contains(CurrentDot)
                                ))
                    {
                        doesCircleExist = true;
                        break;
                    }
                }
                if (doesCircleExist)
                {
                    Destroy(CurrentCircle.gameObject);
                }
                else
                {
                    List<Dot> addedDots = new List<Dot>
                    {
                        dot1,
                        dot2,
                        selectedDot,
                        CurrentDot
                    };

                    CurrentCircle.AddCircle(addedDots);

                    CurrentDot = null;

                    TestPuzzleComplete();
                }
            }
        }
    }

    /// <summary>
    /// Logic for drawing the circle.
    /// </summary>
    public void DrawCircle()
    {
        Dot snapDot;
        if(snapDot = Input.GetDotOnMouse())
        {
            if((snapDot.X == CurrentCircle.X) || (snapDot.Y == CurrentCircle.Y))
            {
                CurrentCircle.ResizeCircle(new Vector2(CurrentDot.X, CurrentDot.Y), new Vector2(snapDot.X, snapDot.Y));
            }
            else
            {
                CurrentCircle.ResizeCircle(new Vector2(CurrentDot.X, CurrentDot.Y), Input.MousePosition);
            }
        }
        else
        {
            CurrentCircle.ResizeCircle(new Vector2(CurrentDot.X, CurrentDot.Y), Input.MousePosition);
        }
    }

    /// <summary>
    /// Determine if the puzzle is complete. If it is, begin end puzzle logic
    /// </summary>
    public void TestPuzzleComplete()
    {
        GameObject[] dotObjects = GameObject.FindGameObjectsWithTag("Dot");
        IEnumerable<Dot> dots = dotObjects.Select(o => o.GetComponent<Dot>());
        foreach (Dot dot in dots)
        {
            if (!dot.FinishedDot)
            {
                return;
            }
        }
        //End puzzle logic here
        BroadcastMessage("EndPuzzle", SendMessageOptions.DontRequireReceiver);
        if(Dialogue.ShouldStart(GameState.Outro))
        {
            Instance.GameStateManager.SwapState(GameState.Outro);
            Dialogue.StartPlayingDialogue(GameState.Ending);
        }
        else
        {
            Instance.GameStateManager.SwapState(GameState.Ending);
        }
        EndPuzzle();
    }

    /// <summary>
    /// End puzzle logic
    /// </summary>
    public void EndPuzzle()
    {
        if (ActiveLevel is null) return;

        SaveData data = Save.LoadFromProfile(Save.CurrentProfile);
        if (data != null)
        {
            if(ActiveLevel >= data.currentLevel)
            {
                Save.SaveToFile(Save.CurrentProfile, data.currentLevel + 1);
            }
        }
        else
        {
            Save.SaveToFile(Save.CurrentProfile, (int)ActiveLevel + 1);
        }

        foreach(GameObject o in Resources.FindObjectsOfTypeAll<GameObject>())
            if (o.name == "EndPanel" || o.name == "Forward")
                o.SetActive(true);
    }

    public void Pause()
    {
        GameObject.FindGameObjectWithTag("PausePanel").GetComponent<PauseManager>().Pause();
        Instance.GameStateManager.SwapState(GameState.Paused);
    }

    public void UnPause()
    {
        GameObject.FindGameObjectWithTag("PausePanel").GetComponent<PauseManager>().UnPause();
        Instance.GameStateManager.SwapState(GameState.Playing);
    }
}
