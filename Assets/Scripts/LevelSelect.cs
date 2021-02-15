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

        int currentLevel = save.LoadFromProfile(save.CurrentProfile)?.currentLevel ?? 0;

        //Gets all level buttons
        Button[] levelButtons = GetComponentsInChildren<Button>();

        //Activates all levels up to current one
        for (int i = 0; i < currentLevel && i < levelButtons.Length; i++)
        {
            levelButtons[i].interactable = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
