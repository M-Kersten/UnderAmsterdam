using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WristMenu : MonoBehaviour
{
    [SerializeField]
    private Sprite[] companyIcons;
    [SerializeField]
    private GameObject parentObject;
    [SerializeField]
    private GameObject visualRadialObject;
    [SerializeField]
    private float maxActiveAngle, minActiveAngle, maxActiveAnglex, minActiveAnglex;
    [SerializeField]
    private GameObject iconImage;

    // Start is called before the first frame update
    void Start()
    {
        ChangeImage(0);

        parentObject = transform.parent.gameObject;
        visualRadialObject = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (parentObject.transform.localEulerAngles.z < maxActiveAngle && parentObject.transform.localEulerAngles.z > minActiveAngle && parentObject.transform.localEulerAngles.x < maxActiveAnglex && parentObject.transform.localEulerAngles.x > minActiveAnglex)
            visualRadialObject.SetActive(true);
        else
            visualRadialObject.SetActive(false);
    }

    void ChangeImage(int companyId) {
        if (companyId <= companyIcons.Length)
            iconImage.GetComponent<Image>().sprite = companyIcons[companyId];
        else 
            Debug.Log("Not that many icons in our array");
    }
}
