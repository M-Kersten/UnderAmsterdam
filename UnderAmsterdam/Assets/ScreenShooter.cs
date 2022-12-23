using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShooter : MonoBehaviour
{
    [SerializeField] private Camera myCamera;
    [SerializeField] private Material myMaterial;

    private void Start()
    {
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

        RenderTexture screenTexture = new RenderTexture(Screen.width, Screen.height, 16);
        myCamera.targetTexture = screenTexture;
        RenderTexture.active = screenTexture;
        myCamera.Render();
        Texture2D renderedTexture = new Texture2D(550, 515);
        renderedTexture.ReadPixels(new Rect(250, 0, 550, 515), 0, 0);
        RenderTexture.active = null;
        myMaterial.SetTexture("_MainTex", (Texture)renderedTexture);
        /*
        byte[] byteArray = renderedTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/Textures/TopDownCapture.png", byteArray);
        Debug.Log("Screenshot taken");*/
    }

    private void Update()
    {
        if (Input.GetKeyDown("space")) StartCoroutine(RenderScreenshot());
    }
}
