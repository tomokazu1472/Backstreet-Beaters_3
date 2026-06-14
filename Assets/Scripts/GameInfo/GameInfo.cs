using System.Collections.Generic;
using UnityEngine;

public enum ScoreBonus
{
    Normal,
    Item,
    Record,
}
public class GameInfo : MonoBehaviour
{
    [SerializeField, Header("スクリプト")] InfoPanel _infoPanel;
    [SerializeField] ComboCountCtrl _comboCountCtrl;
    [SerializeField] SongInfoPanel _songInfoPanel;
    [SerializeField] MissionWindow _missionWindow;
    [SerializeField] ScreenParts _screenParts;
    [SerializeField] BoostSelect _boostSelect;
    [SerializeField] ResultScreen _resultScreen;
    [SerializeField, Header("スコア")] int _score;
    public int score => _score;
    [SerializeField] int _highscore;
    [SerializeField, Tooltip("1ボールの得点")] int _pointPerBall;
    [SerializeField] StageInfo _testInfo;
    [SerializeField, Header("曲がデフォルトに変わります")] bool _debugMode;
    private static StageInfo _stageInfo;
    public static StageInfo stageInfo => _stageInfo;
    double _remainingTime;
    public double RemainingTime => _remainingTime;
    public Dictionary<ItemId, int> MissionBallsCount;

    public static void SetStageInfo(StageInfo info)
    {
        _stageInfo = info;
    }

    public void ResetGameInfo()
    {
        _score = 0;

        if (_debugMode)
        {
            _stageInfo = _testInfo;
        }
        else if (_stageInfo == null)
        {
            Debug.LogWarning("ステージ情報が指定されていません！デバッグ用データが代入されました");
            _stageInfo = _testInfo;
        }

        BeatCtrl.SetBGMInfo(_stageInfo.bgmInfo);

        SetCurrentSongTitle();
        _infoPanel.SetStageInfo(_stageInfo);
        _infoPanel.SetScoreText(_score);
        _songInfoPanel.SetSongInfo(_stageInfo);
        _missionWindow.SetStageInfo(_stageInfo);
        _boostSelect.SetBoostSelectPanel();
        _resultScreen.SetStageInfo(_stageInfo);
        SetMissionBallsCount(_stageInfo);
    }

    void SetMissionBallsCount(StageInfo stage)
    {
        MissionBallsCount = new();
        foreach (BallCollectMission mission in stage.stageMissions)
        {
            MissionBallsCount[mission.ballColor] = mission.amount;
        }
    }

    public void AddScore(int ballAmount, ScoreBonus bonusType)
    {
        int earnScore = (int)(ballAmount * (1f + (_comboCountCtrl.comboCount / 100f))) * _pointPerBall;
        _score += earnScore;
        if (score != 0)
        {
            _infoPanel.SetAddScore(earnScore);
            _infoPanel.SetScoreText(_score);
        }
    }

    public void DecreaseMissionAmount(ItemId itemId, int amount = 1)
    {
        foreach (BallCollectMission mission in _stageInfo.stageMissions)
        {
            if (mission.ballColor == itemId && MissionBallsCount.ContainsKey(itemId))
            {
                MissionBallsCount[itemId] -= amount;
                _infoPanel.UpdateMissionBallsAmount(itemId, MissionBallsCount[itemId]);
            }
        }
    }

    void SetRemainingTime(double time)
    {
        _remainingTime = time;
        _infoPanel.UpdateTime(time);
    }

    void SetCurrentSongTitle()
    {
        string songName = _stageInfo.bgmInfo.songName;
        string author = _stageInfo.bgmInfo.author;
        _screenParts.SetTrackTitle(author + " / " + songName);
    }


    void Update()
    {
        if (BeatCtrl.isPlaying)
        {
            SetRemainingTime(BeatCtrl.GetRemainingTime());
        }
    }
}
