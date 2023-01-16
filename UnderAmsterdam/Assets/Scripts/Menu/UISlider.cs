using Fusion.XR.Host.Rig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class UISlider : NetworkBehaviour
{
    [SerializeField] public GameObject handle;
    [SerializeField] GameObject backgroundBase;
    [SerializeField] Image fillBase;
    [SerializeField] float minXPos = -0.4832999f;
    private Vector3 minPosition;
    private Vector3 maxPosition;
    private float minValueForMixer = 0.0001f;
    private bool touched;

    private Collider touchingCollider;

    [SerializeField] private SettingsUI volumeMixer;
    [Tooltip("Find all volumeType's in the SettingsUI audiomixer in Sounds folder")]
    [SerializeField] private string volumeType;
    [Tooltip("Is this slider used for volume?")]
    [SerializeField] private bool volumeSlider = true;
    
    // Start is called before the first frame update
    void Start()
    {
        minPosition = new Vector3(minXPos, 0.961f, 0);
        maxPosition = new Vector3(-minXPos, 0.961f, 0);
        handle.transform.localPosition = maxPosition;
        handlePosition(handle.transform.localPosition.x);
    }

    private void Update()
    {
        if(touched && touchingCollider != null)
        {
            handlePosition(handle.transform.localPosition.x);

            if (touchingCollider.transform.position.x > handle.transform.position.x)
                handle.transform.localPosition += new Vector3(0.01f, 0, 0);
            else
                handle.transform.localPosition -= new Vector3(0.01f, 0, 0);

            if (handle.transform.localPosition.x < minPosition.x)
            {
                handle.transform.localPosition = minPosition;
            }
            if (handle.transform.localPosition.x > maxPosition.x)
            {
                handle.transform.localPosition = maxPosition;
            }
        }
    }
    public void handlePosition(float localPositionX)
    {
        float volume = minValueForMixer + Mathf.InverseLerp(minPosition.x, maxPosition.x, localPositionX);
        fillBase.fillAmount = volume;
        if (volumeSlider && volumeType != "")
        volumeMixer.SetVolume(volumeType, volume);
        if (volumeType == "Master")
            volumeMixer.MasterVolume();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.TryGetComponent<NetworkObject>(out NetworkObject component))
        {

            if (!component.InputAuthority)
            return;
        }

        if (other.gameObject.layer == 8 && other.CompareTag("UI"))
        {
            touched = true;
            touchingCollider = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.TryGetComponent<NetworkObject>(out NetworkObject component))
        {
            if (!component.InputAuthority)
                return;
        }

        if (other.gameObject.layer == 8 && other.CompareTag("UI"))
        {
            touched = false;
            touchingCollider = null;
        }
    }
}
