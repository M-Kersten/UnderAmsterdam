using Fusion.XR.Host.Locomotion;
using UnityEngine;

namespace Fusion.XR.Host.Rig
{
    /**
     * 
     * Head class for the hardware rig
     * 
     * The gameobject should susually contain a Camera and a Fader
     * 
     **/

    public class HardwareHeadset : MonoBehaviour
    {
        public Fader fader;
        public NetworkTransform networkTransform;

        private void Awake()
        {
            if(fader == null) fader = GetComponentInChildren<Fader>();
            if (networkTransform == null) networkTransform = GetComponent<NetworkTransform>();
        }
    }
}
