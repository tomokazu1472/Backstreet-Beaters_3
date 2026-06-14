using TMPro;
using UnityEngine;

public class MissionWindow : InfoPanelCommon
{
    [SerializeField] TextMeshProUGUI _missionDiscription;

    public override void SetStageInfo(StageInfo info)
    {
        base.SetStageInfo(info);
        SetMission(1.3f);
    }

    protected override void SetMission(float prefabSize = 1.0f)
    {
        base.SetMission(prefabSize);
        
        _missionDiscription.SetText(_stageInfo.missionDiscription);
    }
}
