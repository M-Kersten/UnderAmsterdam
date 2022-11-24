using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WristMenu : MonoBehaviour
{
    [Tooltip("Add company icons here")]
    [SerializeField]
    private Sprite[] companyIcons;
    [SerializeField]
    GameObject parentObject;
    [SerializeField]
    GameObject visualRadialObject;
    [SerializeField]
    private float maxActiveAngle, minActiveAngle, maxActiveAnglex, minActiveAnglex;
    [SerializeField]
    private GameObject iconImage;
    [SerializeField]
    TextMeshProUGUI pointsText;
    [SerializeField] public GameObject topWatch;

    private PlayerData myData;

    // Start is called before the first frame update
    void Start()
    {
        // Grab the parent of this parent
        visualRadialObject = transform.GetChild(0).gameObject;
        visualRadialObject.SetActive(true);
        myData = Gamemanager.Instance.localPlayerData;
    }

    // Update is called once per frame
    //void FixedUpdate()
    //{
    //    // if the angle of the wrist is in between these numbers, show or don't show the menu
    //    if (parentObject.transform.localEulerAngles.z < maxActiveAngle && parentObject.transform.localEulerAngles.z > minActiveAngle && parentObject.transform.localEulerAngles.x < maxActiveAnglex && parentObject.transform.localEulerAngles.x > minActiveAnglex)
    //        visualRadialObject.SetActive(true);
    //    else
    //        visualRadialObject.SetActive(false);
    //}

    void Update()
    {
        // Need a way to grab PlayerData from NetworkRig
        if (myData != null)
            pointsText.text = myData.points.ToString();
    }

    public void ChangeImage(string company) {

        for (int i = 0; i < companyIcons.Length; i++)
        {
            // change image at the top of the wrist watch to the icon in the list
            if (companyIcons[i].name == company)
                iconImage.GetComponent<Image>().sprite = companyIcons[i];
        }
    }
}
