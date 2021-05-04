using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using quiet;

public class Dot : MonoBehaviour
{
    public int requiredCircles = 1;
    int currentCircles;

    public float X
    {
        get { return transform.position.x; }
    }

    public float Y
    {
        get { return transform.position.y; }
    }

    public bool FinishedDot
    {
        get { return requiredCircles == currentCircles; }
    }

    public void Deconstruct(out Vector2 position, out int required, out int current, out bool finished) => (position, required, current, finished) = (transform.position.StripZ(), requiredCircles, CurrentCircles, FinishedDot);

    public int CurrentCircles => currentCircles;

    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        currentCircles = 0;
        sr = gameObject.GetComponent<SpriteRenderer>();
        UpdateColor();
    }

    public void AddCircle()
    {
        currentCircles++;
        UpdateColor();
    }

    public void RemoveCircle()
    {
        if (currentCircles > 0)
        {
            currentCircles--;
        }
        UpdateColor();
    }

    public void Reset()
    {
        currentCircles = 0;
        UpdateColor();
    }

    void UpdateColor()
    {
        string spritePath = BuildSpritePath();
        Sprite newSprite = SpriteLoader.LoadAndCreate(spritePath);
        if (newSprite != null)
            sr.sprite = newSprite;

        if(spritePath == $"Gems/Corrupted/{requiredCircles} Corrupted")
        {
            GameMaster.Instance.Audio.PlaySoundFX("Power down");
        }
    }

    private string BuildSpritePath()
    {
        string path = "Gems/" + (requiredCircles - currentCircles) switch
        {
            0 => $"Done/{requiredCircles} Done",
            1 => $"{requiredCircles}/{requiredCircles}Blue",
            2 => $"{requiredCircles}/{requiredCircles}Purple",
            3 => $"{requiredCircles}/{requiredCircles}Cyan",
            4 => $"{requiredCircles}/{requiredCircles}Green",
            5 => $"{requiredCircles}/{requiredCircles}Orange",
            6 => $"{requiredCircles}/{requiredCircles}Pink",
            _ => $"Corrupted/{requiredCircles} Corrupted",
        };
        return path;
    }
}
