using System;
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
    [SerializeField] private bool _lookatPlayer;

    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void OnEnable()
    {
        var startPosition = transform.localPosition.y;
        var startRotation = transform.eulerAngles;

        var axisValue = _rotationAxis switch
        {
            Axis.X => Vector3.right,
            Axis.Y => Vector3.up,
            Axis.Z => Vector3.forward,
            _ => Vector3.zero
        };

        transform.DOLocalMoveY(startPosition + _hoverDistance, _hoverDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        if (_rotationAxis != Axis.None)
        {
            transform.DOLocalRotate(startRotation + axisValue * 360, _rotationDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental);
        }
    }

    private void Update()
    {
        if (_lookatPlayer && _rotationAxis == Axis.None)
        {
            // lookat player but only rotate in y axis
            var lookatPosition = _camera.transform.position;
            lookatPosition.y = transform.position.y;
            transform.LookAt(lookatPosition, Vector3.up);
        }
    }
}
