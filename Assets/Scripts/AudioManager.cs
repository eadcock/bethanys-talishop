using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    GameObject musicPrefab;
    GameObject musicInstance;
    AudioSource musicAS;
    public bool IsPlaying => !musicAS.mute;

    // Start is called before the first frame update
    void Start()
    {
        if((musicInstance = GameObject.FindGameObjectWithTag("Music")) == null)
        {
            musicInstance = Instantiate(musicPrefab);
        } 

        musicAS = musicInstance.GetComponent<AudioSource>();
    }

    public void Play()
    {
        musicAS.mute = false;
    }

    public void Mute()
    {
        musicAS.mute = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
