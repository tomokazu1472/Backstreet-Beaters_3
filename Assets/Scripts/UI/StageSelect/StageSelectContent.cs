using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StageSelectContent : ButtonBase
{
    [SerializeField] Image _contentImage;
    [SerializeField] DifficulityParams _difficulityParams;
    [SerializeField] Color _defaultColor, _selectedColor;
    [SerializeField] Image _difficulityBackground;
    [SerializeField] TextMeshProUGUI _SongTitle;
    [SerializeField] TextMeshProUGUI _difficulityText;
    [SerializeField, Header("アニメーション設定")] AnimationCurve _clickCurve;
    [SerializeField] float _defaultPosX, _shiftPosX;
    [SerializeField] float _enterTime, _exitTime, _clickDuration;
    [SerializeField] float _hoverSize, _clickScale;
    private StageInfo _stageInfo;
    private StageGroup _stageGroup;
    private Sequence _sequence;
    private int _stageID;
    public int stageID => _stageID;
    private bool _isSelected;

    public void SetStageInfo(StageInfo stageInfo, StageGroup stageGroup, int index)
    {
        _stageInfo = stageInfo;
        _stageGroup = stageGroup;
        _SongTitle.SetText(stageInfo.bgmInfo.songName);
        _difficulityText.SetText(stageInfo.difficulity.ToString());
        _contentImage.color = _defaultColor;

        _stageID = index;

        _difficulityBackground.color = _difficulityParams.GetDifficulityColor(stageInfo.difficulity);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        _stageGroup.SetCurrentInfo(_stageInfo);

        _sequence?.Kill();
        _sequence = DOTween.Sequence();

        _sequence.Append(rectTransform.DOLocalMoveX(_shiftPosX, _enterTime).SetEase(Ease.OutCubic))
                .Join(rectTransform.DOScale(new Vector3(_hoverSize, _hoverSize, 1f), _enterTime).SetEase(Ease.OutBack));
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        _stageGroup.SetToDefaultInfo();

        if (!_isSelected)
        {
            Unselect();
        }
    }


    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        _stageGroup.UnselectAll();
        _stageGroup.SetSelectedInfo(_stageInfo);
        _isSelected = true;

        _sequence?.Kill();
        _sequence = DOTween.Sequence();

        _contentImage.color = _selectedColor;
        _sequence.Append(rectTransform.DOScale(_clickScale, _clickDuration).SetEase(_clickCurve));
    }

    public void Unselect()
    {
        _isSelected = false;

        _contentImage.color = _defaultColor;

        _sequence?.Kill();
        _sequence = DOTween.Sequence();

        _sequence.Append(rectTransform.DOLocalMoveX(_defaultPosX, _exitTime).SetEase(Ease.OutCubic))
                .Join(rectTransform.DOScale(Vector3.one, _exitTime).SetEase(Ease.OutBack));

    }
}
