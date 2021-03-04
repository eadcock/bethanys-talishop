using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class Mute : MonoBehaviour, IPointerClickHandler
{
    private AudioManager am;
    [SerializeField]
    private Sprite playingSprite;
    [SerializeField]
    private Sprite playingSprite_h;
    [SerializeField]
    private Sprite muteSprite;
    [SerializeField]
    private Sprite muteSprite_h;

    private ChangeOnHover coh;
    // Start is called before the first frame update
    void Start()
    {
        am = FindObjectOfType<AudioManager>();
        coh = GetComponent<ChangeOnHover>();
    }

    private void ToggleMute()
    {
        if (am.IsPlaying)
        {
            am.MuteMusic();
            coh.DefaultSprite = muteSprite;
            coh.HoverSprite = muteSprite_h;

            coh.Image.sprite = coh.Image.sprite == playingSprite ? muteSprite : muteSprite_h;
        }
        else
        {
            am.PlayMusic();
            coh.DefaultSprite = playingSprite;
            coh.HoverSprite = playingSprite_h;

            coh.Image.sprite = coh.Image.sprite == muteSprite ? playingSprite : playingSprite_h;
        }

        Debug.Log(am.IsPlaying);
    }

    public void OnPointerClick(PointerEventData pointerEventData) => ToggleMute();
}
