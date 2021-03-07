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
        SaveManager save = GameMaster.Instance.Save;

        save.LoadFile(save.CurrentProfile);

        if (save.RequireProfileInit) save.SaveToProfile(save.CurrentProfile, 1);

        int currentLevel = save.LoadFromProfile(save.CurrentProfile)?.currentLevel ?? 1;

        Debug.Log(currentLevel);

        //Gets all level buttons
        Button[] levelButtons = GetComponentsInChildren<Button>();


        for (int i = 0; i < levelButtons.Length; i++)
        {
            levelButtons[i].interactable = i < currentLevel;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
