using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Mood
{
    Talk,
    Smile,
    Right,
}

public class Beth : MonoBehaviour
{
    public Mood currentMood;

    // Start is called before the first frame update
    void Start()
    {
        currentMood = Mood.Talk;

        UpdateSprite();
    }

    public void SwapMood(Mood newMood)
    {
        if (newMood == currentMood) return;

        currentMood = newMood;

        UpdateSprite();
    }

    public void UpdateSprite()
    {
        Sprite newSprite = SpriteLoader.LoadAndCreate(GetSpritePath(currentMood));
        if (newSprite != null)
            GetComponent<Image>().sprite = newSprite;
        else
            Debug.LogWarning("Sprite wasn't loaded correctly!");
    }

    public string GetSpritePath(Mood mood)
    {
        return "Beth/" + mood switch
        {
            Mood.Smile => "Front_Smile",
            Mood.Right => "Right",
            _ => "Front_Talk",
        };
    }
}
