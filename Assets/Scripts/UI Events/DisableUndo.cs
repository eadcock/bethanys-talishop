using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisableUndo : MonoBehaviour
{
    bool lastState = true;
    // Update is called once per frame
    void Update()
    {
        if(GameMaster.Instance.MoveManager.moves.Count != 0 && !lastState)
        {
            Enable(true);
            lastState = true;
        }
        else if (GameMaster.Instance.MoveManager.moves.Count == 0 && lastState)
        {
            Enable(false);
            lastState = false;
        }
    }

    private void Enable(bool enabled)
    {
        GetComponent<Image>().enabled = enabled;
        GetComponent<Button>().enabled = enabled;
    }
}
