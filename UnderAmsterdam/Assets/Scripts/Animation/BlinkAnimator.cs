using DG.Tweening;
using UnityEngine;

public class BlinkAnimator : MonoBehaviour
{
    [SerializeField] private float _blinkDuration;
    [SerializeField] private float _pauseDuration;
    [SerializeField] private MeshRenderer _leftEye;
    [SerializeField] private MeshRenderer _rightEye;

    private Sequence _moveSequence;

    private void OnEnable()
    {
        var randomPause = Random.Range(1, 1.5f);
        _moveSequence?.Kill();
        _moveSequence = DOTween.Sequence()
            .Append(_leftEye.transform.DOScaleY(0, _blinkDuration).SetEase(Ease.InSine))
            .Join(_rightEye.transform.DOScaleY(0, _blinkDuration).SetEase(Ease.InSine))
            .Append(_leftEye.transform.DOScaleY(1, _blinkDuration * .5f).SetEase(Ease.OutSine))
            .Join(_rightEye.transform.DOScaleY(1, _blinkDuration * .5f).SetEase(Ease.OutSine))
            .AppendInterval(_pauseDuration * randomPause)
            .SetLoops(-1, LoopType.Restart);
    }
}
