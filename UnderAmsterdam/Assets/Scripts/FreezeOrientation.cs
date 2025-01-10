using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeOrientation : MonoBehaviour
{
    [SerializeField] private float yIncrease = 0.0015f;
    private Transform player;
    private float localY;

    private void Start()
    {
        player = Gamemanager.Instance.mainCam;
        transform.parent = Gamemanager.Instance.networkData.transform;
        localY = transform.position.y + 0.055f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.LookAt(player);
        transform.position = new Vector3(transform.position.x, localY, transform.position.z);
        localY += yIncrease;
    }
}
