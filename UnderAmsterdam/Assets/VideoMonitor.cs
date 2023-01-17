using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine;

public class VideoMonitor : MonoBehaviour
{
    [SerializeField] private VideoPlayer myMonitor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnButtonPressed()
    {
        if (myMonitor.isPlaying) myMonitor.Pause();
        else myMonitor.Play();
    }
}
