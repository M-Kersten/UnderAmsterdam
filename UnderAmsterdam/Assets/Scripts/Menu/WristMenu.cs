using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class WristMenu : NetworkBehaviour
{
    [Tooltip("Add company icons here")]
    [SerializeField] private Sprite[] companyIcons;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private GameObject iconImage;

    private PlayerData myData;

    public GameObject topWatch;

    // Start is called before the first frame update
    void Start()
    {
        myData = Gamemanager.Instance.localPlayerData;
    }

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
