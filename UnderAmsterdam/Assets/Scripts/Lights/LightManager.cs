using UnityEngine;
using System.Collections.Generic;
public class LightManager : MonoBehaviour
{

    [Tooltip("External light to flicker; you can leave this null if you attach script to a light")]
    [SerializeField] private Light lamp;
    //[SerializeField] private new Light light;
    [Tooltip("Minimum random light intensity")]
    public float minIntensity = 0f;
    [Tooltip("Maximum random light intensity")]
    public float maxIntensity = 1f;
    [Tooltip("How much to smooth out the randomness; lower values = sparks, higher = lantern")]
    [Range(1, 50)]
    [SerializeField] private int smoothing = 5;
    private float timer = 0.75f;
    
    private Queue<float> smoothQueue;
    private float lastSum = 0;
    [SerializeField] private AudioSource sound;

    /// <summary>
    /// Reset the randomness and start again. You usually don't need to call
    /// this, deactivating/reactivating is usually fine but if you want a strict
    /// restart you can do.
    /// </summary>
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
            // pop off an item if too big
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
            sound.Play();
        }
    }

}