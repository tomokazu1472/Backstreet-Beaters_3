using DG.Tweening;
using UnityEngine;

public class TitleSpeaker : MonoBehaviour
{
    [SerializeField] RectTransform _body, _coneUp, _coneDown;
    [SerializeField] AnimationCurve _bodyEase, _coneEase;
    [SerializeField] float _animationDelay;
    [SerializeField] float _bodyDuration, _coneDuration;

    public void VibeSpeaker()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.AppendInterval(_animationDelay)
                .Append(_body.DOScale(new Vector3(_bodyDuration, _bodyDuration, 1f), 0.3f).From(1.0f).SetEase(_bodyEase))
                .Join(_coneUp.DOScale(new Vector3(_coneDuration, _coneDuration, 1f), 0.3f).From(1.0f).SetEase(_coneEase))
                .Join(_coneDown.DOScale(new Vector3(_coneDuration, _coneDuration, 1f), 0.3f).From(1.0f).SetEase(_coneEase))
                .SetLink(gameObject).SetUpdate(true);
    }
}
