using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TramAnimator : MonoBehaviour
{
    [SerializeField] private float _moveDuration;
    [SerializeField] private float _pauseDuration;
    [SerializeField] private float _moveDistance;

    private Sequence _moveSequence;
    private Vector3 _startPosition;
    
    
    private void Awake()
    {
        _startPosition = transform.position;
    }

    private void OnEnable()
    {
        _moveSequence?.Kill();
        _moveSequence = DOTween.Sequence()
            .Append(transform.DOMoveX(_startPosition.x + _moveDistance, _moveDuration).SetEase(Ease.InOutSine))
            .AppendInterval(_pauseDuration)
            .Append(transform.DOMoveX(_startPosition.x, _moveDuration).SetEase(Ease.InOutSine))
            .AppendInterval(_pauseDuration)
            .SetLoops(-1, LoopType.Restart);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position,
            transform.position + Vector3.right * _moveDistance);
    }
}
