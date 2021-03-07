using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

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
        Collection<GameObject> objectsToMove = new Collection<GameObject>();
        if(SceneManager.GetActiveScene().name == "LevelSelect")
        {
            foreach(GameObject o in GameObject.FindGameObjectsWithTag("LevelButton"))
            {
                objectsToMove.Add(o);
            }
        }
        else
        {
            
        }
        Initiate.Fade(sceneName, Color.black, 1);
    }

    public void GoForward()
    {
        if (GameMaster.Instance.ActiveLevel != null)
        {
            string nextNumPuzzle = $"Puzzle{GameMaster.Instance.ActiveLevel + 1}";
            string nextScene = GameMaster.Instance.ActiveLevel + 1 != 24 ? nextNumPuzzle : "LevelSelect";
            Initiate.Fade(nextScene, Color.black, 1);
        }
        
    }

    public void GoBackward()
    {
        if (GameMaster.Instance.ActiveLevel is null) return;

        Initiate.Fade(GameMaster.Instance.ActiveLevel - 1 == 0 ? "LevelSelect" : $"Puzzle{GameMaster.Instance.ActiveLevel - 1}", Color.black, 1);
    }
}
