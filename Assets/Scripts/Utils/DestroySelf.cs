using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using quiet.Timers;

public class DestroySelf : MonoBehaviour
{
    DoAfter da;
    public float Interval { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        Interval = 1;  
    }

    private void Update()
    {
        if(!da)
        {
            da = DoAfter.DoAfterFactory(() => Destroy(gameObject), Interval);
        }
    }
}
