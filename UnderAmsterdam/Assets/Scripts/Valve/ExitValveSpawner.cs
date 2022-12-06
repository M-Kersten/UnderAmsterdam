using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;

public class ExitValveSpawner : MonoBehaviour
{
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private GameObject Child;
    [SerializeField] private ExitValve InteractionValveScript;

    private bool isSpawned = false;

    private float timeRemaining = 2.75f;

    private void Start()
    {
        if (!playerInputHandler)
            playerInputHandler = GetComponentInParent<PlayerInputHandler>();
        if (!Child)
            Child = this.transform.GetChild(0).gameObject;
    }

    private void FixedUpdate() 
    { 
        if (isSpawned)
            return;
        
        //The user should keep the button pressed to spawn the exit valve
        if (playerInputHandler.isMenuPressed && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
        }
        else if(playerInputHandler.isMenuPressed && timeRemaining < 0)
        {
            Spawner();
        }
    }
    
    private void Spawner() 
    {
        isSpawned = true;
        Child.SetActive(true);
        InteractionValveScript.enabled = true;
        
        Vector3 position = transform.position;
        
        position = Vector3.Slerp(position, new Vector3(position.x, 2f, position.z), Time.deltaTime);
        transform.position = position;
    }
}
