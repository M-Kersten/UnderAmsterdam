using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Host;
using TMPro;
using System;

public class Joinlobby : MonoBehaviour
{
    [SerializeField] private Animator lPlayerAnimator;
    [SerializeField] TextMeshProUGUI textinput;
    private bool canPressButton = true;
    [SerializeField] private float buttonCooldownSeconds = 8;
    public async void OnAutoHostJoin()
    {
        ConnectionManager connection = ConnectionManager.Instance;
        if (canPressButton)
        {
            connection.roomName = textinput.text;
            connection.mode = Fusion.GameMode.AutoHostOrClient;

            canPressButton = false;
            StartCoroutine(ButtonCooldown());

            Gamemanager.Instance.localRigid.GetComponent<Animator>().Play("VisionFadeLocal", 0);

            await connection.Connect();

            if (connection.runner.IsServer)
                connection.runner.SetActiveScene(2);
        }
    }
    private IEnumerator ButtonCooldown()
    {
        yield return new WaitForSeconds(buttonCooldownSeconds);
        canPressButton = true;
    }
}
