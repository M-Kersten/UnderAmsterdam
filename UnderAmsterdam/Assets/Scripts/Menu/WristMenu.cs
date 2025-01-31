using System;
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
            var addedPoints = myData.points - int.Parse(pointsText.text);
            if (addedPoints != startingPoints) WinLosePoints(addedPoints);
                pointsText.text = myData.points.ToString();
        }
    }

    public void ChangeImage(int company)
    {
        foreach (var sprite in companyIcons)
        {
            // change image at the top of the wrist watch to the icon in the list
            if (sprite.name == Enum.GetValues(typeof(CompanyType)).GetValue(company).ToString())
                iconImage.GetComponent<Image>().sprite = sprite;
        }
    }

    public void WinLosePoints(int points)
    {
        var receptionHand = rightHand.isRightHanded ? leftWatch : rightWatch;
        if (points > 0) Instantiate(goldParticles, receptionHand).transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "+" + points.ToString();
        else Instantiate(lossParticles, receptionHand).transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = points.ToString();
    }
}
