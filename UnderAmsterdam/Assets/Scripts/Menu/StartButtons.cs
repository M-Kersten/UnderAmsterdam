using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButtons : MonoBehaviour
{
    private int totalPressed;
    [SerializeField] private int sceneIndex = 1;

    public void ButtonStatus(bool pressed) {
        if (pressed)
            totalPressed++;
        else 
            totalPressed--;

        if (totalPressed == Gamemanager.Instance.cManager._spawnedUsers.Count)
            Gamemanager.Instance.SceneSwitch(sceneIndex);

        Debug.Log("TotalPressed: " + totalPressed);
    }
    
    public void DevStart() {
        Gamemanager.Instance.SceneSwitch(sceneIndex);
    }
}
