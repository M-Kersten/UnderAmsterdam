/**************************************************
Copyright : Copyright (c) RealaryVR. All rights reserved.
Description: Script for VR Button functionality.

Slightly altered to only interact with local player
***************************************************/

using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SettingsButton : MonoBehaviour
{
    public GameObject button;
    public UnityEvent onPress;
    public UnityEvent onRelease;
    GameObject presser;
    AudioSource sound;
    bool isPressed;

    void Start()
    {
        sound = GetComponent<AudioSource>();
        isPressed = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.GetComponent<NetworkObject>() != null)
        {
            if (!other.transform.root.GetComponent<NetworkObject>().HasInputAuthority)
                return;
        }

        if (other.gameObject.layer == 8 && !isPressed && other.CompareTag("UI"))
        {
            button.transform.localPosition = new Vector3(0, 0.003f, 0);
            presser = other.gameObject;
            onPress.Invoke();
            sound.Play();
            isPressed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.GetComponent<NetworkObject>() != null)
        {
            if (!other.transform.root.GetComponent<NetworkObject>().HasInputAuthority)
                return;
        }

        if (other.gameObject.layer == 8 && other.gameObject == presser && other.CompareTag("UI"))
        {
            button.transform.localPosition = new Vector3(0, 0.015f, 0);
            onRelease.Invoke();
            isPressed = false;
        }
    }
}
