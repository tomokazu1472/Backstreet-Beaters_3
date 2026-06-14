using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
public class StageGroup : MonoBehaviour
{
    [SerializeField, Header("スクリプト")] List<StageInfo> _stages;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] StageInfoPanel _infoPanel;
    [SerializeField] List<StageSelectContent> _stageContentList;
    [SerializeField] StageSelectContent _stageContentPrefab;
    [SerializeField, Tooltip("登場遅延"), Header("アニメーション設定")] float _enterDelay;
    [SerializeField, Tooltip("ステージ情報パネルの登場遅延")] float _panelEnterDelay;
    [SerializeField, Tooltip("登場時の選択項目のアニメーション時間")] float _contentEnterTime, _panelEnterTime;
    [SerializeField, Tooltip("退場時の選択項目のアニメーション時間")] float _exitTime;
    [SerializeField, Tooltip("ちょっとつぶされるときのアニメーション時間")] float _squishInTime, _squishOutTime;
    [SerializeField, Tooltip("登場時の選択項目の初期位置")] Vector2 _contentEnterPos;
    [SerializeField, Tooltip("選択項目のデフォ位置")] Vector2 _contentPos;
    [SerializeField, Tooltip("情報パネルのデフォ位置")] Vector2 _panelPos;
    [SerializeField, Tooltip("登場時のちょっとつぶされる大きさ")] Vector2 _squishScale;
    [SerializeField, Tooltip("登場時の選択項目のちょっとつぶされる位置")] Vector2 _contentSquishPos;
    [SerializeField, Tooltip("登場時の情報パネルのちょっとつぶされる位置")] Vector2 _panelSquishPos;
    private StageInfo _currentInfo = null;
    private StageInfo _selectedInfo = null;

    void Start()
    {
        SetStageList();

        // 初期位置に設定
        rectTransform.localPosition = _contentEnterPos;
        _infoPanel.rectTransform.localPosition = _contentEnterPos;
    }

    void SetStageList()
    {
        SetStageInfoPanel(null);

        int index = 0;
        foreach (StageInfo info in _stages)
        {
            StageSelectContent stageContent = Instantiate(_stageContentPrefab, transform);
            _stageContentList.Add(stageContent);
            stageContent.SetStageInfo(info, this, index);
            index++;
        }
    }

    public void SetStageInfoPanel(StageInfo stageinfo)
    {
        _infoPanel.SetStageInfo(stageinfo);
    }

    public void ShowStagePanels()
    {
        SetStageInfoPanel(null);
        _currentInfo = null;
        _selectedInfo = null;

        Sequence contentSequence = DOTween.Sequence();
        Sequence panelSequence = DOTween.Sequence();

        contentSequence.AppendInterval(_enterDelay)
                .Append(rectTransform.DOLocalMove(_contentPos, _contentEnterTime).SetEase(Ease.OutQuad))

                .Append(rectTransform.DOScale(_squishScale, _squishInTime).SetEase(Ease.InCubic))
                .Join(rectTransform.DOLocalMove(_contentSquishPos, _squishInTime).SetEase(Ease.InCubic))

                .Append(rectTransform.DOScale(Vector3.one, _squishOutTime).SetEase(Ease.InOutSine))
                .Join(rectTransform.DOLocalMove(_contentPos, _squishOutTime).SetEase(Ease.InOutSine))

                .SetLink(gameObject);

        panelSequence.AppendInterval(_panelEnterDelay)
                .Append(_infoPanel.rectTransform.DOLocalMove(_panelPos, _contentEnterTime).SetEase(Ease.OutQuad))

                .Append(_infoPanel.rectTransform.DOScale(_squishScale, _squishInTime).SetEase(Ease.InCubic))
                .Join(_infoPanel.rectTransform.DOLocalMove(_panelSquishPos, _squishInTime).SetEase(Ease.InCubic))

                .Append(_infoPanel.rectTransform.DOScale(Vector3.one, _squishOutTime).SetEase(Ease.InOutSine))
                .Join(_infoPanel.rectTransform.DOLocalMove(_panelPos, _squishOutTime).SetEase(Ease.InOutSine))

                .SetLink(gameObject);
    }

    public void HideStagePanels()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(rectTransform.DOLocalMove(_contentEnterPos, _exitTime).SetEase(Ease.InCirc))
                .Join(_infoPanel.rectTransform.DOLocalMove(_contentEnterPos, _exitTime).SetEase(Ease.InCubic))
                .SetLink(gameObject);
    }

    public void UnselectAll()
    {
        foreach (StageSelectContent stageContent in _stageContentList)
        {
            stageContent.Unselect();
        }
    }

    public void SetSelectedInfo(StageInfo stageInfo)
    {
        _selectedInfo = stageInfo;
        _infoPanel.ClearInfo();
        _infoPanel.SetStageInfo(_selectedInfo);
        _infoPanel.InfoChangedAnimation();
    }

    public void SetCurrentInfo(StageInfo stageInfo)
    {
        _currentInfo = stageInfo;
        _infoPanel.ClearInfo();
        _infoPanel.SetStageInfo(_currentInfo);
        _infoPanel.InfoChangedAnimation();
    }
    
    public void SetToDefaultInfo()
    {
        _currentInfo = _selectedInfo;
        _infoPanel.ClearInfo();
        _infoPanel.SetStageInfo(_currentInfo);
    }
}
