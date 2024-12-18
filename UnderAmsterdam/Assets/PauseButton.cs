using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine;

public class PauseButton : MonoBehaviour
{
    [SerializeField] private GameObject button;
    [SerializeField] private VideoPlayer myMonitor;
    [SerializeField] private AudioSource sound;
    [SerializeField] private Image myVignette;
    [SerializeField] private Sprite sPlay,sPause;
    [SerializeField] private float waitForSeconds;

    GameObject presser;
    bool isPressed;
    private bool cooldown;

    void Start()
    {
        isPressed = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPressed && !cooldown)
        {
            button.transform.localPosition = new Vector3(0, 0.003f, 0);
            sound.Play();

            if (myMonitor.isPlaying)
            {
                myMonitor.Pause();
                myVignette.sprite = sPlay;
            }
            else
            {
                myMonitor.Play();
                myVignette.sprite = sPause;
            }

            presser = other.gameObject;
            isPressed = true;
            cooldown = true;
            StartCoroutine(TimerCooldown());
        }
    }
    private IEnumerator TimerCooldown()
    {
        yield return new WaitForSeconds(waitForSeconds);
        cooldown = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == presser)
        {
            button.transform.localPosition = new Vector3(0, 0.015f, 0);
            isPressed = false;
        }
    }
}
