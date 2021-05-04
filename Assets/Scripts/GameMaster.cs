using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using quiet;
using System;

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
    public const int NUM_PUZZLES = 23;

    private static GameMaster _instance;

    public static GameMaster Instance => _instance ??= FindObjectOfType<GameMaster>();

    private static SaveManager _save;

    public SaveManager Save => _save ??= gameObject.AddComponent<SaveManager>();

    private static InputManager _input;

    public InputManager Input => _input ??= gameObject.AddComponent<InputManager>();

    private static AudioManager _audio;

    public AudioManager Audio => _audio ??= GetComponentInChildren<AudioManager>();

    private static DialogueManager _dialogue;

    public DialogueManager Dialogue => _dialogue ??= gameObject.AddComponent<DialogueManager>();

    private static MoveManager _move;

    public MoveManager MoveManager {
        get
        {
            if(_move == null)
            {
                if(!TryGetComponent<MoveManager>(out _move))
                {
                    _move = gameObject.AddComponent<MoveManager>();
                }
            }

            return _move;
        }
    }
    public static SceneTrans SceneTransitioner { get; private set; }

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

        SceneTransitioner = GetComponent<SceneTrans>();

#if UNITY_EDITOR
        _ = Instance.Save;
        Instance.Save.CurrentProfile = "Dev";
        Instance.Save.SaveLevelToProfile("Dev", NUM_PUZZLES / 2);
#endif
    }
    
    // If components don't currently exist, ensure that they do when a scene is loaded.
    public void InitIfNeeded(Scene scene, LoadSceneMode lsm)
    {
        _ = Instance.Save;
        _ = Instance.Input;
        _ = Instance.Audio;
        _ = Instance.MoveManager;

        SceneTransitioner ??= gameObject.AddComponent<SceneTrans>();

        if (ActiveLevel != null)
        {
            if (_dialogue is null)
            {
                _ = Instance.Dialogue;
                Instance.Dialogue.Init();
                Instance.Dialogue.LoadDialogue();
            }
            else
            {
                Instance.Dialogue.Init();
            }

            foreach (GameObject o in GameObject.FindGameObjectsWithTag("EndPanel"))
            {
                if (o.name == "Forward")
                {
                    forwardArrow = o;
                }

                if (o.name == "EndPanel")
                {
                    endPanel = o;
                    endPanel.SetActive(false);
                }
            }

            // Check if there is any intro dialogue
            if (Instance.Dialogue.ShouldStart(GameState.Intro))
            {
                Instance.GameStateManager.SwapState(GameState.Intro);
                Instance.Dialogue.StartPlayingDialogue(GameState.Playing);
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
        } 
        else if ((circleToDelete = Input.GetCircleOnMouse()) != null)
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
            (Dot, Dot) peripherals = angle switch {
                float a when Mathf.Abs(angle) is 90.0f => 
                    (Input.GetDotAtPos(new Vector2(CurrentDot.X + displacement.y / 2, CurrentDot.Y + displacement.y / 2)),
                    Input.GetDotAtPos(new Vector2(CurrentDot.X - displacement.y / 2, CurrentDot.Y + displacement.y / 2))),
                float a when angle is 0.0f || angle is 180.0f =>
                    (Input.GetDotAtPos(new Vector2(CurrentDot.X + displacement.x / 2, CurrentDot.Y + displacement.x / 2)),
                    Input.GetDotAtPos(new Vector2(CurrentDot.X + displacement.x / 2, CurrentDot.Y - displacement.x / 2))),
                _ => (null, null)
            };

            if (peripherals is (null, null))
            {
                Destroy(CurrentCircle.gameObject);
            }
            else
            {
                (Dot dot1, Dot dot2) = peripherals;
                bool doesCircleExist = false;
                foreach (Circle circle in GameObject.FindGameObjectsWithTag("Circle").Select(e => e.GetComponent<Circle>()))
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
                    CurrentCircle.AddCircle(new Dot[] {
                        dot1,
                        dot2,
                        selectedDot,
                        CurrentDot
                    });

                    MoveManager.ClearUndone();
                    MoveManager.PushMove(CurrentCircle);

                    CurrentDot = null;

                    TestPuzzleComplete();
                }
            }
        }
    }

    /// <summary>
    /// Logic for drawing the circle.
    /// </summary>
    public void DrawCircle() => 
        CurrentCircle.ResizeCircle(
            new Vector2(CurrentDot.X, CurrentDot.Y),
            Input.GetDotOnMouse() switch
            {
                null => Input.MousePosition,
                var snapDot => snapDot.transform.position switch
                {
                    var (x, y, _) when x == CurrentDot.X || y == CurrentDot.Y => new Vector2(x, y),
                    _ => Input.MousePosition,
                },
            });

    /// <summary>
    /// Determine if the puzzle is complete. If it is, begin end puzzle logic
    /// </summary>
    public void TestPuzzleComplete()
    {
        // If there is a single dot not completed, the puzzle isn't complete!
        if (GameObject.FindGameObjectsWithTag("Dot").Select(o => o.GetComponent<Dot>()).Any(d => !d.FinishedDot)) return;

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

        Instance.Audio.PlaySoundFX("Complete");

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

        MoveManager.Reset();
    }

    private static GameState stateBeforePause;

    public void Pause()
    {
        if (SceneManager.GetActiveScene().name == "Title") return;
        GameObject.FindGameObjectWithTag("PausePanel").GetComponent<PauseManager>().Pause();
        stateBeforePause = Instance.CurrentGameState;
        Instance.GameStateManager.SwapState(GameState.Paused);
    }

    public void UnPause()
    {
        GameObject.FindGameObjectWithTag("PausePanel").GetComponent<PauseManager>().UnPause();
        Instance.GameStateManager.SwapState(stateBeforePause);
    }

    public void HidePause() => GameObject.FindGameObjectWithTag("PausePanel").GetComponent<PauseManager>().UnPause();

    public static void Quit()
    {
        Instance.Save.SaveToFile(Instance.Save.CurrentProfile);
        Application.Quit();
    }

    public static void SwapToScene(string scene) => SceneTransitioner.ToScene(scene);

    public static void SwapToScene(Scene scene) => SceneTransitioner.ToScene(scene.name);

    public void RestartPuzzle()
    {
        //Gets and resets all dots
        foreach (Dot dot in GameObject.FindGameObjectsWithTag("Dot").Select(o => o.GetComponent<Dot>()).AsEnumerable())
        {
            dot.Reset();
        }

        //Gets and destroys all circles
        GameObject[] circles = GameObject.FindGameObjectsWithTag("Circle");
        for (int i = circles.Length - 1; i >= 0; i--)
        {
            Destroy(circles[i]);
        }

        MoveManager.Reset();

        //Returns to game state
        Instance.GameStateManager.SwapState(0);
    }

    public void Undo() => MoveManager.Undo();
    public void Redo() => MoveManager.Redo();
}
