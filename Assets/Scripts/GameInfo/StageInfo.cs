using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StageInfomation")]
public class StageInfo : ScriptableObject
{
    [Tooltip("ステージ名")] public string stageName;
    [Tooltip("ステージのサムネイル")] public Sprite stageImage;
    [Tooltip("ステージのクリア目標")] public List<BallCollectMission> stageMissions;
    [Tooltip("ステージの難易度")] public Difficulity difficulity;
    [Tooltip("ミッションの説明")] public string missionDiscription;
    [Tooltip("ステージのBGM")] public GameBGM bgmInfo;
}

[System.Serializable]
public class BallCollectMission
{
    [Tooltip("収集するボールの色")] public ItemId ballColor;
    [Tooltip("収集するボールの数")] public int amount;
}

