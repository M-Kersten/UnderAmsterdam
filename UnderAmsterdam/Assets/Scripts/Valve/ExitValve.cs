using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;

public class ExitValve : MonoBehaviour
{
    [SerializeField] private Transform valve;
    [SerializeField] private Transform valveCenter;
    [SerializeField] private Transform rHandTransform;
    [SerializeField] private Transform lHandTransform;
    [SerializeField] private LayerMask layerMask;
    
    [SerializeField] private PlayerInputHandler playerInputHandler;


    
    private Vector3 direction;
    private Ray ray;
    private RaycastHit hit;
    private Vector3 valveCenterPos;

    [SerializeField] private float angle;

    public NetworkRunner runner;

    private void Start()
    {
        ray.origin = valveCenter.position;
        if (!playerInputHandler)
            playerInputHandler = GetComponentInParent<PlayerInputHandler>();
    }

    private void FixedUpdate()
    {
        if (playerInputHandler.isRightGripPressed)
        {
            Vector3 rHandPosition = rHandTransform.position;
            valveCenterPos = valveCenter.position;
            
            direction.x = rHandPosition.x - valveCenterPos.x;
            direction.y = rHandPosition.y - valveCenterPos.y;

            ray.direction = direction.normalized;
        }
        else if (playerInputHandler.isLeftGripPressed)
        {
            Vector3 rHandPosition = lHandTransform.position;
            valveCenterPos = valveCenter.position;
            
            direction.x = rHandPosition.x - valveCenterPos.x;
            direction.y = rHandPosition.y - valveCenterPos.y;

            ray.direction = direction.normalized;
        }
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 10f, layerMask))
        {
            angle = -Mathf.Atan2(ray.direction.y - valveCenter.position.y, ray.direction.x - valveCenterPos.x) * Mathf.Rad2Deg;
        }
        valve.transform.localRotation = Quaternion.Slerp(valve.transform.localRotation, Quaternion.Euler(angle, 90, -90), 20f * Time.deltaTime);
        Debug.DrawRay(ray.origin, ray.direction);
    }
    private void Left()
    {
        SceneManager.LoadScene(name);
        Debug.Log("exit");
    }
}
