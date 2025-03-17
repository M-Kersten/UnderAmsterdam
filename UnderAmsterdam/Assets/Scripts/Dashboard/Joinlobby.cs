using System.Collections;
using UnityEngine;
using TMPro;

public class Joinlobby : MonoBehaviour
{
    [SerializeField] private Animator lPlayerAnimator;
    [SerializeField] TextMeshProUGUI textinput;
    [SerializeField] private float buttonCooldownSeconds = 8;
    [SerializeField] private string loadSceneName = "MainScene";

    private bool canPressButton = true;

    [ContextMenu("Host join")]
    public async void OnAutoHostJoin()
    {
        if (canPressButton)
        {
            Gamemanager.Instance.ConnectionManager.gameMode = Fusion.GameMode.AutoHostOrClient;

            canPressButton = false;
            StartCoroutine(ButtonCooldown());

            Gamemanager.Instance.localRigid.GetComponent<Animator>().Play("VisionFadeLocal", 0);

            await Gamemanager.Instance.ConnectionManager.Connect();

            if (Gamemanager.Instance.ConnectionManager.runner.IsServer)
                Gamemanager.Instance.ConnectionManager.runner.LoadScene(loadSceneName);
        }
    }

    private IEnumerator ButtonCooldown()
    {
        yield return new WaitForSeconds(buttonCooldownSeconds);
        canPressButton = true;
    }
}