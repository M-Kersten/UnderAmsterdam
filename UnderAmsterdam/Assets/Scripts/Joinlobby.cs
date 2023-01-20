using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Host;
using TMPro;
using System;

public class Joinlobby : MonoBehaviour
{
    [SerializeField] private Animator lPlayerAnimator;
    [SerializeField] ConnectionManager connection;
    [SerializeField] TextMeshProUGUI textinput;
    private bool canPressButton = true;

    public async void OnAutoHostJoin()
    {
        if (canPressButton)
        {
            connection.roomName = textinput.text;
            connection.mode = Fusion.GameMode.AutoHostOrClient;

            canPressButton = false;
            StartCoroutine(ButtonCooldown());

            await connection.Connect();

            if (connection.runner.IsServer)
            {
                Gamemanager.Instance.localRigid.GetComponent<Animator>().Play("VisionFadeLocal", 0);
                connection.runner.SetActiveScene(2);
            }
        }
    }
    private IEnumerator ButtonCooldown()
    {
        yield return new WaitForSeconds(1);
        canPressButton = true;
    }
}
