using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class LightManager : MonoBehaviour
{
    [SerializeField] private List<Light> lamp;
    public float minIntensity = -2f;
    public float maxIntensity = 2f;

    [SerializeField] private AudioSource sound;
    [SerializeField] private AudioClip clipSound;
    
    private void Start()
    {
        RandManager.Instance.FlickeringLightsOn.AddListener(FlickeringOn);
        RandManager.Instance.FlickeringLightsOff.AddListener(FlickeringOff);
    }

    private IEnumerator Blinker()
    {
        //Time between intensity change
        float timing = Random.Range(0.065f, 0.5f);
        float[] randIntensity = new float[lamp.Count];
        float[] currents = new float[lamp.Count];
        float currentTime = 0f;
        float t = 0f;

        int i = 0;
        
        for (int j = 0; j < currents.Length; j++)
        {
            currents[j] = lamp[j].intensity;
            randIntensity[j] = Random.Range(minIntensity, maxIntensity);
        }

        while (currentTime < timing)
        {
            t = i == 1 ? Mathf.Cos(Mathf.Pow((t % 4), 2)) * 0.3f : t / timing;
 
            for (int j = 0; j < currents.Length; j++)
            {
                lamp[j].intensity = Mathf.Lerp(currents[j], randIntensity[j], t);
            }

            currentTime += Time.deltaTime;
            i = (i + 1) % 14;
            yield return null;
        }
        StartCoroutine(Blinker());
    }

    private void FlickeringOn()
    {
        //isStarted = true;
        StartCoroutine(Blinker());
    }

    private void FlickeringOff()
    {
        //isStarted = false;
        StopAllCoroutines();
    }

}