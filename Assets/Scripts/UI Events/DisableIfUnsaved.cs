using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisableIfUnsaved : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        bool disable = (SaveData.current.currentLevel - 1 == int.Parse(SceneManager.GetActiveScene().name.Substring(6)));
        Debug.Log(disable);
        gameObject.SetActive(disable);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
