using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoostSelectButton : ButtonBase
{
    [SerializeField, Header("オブジェクト")] RectTransform _mainTransform;
    [SerializeField] Image _discriptionImage, _mainImage;
    [SerializeField] Color _mainColor, _selectedColor, _hoverColor, _hoverColorSelected;
    [SerializeField] BoostSelect _boostSelect;
    [SerializeField] bool _isSelected;
    public bool isSelected => _isSelected;
    [SerializeField, Header("アニメーション設定")] float _hoverSize;
    [SerializeField] float _clickAnimationSizeDelta;
    [SerializeField] float _clickAnimationDuration;

    public override void InitButton()
    {
        base.InitButton();

        _mainImage.color = _mainColor;
        _mainTransform.localScale = Vector3.one;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        _mainTransform.DOScale(new Vector3(_hoverSize, _hoverSize, 1f), 0.1f).SetEase(Ease.OutSine);

        if (_isSelected)
        {
            _mainImage.DOColor(_hoverColorSelected, 0.2f).SetEase(Ease.OutSine);
        }
        else
        {
            _mainImage.DOColor(_hoverColor, 0.2f).SetEase(Ease.OutSine);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        _mainTransform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutSine);

        if (_isSelected)
        {
            _mainImage.DOColor(_selectedColor, 0.2f).SetEase(Ease.OutSine);
        }
        else
        {
            _mainImage.DOColor(_mainColor, 0.2f).SetEase(Ease.OutSine);
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        _mainTransform.DOPunchScale(new Vector3(_clickAnimationSizeDelta, _clickAnimationSizeDelta, 1f), _clickAnimationDuration).SetEase(Ease.OutSine);
        _boostSelect.OnBoostSelectButtonPressed(this);
    }
    
    public void SetSelected(bool isSelectable)
    {
        if (isSelectable)
        {
            _mainImage.DOColor(_selectedColor, 0.1f).SetEase(Ease.OutSine);
            _isSelected = true;
        }
        else
        {
            _mainImage.DOColor(_mainColor, 0.1f).SetEase(Ease.OutSine);
            _isSelected = false;
        }
    }
}
