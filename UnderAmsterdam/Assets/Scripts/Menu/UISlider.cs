using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISlider : MonoBehaviour
{
    [SerializeField] GameObject handle;
    [SerializeField] GameObject backgroundBase;
    [SerializeField] Image fillBase;
    [SerializeField] float minXPos = -0.4832999f;
    private Vector3 minPosition;
    private Vector3 maxPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        minPosition = new Vector3(minXPos, 0.961f, 0);
        maxPosition = new Vector3(-minXPos, 0.961f, 0);
    }

    private void Update()
    {
        fillBase.fillAmount = handlePosition();
        if (handle.transform.localPosition.x < minPosition.x)
        {
            handle.transform.localPosition = minPosition;
        }
        if (handle.transform.localPosition.x > maxPosition.x)
        {
            handle.transform.localPosition = maxPosition;
        }
    }
    private float handlePosition()
    {
        return Mathf.InverseLerp(minPosition.x, maxPosition.x, handle.transform.localPosition.x);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ONTRIGGER: " + other + " TAG: " + other.tag + " WHO: " + other.name);
        if (other.gameObject.layer == 8)
        {
            handle.transform.position = new Vector3(other.transform.position.x, handle.transform.position.y, handle.transform.position.z);
        }
    }
}
