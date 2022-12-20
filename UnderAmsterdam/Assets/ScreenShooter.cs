using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShooter : MonoBehaviour
{
    [SerializeField] private Camera myCamera;

    private void Start()
    {
        Gamemanager.Instance.GameEnd.AddListener(TakeScreenshot);
    }

    void TakeScreenshot()
    {
        myCamera.targetTexture = RenderTexture.GetTemporary(550, 550, 16);
        Texture2D result = new Texture2D(myCamera.targetTexture.width, myCamera.targetTexture.height, TextureFormat.ARGB32, false);
        Rect rect = new Rect(275, 20, myCamera.targetTexture.width, myCamera.targetTexture.height);
        result.ReadPixels(rect, 0, 0);
        byte[] byteArray = result.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/Screenshot.png", byteArray);
        Debug.Log("Screenshot taken");
        //RenderTexture.ReleaseTemporary(myCamera.targetTexture);
    }

    private void Update()
    {
        if (Input.GetKeyDown("space")) TakeScreenshot();
    }
}
