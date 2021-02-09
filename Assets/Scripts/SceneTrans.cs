using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrans : MonoBehaviour
{
    [SerializeField]
    int puzzleNumber;

    string currentScene;

    // Start is called before the first frame update
    void Start()
    {
        currentScene = SceneManager.GetActiveScene().name;
        if(currentScene.Contains("Puzzle") && !int.TryParse(currentScene.Substring(6), out puzzleNumber))
        {
            puzzleNumber = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToScene(string sceneName)
    {
        Initiate.Fade(sceneName, Color.black, 1);
    }

    public void GoForward()
    {
        string nextScene = SceneManager.GetSceneByName($"Puzzle{puzzleNumber + 1}") != null ? $"Puzzle{puzzleNumber + 1}" : "LevelSelect";
        Initiate.Fade(nextScene, Color.black, 1);
    }

    public void GoBackward()
    {
        Initiate.Fade(puzzleNumber - 1 == 0 ? "LevelSelect" : $"Puzzle{puzzleNumber - 1}", Color.black, 1);
    }

    public void FinishPuzzle()
    {
        if(puzzleNumber >= SaveData.current.currentLevel)
        {
            SaveData.current.currentLevel = puzzleNumber + 1;
            SerializationManager.Save(SaveData.SaveName, SaveData.current);
            Debug.Log("Game saved");
        }        
    }
}
