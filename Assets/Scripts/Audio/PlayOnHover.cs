using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayOnHover : MonoBehaviour, IPointerEnterHandler
{
    public bool playSound;
    public string sound;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (playSound)
            GameMaster.Instance.Audio.PlaySoundFX(sound);
    }
}
