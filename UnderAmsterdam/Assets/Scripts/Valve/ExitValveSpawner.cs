using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;

public class ExitValveSpawner : MonoBehaviour
{
    [SerializeField] private Transform mainCam;
    [SerializeField] private GameObject ExitValvePrefab, prefabSpawnPos;
    [SerializeField] private float buttonActivationTime = 2;
    private PlayerInputHandler playerInputHandler;

    private bool isSpawned = false;
    private float timeRemaining = 2;
    private float movementDuration = 2;
    private void Start()
    {
        playerInputHandler = GetComponent<PlayerInputHandler>();
    }
    private void FixedUpdate() 
    {
        if (!isSpawned && SceneManager.GetActiveScene().name != "A1Menu" && playerInputHandler.isMenuPressed)
        {
            //The user should keep the button pressed to spawn the exit valve
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else if (timeRemaining < 0)
            {
                SpawnPipe();
            }
        }
    }
    
    private IEnumerator MovePipe(GameObject pipe, Vector3 targetPos, Vector3 pos)
    {
        float elapsed = 0;
        while (elapsed < movementDuration)
        {
                elapsed += Time.deltaTime;
                pipe.transform.position = Vector3.Lerp(pos, targetPos, elapsed / movementDuration);
                yield return null;
        }
        pipe.transform.position = targetPos;

        yield return true;
    }
    private void SpawnPipe()
    {
        isSpawned = true;

        GameObject pipe = Instantiate(ExitValvePrefab, prefabSpawnPos.transform.position, Quaternion.identity);
        pipe.GetComponent<ExitValve>().spawnerRef = this;
        StartCoroutine(MovePipe(pipe, new Vector3(pipe.transform.position.x, mainCam.position.y, pipe.transform.position.z), pipe.transform.position));
    }
    public void DespawnPipe(GameObject pipe)
    {
        isSpawned = false;
        timeRemaining = buttonActivationTime;
        Destroy(pipe, movementDuration);
    }
}
