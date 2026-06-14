using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseButton : ButtonBase
{
    [Header("スクリプト")]
    [SerializeField] Sprite _pauseIcon;
    [SerializeField] Sprite _resumeIcon;
    [SerializeField] Image _iconImage;
    [SerializeField] RectTransform _mainPos, _shadowPos;
    [SerializeField] Image _mainimage, _shadowImage;
    [SerializeField] Color _buttonColor, _buttonShadowColor;
    [SerializeField] Color _hoverColor;
    [SerializeField] bool _isActive;
    public bool isActive => _isActive;

    [Header("アニメーション設定")]
    [SerializeField] float _durationIn = 0.2f;
    [SerializeField] float _durationOut = 0.2f;

    public override void InitButton()
    {
        base.InitButton();

        _mainPos.DOKill(); // 進行中Tween停止
        _shadowPos.DOKill();
        _mainPos.localScale = Vector3.one;
        _shadowPos.localScale = Vector3.one;
        _mainPos.localRotation = Quaternion.Euler(0f, 0f, 0f);
        _shadowPos.localRotation = Quaternion.Euler(0f, 0f, 0f);
        
        _mainimage.color = _buttonColor;
        _shadowImage.color = _buttonShadowColor;
    }

    public void SetPauseIcon()
    {
        _iconImage.sprite = _resumeIcon;
    }

    public void SetResumeIcon()
    {
        _iconImage.sprite = _pauseIcon;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (_mainPos == null || _shadowPos == null) return;

        // 既存Tweenを止めてから開始
        _mainPos.DOKill();
        _shadowPos.DOKill();

        DOTween.Sequence().Append(_mainPos.DOLocalRotate(new Vector3(0f, 0f, 4f), _durationIn).SetEase(Ease.OutBack))
                .Join(_mainPos.DOScale(new Vector2(1.08f, 1.08f), _durationIn).SetEase(Ease.OutBack))
                .SetLink(gameObject).SetUpdate(true);

        DOTween.Sequence().Append(_shadowPos.DOLocalRotate(new Vector3(0f, 0f, -2f), _durationIn).SetEase(Ease.OutBack))
                .Join(_shadowPos.DOScale(new Vector2(1.08f, 1.08f), _durationIn).SetEase(Ease.OutBack))
                .SetLink(gameObject).SetUpdate(true);            
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        if (_mainPos == null || _shadowPos == null) return;

        // 既存Tweenを止めてから開始
        _mainPos.DOKill();
        _shadowPos.DOKill();

        DOTween.Sequence().Append(_mainPos.DOLocalRotate(Vector3.zero, _durationOut).SetEase(Ease.OutBack))
                .Join(_mainPos.DOScale(new Vector2(1f, 1f), _durationOut).SetEase(Ease.OutBack))
                .SetLink(gameObject).SetUpdate(true);
                
        DOTween.Sequence().Append(_shadowPos.DOLocalRotate(Vector3.zero, _durationOut).SetEase(Ease.OutBack))
                .Join(_shadowPos.DOScale(new Vector2(1f, 1f), _durationOut).SetEase(Ease.OutBack))
                .SetLink(gameObject).SetUpdate(true);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        if (_mainPos == null || _shadowPos == null) return;
        
        // 既存Tweenを止めてから開始
        _mainPos.DOKill();
        _shadowPos.DOKill();

        DOTween.Sequence().Append(_mainPos.DOPunchRotation(new Vector3(0, 0, -3f), 0.25f, 4, 1f))
                .Join(_mainPos.DOPunchScale(new Vector3(-0.1f, -0.1f, 0), 0.3f, 4, 1f))
                .Join(_mainimage.DOColor(_hoverColor, 0.1f))
                .AppendInterval(0.1f)
                .Append(_mainimage.DOColor(_buttonColor, 0.1f))
                .SetLink(gameObject).SetUpdate(true);

        DOTween.Sequence().Append(_shadowPos.DOPunchRotation(new Vector3(0, 0, 2f), 0.25f, 4, 1f))
                .Join(_shadowPos.DOPunchScale(new Vector3(-0.1f, -0.1f, 0), 0.3f, 4, 1f))
                .SetLink(gameObject).SetUpdate(true);
    }
}
