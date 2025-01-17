using DG.Tweening;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Serialization;

public class HoverAnimation : MonoBehaviour
{
    [SerializeField] private float _hoverDuration;
    [SerializeField] private float _hoverDistance;
    [SerializeField] private float _rotationDuration;
    [SerializeField] private Axis _rotationAxis;
    
    private void OnEnable()
    {
        var startPosition = transform.position.y;
        var startRotation = transform.eulerAngles;

        var axisValue = _rotationAxis switch
        {
            Axis.X => Vector3.right,
            Axis.Y => Vector3.up,
            Axis.Z => Vector3.forward,
            _ => Vector3.zero
        };

        transform.DOMoveY(startPosition + _hoverDistance, _hoverDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        if (_rotationAxis != Axis.None)
        {
            transform.DOLocalRotate(startRotation + axisValue * 360, _rotationDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental);
        }
    }
}
