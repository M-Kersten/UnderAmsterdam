using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Fusion;

public class CityMove : NetworkBehaviour
{
    [SerializeField] GameObject[] toDisableObjects, toEnableObjects;
    [SerializeField] Material newMaterial;
    [SerializeField] ScoreBoard scoreBoard;

    [SerializeField] private float moveToGameDuration = 5f;
    [SerializeField] private float moveDownY = -0.5f;
    [SerializeField] private GameObject grabText;
    [SerializeField] private ParticleSystem helmetParticles;
    [SerializeField] private GameObject _ground;
    [SerializeField] private ParticleSystem[] _groundParticles;
    
    private List<PlayerRef> readyList= new();
    Vector3 movedown;
    private Vector3 playerPos;
    private Vector3 moveup;
    private bool endOfGame = false;
    private Vector3 _groundOriginalPosition;
    
    void Start()
    {
        Gamemanager.Instance.GameEnd.AddListener(EndOfGame);
        _groundOriginalPosition = _ground.transform.position;
    }

    public static NetworkObject GetLocalPlayer()
    {
        foreach (var networkObject in FindObjectsOfType<NetworkObject>())
        {
            if (networkObject.HasInputAuthority)
                return networkObject;
           
        }
        return null;
    }

    [ContextMenu("Grab helmet")]
    public void TestGrabHelmet()
    {
        var localPlayer = GetLocalPlayer();
        
        if (localPlayer != null)
            GrabHelmet(localPlayer.gameObject);
    }
  
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
            GrabHelmet(other.gameObject);
    }

    private void GrabHelmet(GameObject other)
    {
        var otherObj = other.GetComponentInParent<NetworkObject>();
        var tempPlayer = otherObj.InputAuthority;
        var playerData = other.transform.root.GetComponent<PlayerData>();
            
        if(grabText.activeSelf && otherObj.HasInputAuthority)
        {
            helmetParticles.Play();
            grabText.SetActive(false);
        }
            
        // If I am host
        if (HasStateAuthority && !readyList.Contains(tempPlayer))
        {
            readyList.Add(tempPlayer);
            CheckAllPlayers();

            // Always enable the cap when a player steps into the ready box first time.
            if (!playerData.playerCap.activeSelf)
                RPC_EnableCap(playerData);   
        }
    }

    private void CheckAllPlayers()
    {
        int readyPlayers = 0;
        
        for(int i = 0; i < readyList.Count; i++)
        {
            if (Gamemanager.Instance.ConnectionManager.SpawnedUsers.ContainsKey(readyList[i]))
                readyPlayers++;
            else
                readyList.Remove(readyList[i]);
        }

        if (Gamemanager.Instance.ConnectionManager.SpawnedUsers.Count == readyPlayers && readyPlayers > 0)
        {
            RPC_StartGame();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_StartGame()
    {
        playerPos = Gamemanager.Instance.localRigid.gameObject.transform.position;
        moveup = playerPos;
        movedown = new Vector3(playerPos.x, moveDownY, playerPos.z);
        MovePlayers(playerPos, movedown);
        
        foreach (var particleSystem in _groundParticles)
            particleSystem.Play();

        DOTween.Sequence()
            .Append(_ground.transform.DOShakePosition(1.5f, new Vector3(.1f,0.01f,.1f), 40, randomnessMode: ShakeRandomnessMode.Harmonic).SetEase(Ease.InOutSine))
            .Join(_ground.transform.DOMoveY(moveDownY, moveToGameDuration).SetDelay(1).SetEase(Ease.InSine));
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_EnableCap(PlayerData pData)
    {
        pData.playerCap.SetActive(true);
    }

    private void EndOfGame()
    {
        playerPos = Gamemanager.Instance.localRigid.transform.position;
        moveup = new Vector3(playerPos.x, moveup.y, playerPos.z);
        endOfGame = true;
        EnableObjectsBeforeGameOver();
        MovePlayers(playerPos, moveup);
        
        _ground.GetComponent<Renderer>().material = newMaterial;
        _ground.transform.DOMoveY(_groundOriginalPosition.y, moveToGameDuration)
            .SetDelay(1)
            .SetEase(Ease.InSine);
    }
    
    private void MovePlayers(Vector3 from, Vector3 to)
    {
        if (!endOfGame)
        {
            toDisableObjects[0].SetActive(false);
            toDisableObjects[1].SetActive(false);
        }
        
        DOVirtual.Vector3(from, to, moveToGameDuration, value =>
        {
            Gamemanager.Instance.localRigid.gameObject.transform.position = value;
        }).SetEase(Ease.InSine)
            .SetDelay(1f)
            .OnComplete(() =>
        {
            if (!endOfGame)
            {
                Gamemanager.Instance.OnGameStart();
                DisableObjectsAfterGameStart();
            }
            
            if (endOfGame)
            {
                Gamemanager.Instance.localRigid.GetComponent<Animator>().Play("VisionFadeLocal", 0);
                DOVirtual.DelayedCall(1, () =>
                {
                    scoreBoard.WarpPlayers();
                    Gamemanager.Instance.localRigid.GetComponent<Animator>().Play("ReverseVisionFadeLocal", 0);
                });
            }
        });
    }

    private void DisableObjectsAfterGameStart()
    {
        GetComponent<BoxCollider>().enabled = false;
        foreach (var disableObject in toDisableObjects)
            disableObject.SetActive(false);
    }
    private void EnableObjectsBeforeGameOver()
    {
        foreach (var enableObject in toEnableObjects)
            enableObject.SetActive(true);
    }
}
