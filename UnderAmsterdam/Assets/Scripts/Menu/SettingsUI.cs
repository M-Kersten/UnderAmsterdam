using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private AudioMixer volumeMixer;
    private bool leftHandedMode;

    public void SetVolume(string sliderType, float sliderValue)
    {
        volumeMixer.SetFloat(sliderType, Mathf.Log10(sliderValue) * 20);
    }

    public void LeftHanded()
    {
        if (leftHandedMode)
        {
            leftHandedMode = false;
        } else
        {
            leftHandedMode = true;
        }
    }
}
