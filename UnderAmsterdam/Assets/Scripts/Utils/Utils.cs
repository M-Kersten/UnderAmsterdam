using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Host.Rig;

public class Utils : MonoBehaviour
{
    public NetworkRig rig;
    public Transform transformPlayer;
    // Start is called before the first frame update
    void Start()
    {
        //set the layer for the local player so he can't see his own cap
        if (rig.IsLocalNetworkRig)
            SetRenderLayer(transformPlayer, 9);
    }

    private static void SetRenderLayer(Transform transformPlayer, int layerNumber) 
    {
        foreach (Transform transform in transformPlayer.GetComponentsInChildren<Transform>(true))
            transform.gameObject.layer = layerNumber;
    }

}
