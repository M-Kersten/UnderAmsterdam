using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WristMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject parentObject;
    [SerializeField]
    private GameObject visualRadialObject;
    [SerializeField]
    private float maxActiveAngle, minActiveAngle;

    // Start is called before the first frame update
    void Start()
    {
        parentObject = transform.parent.gameObject;
        visualRadialObject = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (parentObject.transform.localEulerAngles.z < maxActiveAngle && parentObject.transform.localEulerAngles.z > minActiveAngle)
            visualRadialObject.SetActive(true);
        else
            visualRadialObject.SetActive(false);
    }
}
