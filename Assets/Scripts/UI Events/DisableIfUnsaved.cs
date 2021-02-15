﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisableIfUnsaved : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        if (GameMaster.Instance.Save.CurrentSaveData != null)
        {
            bool disable = GameMaster.Instance.Save.CurrentSaveData.currentLevel - 1 == GameMaster.Instance.ActiveLevel;
            gameObject.SetActive(disable);
        }
        else
        {
            gameObject.SetActive(true);
        }
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
