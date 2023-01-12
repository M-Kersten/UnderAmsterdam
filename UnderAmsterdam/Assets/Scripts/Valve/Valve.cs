using UnityEngine;

public class Valve : MonoBehaviour
{
    [SerializeField] private Transform valve;
    [SerializeField] private Transform valveCenter;
    [SerializeField] private Transform rHandTransform;
    [SerializeField] private Transform lHandTransform;
    [SerializeField] private LayerMask layerMask;

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

        Vector3 previousDir = new Vector3(0,0,0);
        if (!playerInputHandler.isRightGripPressed && !playerInputHandler.isLeftGripPressed)
        {
            previousDir = direction;
        }


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
            //angle = -Mathf.Atan2(ray.direction.y - valveCenter.position.y, ray.direction.x - valveCenterPos.x) * Mathf.Rad2Deg;
            angle = -Mathf.Atan2(ray.direction.y, ray.direction.x) * Mathf.Rad2Deg;
            float prevAngle = -Mathf.Atan2(previousDir.y, previousDir.x) * Mathf.Rad2Deg;
            angle += prevAngle;
            //angle += valve.transform.localRotation.x;
        }
        valve.transform.localRotation = Quaternion.Slerp(valve.transform.localRotation, Quaternion.Euler(angle, 90, -90), 20f * Time.deltaTime);
        Debug.DrawRay(ray.origin, ray.direction);
    }
}
