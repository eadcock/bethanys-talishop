using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LastPuzzle : MonoBehaviour, IPointerClickHandler
{
    private SceneTrans trans;
    // Start is called before the first frame update
    void Start()
    {
        trans = GetComponentInParent<SceneTrans>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(GameMaster.Instance.ActiveLevel == 1)
        {
            GameMaster.SceneTransitioner.ToScene("LevelSelect");
        }
        else
        {
            GameMaster.SceneTransitioner.GoBackward();
        }
    }
}
