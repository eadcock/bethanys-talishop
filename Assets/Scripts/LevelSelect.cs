using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Loads save
        SaveData.current = (SaveData)SerializationManager.Load(Application.persistentDataPath + "/saves/" + SaveData.SaveName + ".save");

        //Gets all level buttons
        Button[] levelButtons = GetComponentsInChildren<Button>();

        //Activates all levels up to current one
        for (int i = 0; i < SaveData.current.currentLevel && i < levelButtons.Length; i++)
        {
            levelButtons[i].interactable = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
