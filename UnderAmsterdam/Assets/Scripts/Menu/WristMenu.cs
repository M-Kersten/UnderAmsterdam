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
    [SerializeField] private PlayerData myData;
    [SerializeField] private Transform leftWatch, rightWatch;
    private int startingPoints = 1000;

    public GameObject topWatch;

    void Update()
    {
        // Need a way to grab PlayerData from NetworkRig
        if (myData != null && pointsText.text != myData.points.ToString())
        {
            int addedPoints = myData.points - int.Parse(pointsText.text);
            if (addedPoints != startingPoints) winLosePoints(addedPoints);
            pointsText.text = myData.points.ToString();
        }
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
        Transform receptionHand = rightHand.isRightHanded ? leftWatch : rightWatch;
        if (points > 0) Instantiate(goldParticles, receptionHand).transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "+" + points.ToString();
        else Instantiate(lossParticles, receptionHand).transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = points.ToString();
    }
}
