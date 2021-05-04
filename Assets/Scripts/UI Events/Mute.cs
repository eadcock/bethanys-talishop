using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class Mute : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Sprite playingSprite;
    [SerializeField]
    private Sprite playingSprite_h;
    [SerializeField]
    private Sprite muteSprite;
    [SerializeField]
    private Sprite muteSprite_h;

    private (Sprite idle, Sprite hover) PlayingSprites => (playingSprite, playingSprite_h);
    private (Sprite idle, Sprite hover) MuteSprites => (muteSprite, muteSprite_h);

    private ChangeOnHover coh;
    // Start is called before the first frame update
    void Start()
    {
        coh = GetComponent<ChangeOnHover>();
    }

    private void SwapState((Sprite idle, Sprite hover) sprites)
    {
        (coh.DefaultSprite, coh.HoverSprite) = sprites;
        coh.Image.sprite = coh.Image.sprite == sprites.idle ? sprites.idle : sprites.hover;
    }

    private void ToggleMute()
    {
        if (GameMaster.Instance.Audio.IsMusicPlaying)
        {
            GameMaster.Instance.Audio.MuteMusic();
            SwapState(MuteSprites);
        }
        else
        {
            GameMaster.Instance.Audio.PlayMusic();
            SwapState(PlayingSprites);
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData) => ToggleMute();
}
