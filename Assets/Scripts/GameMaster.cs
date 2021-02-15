using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public int? ActiveLevel
    {
        get
        {
            string currentScene = SceneManager.GetActiveScene().name;
            if (!currentScene.Contains("Puzzle")) return null;

            int puzzleNumber = 0;
            if (!int.TryParse(currentScene.Substring(6), out puzzleNumber))
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
    }

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
        EndPuzzle();
    }

    public async void EndPuzzle()
    {
        if (ActiveLevel is null) return;

        SaveManager save = GameMaster.Instance.Save;
        SaveData data = save.LoadFromProfile(save.CurrentProfile);
        if (data != null && ActiveLevel >= data.currentLevel)
        {
            save.SaveToProfile(save.CurrentProfile, data.currentLevel + 1);
        }

        await Task.Delay(1000);
        GameObject.Find("/GameUI/EndPanel").SetActive(true);
        GameObject.Find("/GameUI/Forward").SetActive(true);
    }
}
