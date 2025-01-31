using System.Collections;
using DG.Tweening;
using UnityEngine;
using TMPro;

public class InOutIndicatorScript : MonoBehaviour
{
    [SerializeField] private GameObject LocalPlayer;
    [SerializeField] private TextMeshProUGUI textObject;
    [SerializeField] private float indicatorAliveTime;
    
    public void InitializeIndicator(bool shouldBeOutput)
    {
        LocalPlayer = GameObject.Find("LocalPlayerSession").gameObject;
        textObject.text = shouldBeOutput ? "OUT" : "IN";

        var originalScale = transform.localScale;
        transform.localScale = Vector3.zero;
        transform.DOScale(originalScale, 0.3f).SetEase(Ease.OutQuad);
        
        DOVirtual.DelayedCall(indicatorAliveTime, Despawn);
    }

    void Despawn()
    {
        transform.DOScale(Vector3.zero, .3f).SetEase(Ease.InSine).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
    
    void FixedUpdate()
    {
        RotateTowardsPlayer();
    }
    
    private void RotateTowardsPlayer()
    {
        var lookPos = transform.position - LocalPlayer.transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime);
    }
}
