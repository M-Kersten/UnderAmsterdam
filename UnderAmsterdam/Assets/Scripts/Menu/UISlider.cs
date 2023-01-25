using Fusion.XR.Host.Rig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class UISlider : MonoBehaviour
{
    [SerializeField] public GameObject handle;
    [SerializeField] GameObject backgroundBase;
    [SerializeField] Image fillBase;
    [SerializeField] float minXPos = -0.4832999f;
    private Vector3 minPosition;
    private Vector3 maxPosition;
    private float minValueForMixer = 0.0001f;
    private bool touched;

    private Vector3 handInLocalSpace;
    private float handMovedSinceTouch;
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
        HandlePosition(handle.transform.localPosition.x);
    }

    private void Update()
    {
        if(touched && touchingCollider != null)
        {
            HandlePosition(handle.transform.localPosition.x);

            float newPosition = handle.transform.parent.InverseTransformPoint(touchingCollider.transform.position).x;

            handMovedSinceTouch = newPosition - handInLocalSpace.x;
            float newX = Mathf.Clamp(handInLocalSpace.x + handMovedSinceTouch, minPosition.x, maxPosition.x);

            handle.transform.localPosition = new Vector3(newX, handle.transform.localPosition.y, handle.transform.localPosition.z);
        }
    }
    public void HandlePosition(float localPositionX)
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
        if (other.transform.root.GetComponent<NetworkObject>() != null)
        {
            if (!other.transform.root.GetComponent<NetworkObject>().HasInputAuthority)
                return;
        }

        if (other.gameObject.layer == 8 && other.CompareTag("UI"))
        {
            touched = true;
            touchingCollider = other;
            handInLocalSpace = handle.transform.parent.InverseTransformPoint(touchingCollider.transform.position);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.GetComponent<NetworkObject>() != null)
        {
            if (!other.transform.root.GetComponent<NetworkObject>().HasInputAuthority)
                return;
        }

        if (other.gameObject.layer == 8 && other.CompareTag("UI"))
        {
            touched = false;
            touchingCollider = null;
            HandlePosition(handle.transform.localPosition.x);
        }
    }
}
