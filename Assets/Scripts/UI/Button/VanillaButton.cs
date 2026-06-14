using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VanillaButton : ButtonBase
{
    [Header("スクリプト")]
    [SerializeField] TextMeshProUGUI _buttonText;
    [SerializeField] RectTransform _mainPos, _shadowPos;
    [SerializeField] Image _mainimage, _shadowImage;
    [SerializeField] Color _buttonColor, _buttonShadowColor;
    [SerializeField] Color _hoverColor, _disableColor;
    Sequence _sequence;

    [Header("アニメーション設定")]
    [SerializeField] float _durationIn = 0.2f;
    [SerializeField] float _durationOut = 0.2f;

    public override void InitButton()
    {
        base.InitButton();

        _mainimage.color = _buttonColor;
        _shadowImage.color = _buttonShadowColor;

        InitPosition();
    }
    
    void InitPosition()
    {
        _mainPos.localScale = Vector3.one;
        _shadowPos.localScale = Vector3.one;
        _mainPos.localRotation = Quaternion.Euler(0f, 0f, 0f);
        _shadowPos.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (_isSelectable == false) return;
        base.OnPointerEnter(eventData);


        // 既存Tweenを止めてから開始
        _sequence?.Kill();
        
        InitButton();

        _sequence = DOTween.Sequence().Append(_mainPos.DOLocalRotate(new Vector3(0f, 0f, 4f), _durationIn).SetEase(Ease.OutBack))
                .Join(_mainPos.DOScale(1.08f, _durationIn).SetEase(Ease.OutBack))
                .Join(_shadowPos.DOLocalRotate(new Vector3(0f, 0f, -2f), _durationIn).SetEase(Ease.OutBack))
                .SetLink(gameObject).SetUpdate(true);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        // 既存Tweenを止めてから開始
        if (_sequence != null) _sequence.Kill();
        _sequence = DOTween.Sequence();
        
        _sequence.Append(_mainPos.DOLocalRotate(Vector3.zero, _durationOut).SetEase(Ease.OutBack))
                .Join(_mainPos.DOScale(1f, _durationOut).SetEase(Ease.OutBack))
                .Join(_shadowPos.DOLocalRotate(Vector3.zero, _durationOut).SetEase(Ease.OutBack))
                .OnComplete(() =>
                {
                    InitButton();
                })
                .SetLink(gameObject).SetUpdate(true);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (_isSelectable == false) return;
        base.OnPointerClick(eventData);

        
        // 既存Tweenを止めてから開始
        if (_sequence != null) _sequence.Kill();
        _sequence = DOTween.Sequence();

        InitButton();

        _sequence.Append(_mainPos.DOPunchRotation(new Vector3(0, 0, -3f), 0.25f, 4, 1f))
                .Join(_mainPos.DOPunchScale(new Vector3(-0.1f, -0.1f, 0), 0.3f, 4, 1f))
                .Join(_mainimage.DOColor(_hoverColor, 0.1f))
                .Join(_shadowPos.DOPunchRotation(new Vector3(0, 0, 2f), 0.25f, 4, 1f))
                .Append(_mainimage.DOColor(_buttonColor, 0.1f))
                .OnComplete(() =>
                {
                    InitButton();
                })
                .SetLink(gameObject).SetUpdate(true);
    }

    public void SetButtonText(string text)
    {
        if (_buttonText != null)
        {
            _buttonText.text = text;
        }
    }

    public override void SetButtonSelectable(bool isEnable)
    {
        base.SetButtonSelectable(isEnable);
        
        if (isEnable)
        {
            _mainimage.color = _buttonColor;
        }
        else
        {
            _mainimage.color = _disableColor;
        }
    }
}
