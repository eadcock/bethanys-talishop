using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using quiet;

public class Flicker : MonoBehaviour
{
    public float minZ;
    public float maxZ;

    public Color minColor;
    public Color maxColor;

    private Light light;

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        /*float delta = Random.Range(-0.1f, 0.1f);
        float futurePos = transform.position.z + delta;
        if (Math.InRange(futurePos, minZ, maxZ))
            transform.position.Replace(delta, Dimension.Z);*/

        float[] variance = { -0.005f, 0.005f };
        Vector3 cDelta = new Vector3(Random.Range(variance[0], variance[1]), Random.Range(variance[0], variance[1]), Random.Range(variance[0], variance[1]));
        Color futureColor = new Color(light.color.r + cDelta.x, light.color.g + cDelta.y, light.color.b + cDelta.z);
        light.color = new Color(
            Math.InRange(futureColor.r, minColor.r, maxColor.r) ? futureColor.r : light.color.r,
            Math.InRange(futureColor.g, minColor.g, maxColor.g) ? futureColor.g : light.color.g,
            Math.InRange(futureColor.b, minColor.b, maxColor.b) ? futureColor.b : light.color.b);
    }
}
