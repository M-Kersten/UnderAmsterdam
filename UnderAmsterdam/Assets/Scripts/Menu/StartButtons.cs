using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Host;

public class StartButtons : MonoBehaviour
{
    private int totalPressed;
    [SerializeField] private int sceneIndex = 1;
    [SerializeField] GameObject stad; 
   bool moveStreet;
   float speed = 3; 

    public void ButtonStatus(bool pressed) {
        if (pressed)
            totalPressed++;
        else 
            totalPressed--;

        if (totalPressed == ConnectionManager.Instance._spawnedUsers.Count) {
            moveStreet = true;
            
        }

        Debug.Log("TotalPressed: " + totalPressed);
    }
    
    public void DevStart() {
        Gamemanager.Instance.SceneSwitch(sceneIndex);
    }
    void Update()
    {
        if (stad.transform.position.y < 1.51f && moveStreet)
        {
            stad.transform.Translate(Vector3.up * speed * Time.deltaTime);
        }
        if (stad.transform.position.y == 1.51f && moveStreet)
        {
            Gamemanager.Instance.SceneSwitch(sceneIndex);
        }
        //  if else transform.Translate(0);
    }
}
