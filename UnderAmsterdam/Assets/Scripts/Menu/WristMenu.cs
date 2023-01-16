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
    [SerializeField] private GameObject goldParticles, lossParticles;
    [SerializeField] private HandTileInteraction rightHand;

    private PlayerData myData;

    public GameObject topWatch;

    // Start is called before the first frame update
    void Start()
    {
        myData = GetComponent<PlayerData>();
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

    public void winLosePoints(int points)
    {
        Transform receptionHand = rightHand.isRightHanded ? myData.localLeftHand : myData.localRightHand;
        if (points > 0) Instantiate(goldParticles, receptionHand).transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "+" + points.ToString();
        else Instantiate(lossParticles, receptionHand).transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = points.ToString();
    }
}
