using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShooter : MonoBehaviour
{
    [SerializeField] private Camera myCamera;
    [SerializeField] private Material myMaterial;
    [SerializeField] private GameObject myQuad;

    private void Start()
    {
        myQuad.SetActive(false);
        myCamera.enabled = false;
        Gamemanager.Instance.GameEnd.AddListener(TakeScreenshot);
    }

    void TakeScreenshot()
    {
        myQuad.SetActive(true);
        StartCoroutine(RenderScreenshot());
    }

    IEnumerator RenderScreenshot()
    {
        yield return new WaitForEndOfFrame();

        RenderTexture screenTexture = new RenderTexture(550, 515, 16);
        myCamera.targetTexture = screenTexture;
        RenderTexture.active = screenTexture;
        myCamera.Render();
        myMaterial.mainTexture = screenTexture;
    }
}
