using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{

    [SerializeField]
    int requiredCircles = 1;
    int currentCircles;

    public Color[] Colors;

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

    public int CurrentCircles => currentCircles;

    private SpriteRenderer sr;
    private Color circleColor;

    // Start is called before the first frame update
    void Start()
    {
        currentCircles = 0;
        sr = gameObject.GetComponent<SpriteRenderer>();
        UpdateColor();
    }

    // Update is called once per frame
    void Update() { }

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
        Sprite newSprite = SpriteLoader.LoadAndCreate(BuildSpritePath());
        if (newSprite != null)
            sr.sprite = newSprite;
    }

    private string BuildSpritePath()
    {
        string path = "Gems/";
        switch (requiredCircles - currentCircles)
        {
            case 0:
                path += $"Done/{requiredCircles} Done";
                break;
            case 1:
                path += $"{requiredCircles}/{requiredCircles}Blue";
                break;
            case 2:
                path += $"{requiredCircles}/{requiredCircles}Purple";
                break;
            case 3:
                path += $"{requiredCircles}/{requiredCircles}Cyan";
                break;
            case 4:
                path += $"{requiredCircles}/{requiredCircles}Green";
                break;
            case 5:
                path += $"{requiredCircles}/{requiredCircles}Orange";
                break;
            case 6:
                path += $"{requiredCircles}/{requiredCircles}Pink";
                break;
            default:
                path += $"Corrupted/{requiredCircles} Corrupted";
                break;
        }

        return path;
    }
}
