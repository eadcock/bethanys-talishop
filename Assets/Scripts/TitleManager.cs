using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TitleManager : MonoBehaviour
{
    public TextMeshPro text;
    private SceneTrans sceneTrans;
    public float textCountdown = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        sceneTrans = this.GetComponent<SceneTrans>();
    }

    // Update is called once per frame
    void Update()
    {
        // fade in text after a few seconds
        // text.setactive(true)
        if (textCountdown <= 0.0f)
        {
            text.text = "Press any key to continue";

            // check if user presses a key and go to tutorial
            if (Input.anyKeyDown)
            {
                sceneTrans.ToScene("LevelSelect");
            }
        }
        else
        {
            textCountdown -= Time.deltaTime;
        }

        
    }
}
