using Fusion.XR.Host.Rig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Fusion.XR.Host.Locomotion
{
    /**
     * 
     * Simple locomotion system
     * - trigger a fade and rotation when input horizontal axis is above a threshold
     * - trigger a fade and teleport when Teleport method called (usually from a RayBeamer)
     * 
     * Look for child RayBeamer, to trigger Teleport on the onRelease event of the beamers
     * 
     * Ensure that there is no bounce effect, with several movements called too quickly when the user keeps pressing the associated input  
     * 
     **/

    [RequireComponent(typeof(HardwareRig))]
    public class RigLocomotion : MonoBehaviour
    {
        [Header("Snap turn")]
        public InputActionProperty leftControllerTurnAction;
        public InputActionProperty rightControllerTurnAction;

        public float debounceTime = 0.5f;
        public float snapDegree = 45f;
        public float rotationInputThreshold = 0.5f;

        [Header("Teleportation")]
        [Tooltip("Automatically found if not set")]
        public List<RayBeamer> teleportBeamers;

        bool rotating = false;
        float timeStarted = 0;
        private float turnTimer = 0f;

        HardwareRig rig;

        public LayerMask locomotionLayerMask = 0;

        // If locomotion constraints are needed, a ILocomotionValidationHandler can restrict them
        ILocomotionValidationHandler locomotionValidationHandler;

        private void Awake()
        {
            rig = GetComponentInParent<HardwareRig>();
            locomotionValidationHandler = GetComponentInParent<ILocomotionValidationHandler>();

            if (teleportBeamers.Count == 0) teleportBeamers = new List<RayBeamer>(GetComponentsInChildren<RayBeamer>());
            
            foreach (var beamer in teleportBeamers)
                beamer.onRelease.AddListener(OnBeamRelease);

            var bindings = new List<string> { "joystick" };
            leftControllerTurnAction.EnableWithDefaultXRBindings(leftBindings: bindings);
            rightControllerTurnAction.EnableWithDefaultXRBindings(rightBindings: bindings);
        }

        private void Start()
        {
            if (locomotionLayerMask == 0)
                Debug.LogError("RigLocomotion: for locomotion to be possible, at least one layer has to be added to locomotionLayerMask, add used on locomotion surface colliders");
        }

        private void FixedUpdate()
        {
            CheckSnapTurn();
        }

        protected virtual void CheckSnapTurn()
        {
            if (rotating) 
                return;
            
            turnTimer += Time.deltaTime;
            if (turnTimer <= debounceTime)
                return;

            var rightStickTurn = rightControllerTurnAction.action.ReadValue<Vector2>().x;
            var snapAngle = Mathf.Sign(rightStickTurn) * snapDegree;

            if (Mathf.Abs(rightStickTurn) > rotationInputThreshold)
            {
                turnTimer = 0;
                rotating = true;
                rig.Rotate(snapAngle);
                rotating = false;
            }
        }

        public virtual bool ValidLocomotionSurface(Collider surfaceCollider)
        {
            // We check if the hit collider is in the locomoation layer mask
            bool colliderInLocomotionLayerMask = locomotionLayerMask == (locomotionLayerMask | (1 << surfaceCollider.gameObject.layer));
            return colliderInLocomotionLayerMask;
        }

        protected virtual void OnBeamRelease(Collider lastHitCollider, Vector3 position)
        {
            if (!enabled) 
                return;
            // Checking potential validation handler
            if (locomotionValidationHandler != null)
            {
                var headsetPositionRelativeToRig = rig.transform.InverseTransformPoint(rig.headset.transform.position);
                Vector3 newHeadsetPosition = position + headsetPositionRelativeToRig.y * rig.transform.up;
                if (!locomotionValidationHandler.CanMoveHeadset(newHeadsetPosition)) 
                    return;
            }

            // Checking target surface layer
            if (ValidLocomotionSurface(lastHitCollider))
            {
                StartCoroutine(rig.FadedTeleport(position));
            }
        }
    }
}
