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
    public Light light;
    // Start is called before the first frame update
    void Start()
    {
        currentCircles = 0;
        UpdateColor();
    }

    // Update is called once per frame
    void Update()
    {

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
        sr = gameObject.GetComponent<SpriteRenderer>();
        Debug.Log(requiredCircles - currentCircles);
        Color newColor = requiredCircles - currentCircles >= 0 ? Colors[requiredCircles - currentCircles] : Color.red;
        sr.color = light.color = newColor;
        if (sr.color.a != 1)
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1);
    }
}
