using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevCheats : MonoBehaviour
{
#if UNITY_EDITOR
    private readonly bool enabled = true;

#else
    private readonly bool enabled = false;
#endif
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(enabled);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
