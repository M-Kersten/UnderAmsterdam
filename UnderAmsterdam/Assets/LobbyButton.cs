using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LobbyButton : MonoBehaviour
{
    [SerializeField] private GameObject button;
    [SerializeField] private AudioSource sound;
    [SerializeField] private Material redMaterial, greenMaterial;
    public UnityEvent onReady, onNotReady;
    private Renderer buttonColor;
    GameObject presser;
    bool isReady;

    void Start()
    {
        sound = GetComponent<AudioSource>();
        buttonColor = button.GetComponent<Renderer>();
        isReady = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isReady || isReady && other.gameObject == presser)
        {
            isReady = !isReady;
            if (isReady)
            {
                button.transform.localPosition = new Vector3(0, 0.003f, 0);
                onReady.Invoke();
                buttonColor.material = greenMaterial;
            }
            presser = other.gameObject;
            
            sound.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == presser)
        {
            if (!isReady)
            {
                button.transform.localPosition = new Vector3(0, 0.015f, 0);
                onNotReady.Invoke();
                buttonColor.material = redMaterial;
            }
        }
    }
}
