using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShooter : MonoBehaviour
{
    [SerializeField] private Camera myCamera;
    [SerializeField] private Material myMaterial;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        myCamera.enabled = false;
        Gamemanager.Instance.GameEnd.AddListener(TakeScreenshot);
    }

    void TakeScreenshot()
    {
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

    private void Update()
    {
        if (Input.GetKeyDown("space")) StartCoroutine(RenderScreenshot());
    }
}
