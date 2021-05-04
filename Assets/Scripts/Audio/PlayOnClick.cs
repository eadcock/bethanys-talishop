using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayOnClick : MonoBehaviour, IPointerClickHandler
{
    public bool playSound;
    public string sound;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (playSound)
            GameMaster.Instance.Audio.PlaySoundFX(sound);
    }
}
