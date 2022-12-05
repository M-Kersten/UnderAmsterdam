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
    private bool isCoroutineStarted = false;

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
        if (!isCoroutineStarted && playerInputHandler.isMenuPressed)
        {
            StartCoroutine(PressingTimer());
        }
        else if(!playerInputHandler.isMenuPressed && isCoroutineStarted)
        {
            StopCoroutine(PressingTimer());
            isCoroutineStarted = false;
        }
    }

    private IEnumerator PressingTimer()
    {
        isCoroutineStarted = true;
        while (playerInputHandler.isMenuPressed)
        {
            yield return new WaitForSecondsRealtime(2.75f);
            Spawner();
        }
        yield return new WaitForFixedUpdate();
        isCoroutineStarted = false;
    }

    private void Spawner() 
    {
        StopCoroutine(PressingTimer());
        isCoroutineStarted = false;

        isSpawned = true;
        Child.SetActive(true);
        InteractionValveScript.enabled = true;
        //transform.position = transform.position.Lerp(transform.position, new Vector3(transform.position.x, 2f, transform.position.z), Time.deltaTime);

    }
}
