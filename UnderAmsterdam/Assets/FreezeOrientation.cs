using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeOrientation : MonoBehaviour
{
    private Transform player;
    private Transform watch;
    private float localY;

    private void Start()
    {
        player = Gamemanager.Instance.lPlayerCC.transform.GetChild(0).GetChild(0);
        watch = transform.parent.GetChild(1).GetChild(2);
        localY = watch.position.y + 0.035f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(player);
        transform.position = new Vector3(watch.position.x, localY, watch.position.z);
        localY += 0.0015f;
    }
}
