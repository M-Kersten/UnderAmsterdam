using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pressbutton : MonoBehaviour
{
    public Camera mainKamera;
    private RaycastHit raycastHit;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainKamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit))
            {
                if (raycastHit.collider.gameObject.name == "Host Button")//this string must be name of your button I named my button myButton
                {
                    Debug.Log("clicked");
                }
            }
        }

    }
}
