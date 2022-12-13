using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class LightManager : MonoBehaviour
{
    [SerializeField] private Light lamp;
    public float minIntensity = -2f;
    public float maxIntensity = 2f;

    [SerializeField] private AudioSource sound;
    [SerializeField] private AudioClip clipSound;
    
    private void Start()
    {
        // External or internal light?
        if (lamp == null)
        {
            lamp = GetComponent<Light>();
        }

        RandManager.Instance.FlickeringLightsOn.AddListener(FlickeringOn);
        RandManager.Instance.FlickeringLightsOff.AddListener(FlickeringOff);
    }

    private IEnumerator Blinker()
    {
        //Time between intensity change
        float timing = Random.Range(0.065f, 0.5f);
        float randIntensity = Random.Range(minIntensity, maxIntensity);
        float current = lamp.intensity;
        float currentTime = 0f;
        float t = 0f;

        int i = 0;
        
        while (currentTime < timing)
        {
            t = i == 2 ? Mathf.Cos(Mathf.Pow((t % 4), 2)) * 0.3f : t / timing;
            lamp.intensity = Mathf.Lerp(current, randIntensity, t);

            currentTime += Time.deltaTime;
            i = (i + 1) % 3;
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