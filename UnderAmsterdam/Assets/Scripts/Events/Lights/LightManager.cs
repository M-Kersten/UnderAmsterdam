using UnityEngine;
using System.Collections.Generic;
public class LightManager : MonoBehaviour
{
    [SerializeField] private Light lamp;
    public float minIntensity = 0f;
    public float maxIntensity = 1f;
    [Range(1, 50)]
    [SerializeField] private int smoothing = 5;

    private float timer = 0.75f;

    private Queue<float> smoothQueue;
    private float lastSum = 0;
    [SerializeField] private AudioSource sound;
    [SerializeField] private AudioClip clipSound;

    public void Reset()
    {
        smoothQueue.Clear();
        lastSum = 0;
    }

    private void Start()
    {
        smoothQueue = new Queue<float>(smoothing);
        // External or internal light?
        if (lamp == null)
        {
            lamp = GetComponent<Light>();
        }

    }

    private void Update()
    {
        timer = -Time.deltaTime;

        if (timer <= 0)
        {
            while (smoothQueue.Count >= smoothing)
            {
                lastSum -= smoothQueue.Dequeue();
            }

            // Generate random new item, calculate new average
            float newVal = Random.Range(minIntensity, maxIntensity);
            smoothQueue.Enqueue(newVal);
            lastSum += newVal;

            // Calculate new smoothed average
            lamp.intensity = lastSum / (float)smoothQueue.Count;
            sound.PlayOneShot(clipSound);
        }
    }

}