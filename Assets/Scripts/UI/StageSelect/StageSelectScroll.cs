using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Linq;

public class StageSelectScroll : MonoBehaviour
{
    [SerializeField] StageGroup _stageGroup;
    [SerializeField] RectTransform _stageSelectScroll;
    [SerializeField] TextMeshProUGUI _backText, _discriptionText;
    [SerializeField, Tooltip("登場時の登場遅延")] float _enterDelay;
    [SerializeField, Tooltip("登場時の裏の文字のアニメーション時間")] float _backTextEnterTime;
    [SerializeField, Tooltip("登場時の説明テキストのアニメーション時間")] float _discriptionTextEnterTime;
    [SerializeField, Tooltip("退場時のアニメーション時間")] float _exitTime;
    [SerializeField, Tooltip("登場時の裏の文字の初期位置")] Vector2 _backTextEnterPos;
    [SerializeField, Tooltip("登場時の説明テキストの初期位置")] Vector2 _discriptionTextEnterPos;
    [SerializeField, Tooltip("裏の文字のデフォ位置")] Vector2 _backTextPos;
    [SerializeField, Tooltip("説明テキストのデフォ位置")] Vector2 _discriptionTextPos;
    private Sequence _sequence;

    void Start()
    {
        _backText.rectTransform.localPosition = _backTextEnterPos;
        _discriptionText.rectTransform.localPosition = _discriptionTextEnterPos;
    }

    public void GoToStageSelect()
    {
        _stageGroup.ShowStagePanels();
        
        _sequence?.Kill();
        _sequence = DOTween.Sequence();

        _sequence.AppendInterval(_enterDelay)
                .Append(_backText.rectTransform.DOLocalMove(_backTextPos, _backTextEnterTime).From(_backTextEnterPos).SetEase(Ease.OutExpo))
                .Join(_discriptionText.rectTransform.DOLocalMove(_discriptionTextPos, _discriptionTextEnterTime).From(_discriptionTextEnterPos).SetEase(Ease.OutExpo))
                .SetLink(gameObject);
    }

    public void ExitStageSelect()
    {
        _stageGroup.HideStagePanels();

        _sequence?.Kill();
        _sequence = DOTween.Sequence();

        _sequence.Append(_backText.rectTransform.DOLocalMove(_backTextEnterPos, _exitTime).SetEase(Ease.InExpo))
                .Join(_discriptionText.rectTransform.DOLocalMove(_discriptionTextEnterPos, _exitTime).SetEase(Ease.InExpo))
                .SetLink(gameObject);
    }
}