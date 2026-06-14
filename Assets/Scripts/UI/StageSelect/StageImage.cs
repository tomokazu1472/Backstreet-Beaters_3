using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
/*
public class StageImage : ButtonBase
{
    [SerializeField] Image _stageImage;
    [SerializeField, Header("アニメーション設定")] float _cursorEnterSize;
    [SerializeField] float _cursorEnterDuration, _cursorExitDuration;
    public void SetImage(Sprite sprite)
    {
        _stageImage.sprite = sprite;
    }

    public void OnCursorEnter()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(rectTransform.DOScale(new Vector3(_cursorEnterSize, _cursorEnterSize, 1f), _cursorEnterDuration).SetEase(Ease.OutCubic))
                .SetLink(gameObject);
    }
    
    public void OnCursorExit()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(rectTransform.DOScale(Vector3.one, _cursorExitDuration).SetEase(Ease.OutCubic))
                .SetLink(gameObject);
    }
    
}
*/