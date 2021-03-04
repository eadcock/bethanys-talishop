using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    public enum Direction
    {
        Down = -1,
        Up = 1,
    }

    [SerializeField]
    GameObject musicPrefab;
    AudioSource musicAS;
    public bool IsPlaying => !musicAS.mute;

    // Start is called before the first frame update
    void Start()
    {
        musicAS = gameObject.AddComponent<AudioSource>();
    }

    public void PlayMusic()
    {
        musicAS.Play();
    }

    public void PauseMusic()
    {
        musicAS.Pause();
    }

    public void MuteMusic()
    {
        musicAS.mute = true;
    }

    public void SetVolume(float vol)
    {
        musicAS.volume = Mathf.Clamp(vol, 0.0f, 1.0f);
    }

    public void ChangeVolume(Direction dir)
    {
        musicAS.volume = Mathf.Clamp01(dir == Direction.Down ? musicAS.volume - 0.1f : musicAS.volume + 0.1f);
    }
}
