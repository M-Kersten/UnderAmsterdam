using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitValveSpawner : MonoBehaviour
{
    [SerializeField] private Transform mainCam;
    [SerializeField] private GameObject ExitValvePrefab, prefabSpawnPos;
    [SerializeField] private float buttonActivationTime = 2;
    
    private PlayerInputHandler playerInputHandler;
    private bool isSpawned = false;
    private float timeRemaining;
    private float movementDuration = 2;
    
    private void Start()
    {
        playerInputHandler = GetComponent<PlayerInputHandler>();
        timeRemaining = buttonActivationTime;
    }
    
    private void FixedUpdate() 
    {
        if (!isSpawned && SceneManager.GetActiveScene().buildIndex > 0 && playerInputHandler.isMenuPressed)
        {
            //The user should keep the button pressed to spawn the exit valve
            if (timeRemaining > 0)
                timeRemaining -= Time.deltaTime;
            else if (timeRemaining < 0)
                SpawnPipe();
        }
    }
    
    private void SpawnPipe()
    {
        isSpawned = true;

        var pipe = Instantiate(ExitValvePrefab, prefabSpawnPos.transform.position, Quaternion.identity);
        pipe.GetComponent<ExitValve>().spawnerRef = this;

        var spawnPosition = new Vector3(pipe.transform.position.x, mainCam.position.y, pipe.transform.position.z);
        pipe.transform.DOMove(spawnPosition, movementDuration).SetEase(Ease.InOutQuad);
    }
    
    public void DespawnPipe(GameObject pipe)
    {
        isSpawned = false;
        timeRemaining = buttonActivationTime;
        Destroy(pipe, movementDuration);
    }
}
