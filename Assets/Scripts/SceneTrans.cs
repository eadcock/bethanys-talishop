using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrans : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
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
        if (GameMaster.Instance.ActiveLevel != null)
        {
            string nextNumPuzzle = $"Puzzle{GameMaster.Instance.ActiveLevel + 1}";
            string nextScene = SceneManager.GetSceneByName(nextNumPuzzle) != null ? nextNumPuzzle : "LevelSelect";
            Initiate.Fade(nextScene, Color.black, 1);
        }
        
    }

    public void GoBackward()
    {
        if (GameMaster.Instance.ActiveLevel is null) return;

        Initiate.Fade(GameMaster.Instance.ActiveLevel - 1 == 0 ? "LevelSelect" : $"Puzzle{GameMaster.Instance.ActiveLevel - 1}", Color.black, 1);
    }
}
