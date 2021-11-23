using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{
    public Light light;
    public float length;
    public float minIntensity;
    public float maxIntensity;
    public AnimationCurve curve;

    private float duration;
    // Start is called before the first frame update
    void Start()
    {
        duration = 0;
        light.intensity = minIntensity;
    }

    // Update is called once per frame
    void Update()
    {
        duration += Time.deltaTime;
        light.intensity = Mathf.Clamp(curve.Evaluate(duration) * maxIntensity, minIntensity, maxIntensity);
    }
}
