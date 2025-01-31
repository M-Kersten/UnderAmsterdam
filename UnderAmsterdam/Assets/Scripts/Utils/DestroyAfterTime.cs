using DG.Tweening;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] float time;

    private void Start() => DOVirtual.DelayedCall(time, () => Destroy(gameObject));
}
