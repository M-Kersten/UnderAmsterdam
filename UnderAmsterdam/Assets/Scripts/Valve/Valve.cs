using UnityEngine;
using UnityEngine.Events;

public class Valve : MonoBehaviour
{
    [SerializeField] private Transform valve;
    [SerializeField] private Transform valveCenter;
    [SerializeField] private Transform rHandTransform;
    [SerializeField] private Transform lHandTransform;
    [SerializeField] private LayerMask layerMask;

    public UnityEvent ValveTurned;
    
    private PlayerInputHandler playerInputHandler;
    private Vector3 direction;
    private Ray ray;
    private Vector3 valveCenterPos;
    private float angle;
    
    private void Start()
    {
        playerInputHandler = Gamemanager.Instance.playerInputHandler;
        lHandTransform = playerInputHandler.transform.GetChild(1).transform; // second child of local player is always left hand
        rHandTransform = playerInputHandler.transform.GetChild(2).transform; // third child of local player is always right hand
    }
    
    private void FixedUpdate()
    {
        ray.origin = valveCenter.position;
        
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
            Vector3 lHandPosition = lHandTransform.position;
            valveCenterPos = valveCenter.position;

            direction.x = lHandPosition.x - valveCenterPos.x;
            direction.y = lHandPosition.y - valveCenterPos.y;

            ray.direction = direction.normalized;
        }

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 0.25f, layerMask))
            angle = -Mathf.Atan2(ray.direction.y, ray.direction.x) * Mathf.Rad2Deg;

        Vector3 newForward = Quaternion.Euler(0, angle, 0) * valve.transform.forward;
        Debug.DrawRay(valveCenterPos, newForward * 0.25f, Color.green);

        valve.transform.localRotation = Quaternion.Slerp(valve.transform.localRotation, Quaternion.Euler(angle, 90, -90), 20f * Time.deltaTime);
        valveTurned();
    }

    private void valveTurned()
    {
        if ((valve.transform.localRotation.eulerAngles.x >= 90 || valve.transform.localRotation.eulerAngles.x <= -90))
        {
            ValveTurned.Invoke();
        }
    }
}
