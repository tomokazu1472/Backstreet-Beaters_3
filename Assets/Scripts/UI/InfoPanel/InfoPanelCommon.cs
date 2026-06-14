using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class InfoPanelCommon : MonoBehaviour
{
    [SerializeField] public RectTransform rectTransform;
    [SerializeField] protected RectTransform _missionGroup;
    [SerializeField] protected CanvasGroup _canvasGroup;
    [SerializeField] protected List<StageSelectMission> _missionList;
    [SerializeField] private StageSelectMission _missionPrefab;
    protected StageInfo _stageInfo;

    public virtual void SetStageInfo(StageInfo info)
    {
        _stageInfo = info;
    }

    protected virtual void SetMission(float prefabSize = 1f)
    {
        if (_stageInfo.stageMissions == null) return;

        foreach (BallCollectMission mission in _stageInfo.stageMissions)
        {
            StageSelectMission newMission = Instantiate(_missionPrefab, _missionGroup);
            newMission.gameObject.transform.localScale = new Vector3(prefabSize, prefabSize, 1f);
            newMission.SetDefault(mission.ballColor, mission.amount);
            _missionList.Add(newMission);
        }

    }

    public virtual void ClearInfo()
    {
        foreach (StageSelectMission mission in _missionList)
        {
            if (mission != null)
            {
                Destroy(mission.gameObject);
            }
        }
        _missionList.Clear();
    }
    
    public void SetPosition(Vector2 pos)
    {
        rectTransform.localPosition = pos;
    }

    public void MovetoPosition(Vector2 pos)
    {
        rectTransform.DOLocalMove(pos, 0.3f).SetEase(Ease.OutCirc);
    }

    public void SetAlpha(float alpha)
    {
        _canvasGroup.alpha = alpha;
    }

    public void FadeOut(float duration)
    {
        _canvasGroup.DOFade(0, duration).SetEase(Ease.InOutSine);
    }

    public void FadeIn(float duration)
    {
        _canvasGroup.DOFade(1, duration).SetEase(Ease.InOutSine);
    }

    public void FadeTo(float value, float duration)
    {
        _canvasGroup.DOFade(value, duration).SetEase(Ease.InOutSine);
    }
}
