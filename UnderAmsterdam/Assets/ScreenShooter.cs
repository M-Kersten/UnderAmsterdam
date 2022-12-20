using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShooter : MonoBehaviour
{
    private void Start()
    {
        Gamemanager.Instance.GameEnd.AddListener(TakeScreenshot);
    }

    void TakeScreenshot()
    {
        ScreenCapture.CaptureScreenshot("screenshot.png");
        Debug.Log("Screenshot taken");
    }

    private void Update()
    {
        if (Input.GetKeyDown("space")) TakeScreenshot();
    }
}
