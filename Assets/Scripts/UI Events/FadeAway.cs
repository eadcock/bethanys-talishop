using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeAway : MonoBehaviour
{
    public int interval;

    private float startTime;
    private Image image;

    public void Start()
    {
        image = GetComponent<Image>();
        startTime = Time.time;
    }
    
    public void Update()
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(255.0f, 0.0f, quiet.Math.Map(Time.time - startTime, 0, interval, 0, 1)));
    }
}
