using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;

public class ExitValveSpawner : MonoBehaviour
{
    [SerializeField] private Transform mainCam;
    [SerializeField] private GameObject ExitValvePrefab;
    private PlayerInputHandler playerInputHandler;

    private bool isSpawned = false;
    private float timeRemaining = 2.75f;
    private void Start()
    {
        playerInputHandler = GetComponent<PlayerInputHandler>();
    }
    private void FixedUpdate() 
    { 
        if (isSpawned) // and if scene is 0
            return;

        //The user should keep the button pressed to spawn the exit valve
        if (playerInputHandler.isMenuPressed && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
        }
        else if(playerInputHandler.isMenuPressed && timeRemaining < 0)
        {
             StartCoroutine(MovePipe());
        }
    }
    
    private IEnumerator MovePipe()
    {
        isSpawned = true;

        Quaternion headQ = Quaternion.Euler(0, mainCam.eulerAngles.y, 0);
        Vector3 forwardPos = headQ * transform.forward + transform.position;

        GameObject pipe = Instantiate(ExitValvePrefab, forwardPos, Quaternion.identity);

        //Move raise code to coroutine
        Vector3 position = pipe.transform.position;
        Vector3 targetPos = new Vector3(position.x, 1f, position.z);

        float elapsed = 0;
        float duration = 2f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            pipe.transform.position = Vector3.Lerp(position, targetPos, elapsed / duration);
            yield return null;
        }
        ExitValvePrefab.transform.position = position;

        yield return null;
    }
}
