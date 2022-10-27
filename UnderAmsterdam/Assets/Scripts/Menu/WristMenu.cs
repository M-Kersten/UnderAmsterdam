using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WristMenu : MonoBehaviour
{
    [Tooltip("Add company icons here")]
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
        // Grab the parent of this parent
        parentObject = transform.parent.transform.parent.gameObject;
        visualRadialObject = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // if the angle of the wrist is in between these numbers, show or don't show the menu
        if (parentObject.transform.localEulerAngles.z < maxActiveAngle && parentObject.transform.localEulerAngles.z > minActiveAngle && parentObject.transform.localEulerAngles.x < maxActiveAnglex && parentObject.transform.localEulerAngles.x > minActiveAnglex)
            visualRadialObject.SetActive(true);
        else
            visualRadialObject.SetActive(false);
    }

    void ChangeImage(int companyId) {
        // if we have an image for this company then show it
        if (companyIcons[companyId] != null)
            // change image at the top of the wrist watch to the icon in the list
            iconImage.GetComponent<Image>().sprite = companyIcons[companyId];
        else 
            Debug.Log("No image for this company in the list");
    }
}
