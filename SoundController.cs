using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    AudioSource[] aSources;

    // Start is called before the first frame update
    void Start()
    {
        aSources = GetComponents<AudioSource>();
    }

    public void PlaySound(int a)
    {
        Debug.Log("Sound controller playing sound " + a + ", length is " + aSources.Length);
        if (a >= 0 && a < aSources.Length)
        {
            aSources[a].Play();
        }
    }

    public bool IsPlayingIndex(int a)
    {
        if (aSources[a].isPlaying)
            return true;

        return false;
    }

    public void StopSound(int a)
    {
        if (aSources[a].isPlaying)
            aSources[a].Stop();
    }

}
