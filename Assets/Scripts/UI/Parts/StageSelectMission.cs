using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectMission : MonoBehaviour
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] Image _missionBallImage;
    [SerializeField] TextMeshProUGUI _missionBallsAmountText, _completeText;
    [SerializeField] CanvasGroup _partsGroup;
    [SerializeField] List<Sprite> _ballImages;
    [SerializeField, Header("アニメーション設定")] float _completedPartsAlpha;
    [SerializeField] float _completePartsDuration;
    [SerializeField] float _completeTextDuration;
    [Header("情報")] public ItemId item;
    public int amount;
    private bool _isCompleted;

    public void SetPosition(Vector2 pos)
    {
        rectTransform.localPosition = pos;
    }

    public void SetDefault(ItemId itemId, int amount)
    {
        item = itemId;
        this.amount = amount;
        SetImage(itemId);
        SetAmount(amount);
        _partsGroup.alpha = 1f;
        _completeText.alpha = 0f;
        _completeText.rectTransform.localScale = new Vector3(2f, 2f, 1f);
        _isCompleted = false;
    }

    void SetImage(ItemId itemId)
    {
        _missionBallImage.sprite = _ballImages[(int)itemId];
    }

    public void SetAmount(int amount)
    {
        _missionBallsAmountText.SetText($"{amount}");
    }

    public void CompleteAnimation()
    {
        if (_isCompleted) return;
        SetAmount(0);

        Sequence sequence = DOTween.Sequence();
        _isCompleted = true;

        sequence.Append(_partsGroup.DOFade(_completedPartsAlpha, _completePartsDuration).SetEase(Ease.InOutSine))
                .Join(_completeText.DOFade(1f, _completeTextDuration).SetEase(Ease.InCubic))
                .Join(_completeText.rectTransform.DOScale(new Vector3(1f, 1f, 1f), _completeTextDuration).SetEase(Ease.InCubic))
                .SetLink(gameObject);
    }
}
