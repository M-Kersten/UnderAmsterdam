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

    [SerializeField] private PlayerInputHandler playerInputHandler;

    [SerializeField] private LayerMask layerMask;
    private Vector3 direction;
    private Ray ray;
    private RaycastHit hit;


    [SerializeField] private float angle;

    public NetworkRunner runner;

    private void Start()
    {
        ray.origin = valveCenter.position;
    }

    private void FixedUpdate()
    {

        if (playerInputHandler.isRightGripPressed)
        {
            direction.x = rHandTransform.position.x - valveCenter.position.x;
            direction.y = rHandTransform.position.y - valveCenter.position.y;

            ray.direction = direction.normalized;

            if (Physics.Raycast(ray, out RaycastHit hitInfo, 0.5f, layerMask))
            {
                angle = -Mathf.Atan2(ray.direction.y - valveCenter.position.y, ray.direction.x - valveCenter.position.x) * Mathf.Rad2Deg;
            }
        }
        else if (playerInputHandler.isLeftGripPressed)
        {
            direction.x = lHandTransform.position.x - valveCenter.position.x;
            direction.y = lHandTransform.position.y - valveCenter.position.y;

            ray.direction = direction.normalized;

            if (Physics.Raycast(ray, out RaycastHit hitInfo, 10f, layerMask))
            {
                angle = -Mathf.Atan2(ray.direction.y - valveCenter.position.y, ray.direction.x - valveCenter.position.x) * Mathf.Rad2Deg;
            }
        }
        valve.transform.localRotation = Quaternion.Slerp(valve.transform.localRotation, Quaternion.Euler(angle, 90, -90), 20f * Time.deltaTime);

        Debug.DrawRay(ray.origin, ray.direction);




    }
    private void left()
    {
        SceneManager.LoadScene(name);
        Debug.Log("exit");
    }
}
