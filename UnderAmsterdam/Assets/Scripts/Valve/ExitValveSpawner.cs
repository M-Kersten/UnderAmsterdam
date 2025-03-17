using DG.Tweening;
using Fusion.XR.Host.Rig;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitValveSpawner : MonoBehaviour
{
    [SerializeField] private Transform mainCam;
    [SerializeField] private GameObject ExitValvePrefab, prefabSpawnPos;
    [SerializeField] private float buttonActivationTime = 2;
    [SerializeField] private HardwareHand _leftHand;
    
    private PlayerInputHandler playerInputHandler;
    private bool isSpawned = false;
    private float timeRemaining;
    private float movementDuration = 2;
    private ExitValve _activeExtiValve;
    
    private void Start()
    {
        playerInputHandler = GetComponent<PlayerInputHandler>();
        timeRemaining = buttonActivationTime;
    }
    
    private void FixedUpdate() 
    {
        if (!isSpawned && SceneManager.GetActiveScene().buildIndex > 0 && playerInputHandler.isMenuPressed)
        {
            // add haptics to left joystick
            //The user should keep the button pressed to spawn the exit valve
            if (timeRemaining > 0)
            {
                _leftHand.SendHapticImpulse(.4f, .01f);
                timeRemaining -= Time.deltaTime;
            }
            else if (timeRemaining < 0)
            {
                _leftHand.SendHapticImpulse(1, .1f);
                SpawnPipe();
            }
        }
    }
    
    private void SpawnPipe()
    {
        isSpawned = true;

        _activeExtiValve = Instantiate(ExitValvePrefab, prefabSpawnPos.transform.position, Quaternion.identity).GetComponent<ExitValve>();
        _activeExtiValve.spawnerRef = this;

        var lookatPosition = new Vector3(mainCam.position.x, _activeExtiValve.transform.position.y, mainCam.position.z);
        _activeExtiValve.transform.LookAt(lookatPosition);
        
        _activeExtiValve.transform.DOMoveY(mainCam.position.y, movementDuration).SetEase(Ease.InOutQuad);
    }

    [ContextMenu("Test exit valve")]
    public void TestExitValve()
    {
        SpawnPipe();
        DOVirtual.DelayedCall(3, () =>
        {
            _activeExtiValve.StartReturnMenu();
        });
    }
    
    public void DespawnPipe(GameObject pipe)
    {
        isSpawned = false;
        timeRemaining = buttonActivationTime;
        Destroy(pipe, movementDuration);
    }
    
    
}
