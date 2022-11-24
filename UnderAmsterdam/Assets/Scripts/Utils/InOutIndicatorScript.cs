using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InOutIndicatorScript : MonoBehaviour
{
    [SerializeField] private GameObject LocalPlayer;
    [SerializeField] private TextMeshProUGUI textObject;
    [SerializeField] private float distance, indicatorAliveTime, forwardModifier, heightOffset;

    private Vector3 startPos, forwardDir;
    public void InitializeIndicator(bool shouldBeOutput)
    {
        LocalPlayer = GameObject.Find("LocalPlayer").gameObject;
        textObject.text = shouldBeOutput ? "OUT" : "IN";
        startPos = transform.position + new Vector3(0, heightOffset, 0);
        forwardDir = (LocalPlayer.transform.position - transform.position).normalized;
        StartCoroutine(DestroyTimer(indicatorAliveTime));
    }
    void FixedUpdate()
    {
        RotateTowardsPlayer();
        transform.position = startPos + new Vector3(0, Mathf.Sin(Time.time), 0) + (forwardDir * forwardModifier);
    }
    private void RotateTowardsPlayer()
    {
        var lookPos = transform.position - LocalPlayer.transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime);
    }
    private IEnumerator DestroyTimer(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
        yield return null;
    }
}
