using UnityEngine;
using System;
using System.Collections.Generic;
using DG.Tweening;

public class InfoPanel : InfoPanelCommon
{
    [SerializeField] TMPro.TextMeshProUGUI _scoreText;
    [SerializeField] TMPro.TextMeshProUGUI _remainingTimeText;
    [SerializeField] TMPro.TextMeshProUGUI _comboText;
    [SerializeField] ScoreAddText _scoreAddPrefab;
    [SerializeField] List<ScoreAddText> _scoreAddTextList;
    [SerializeField] Vector2 _scoreTextPosition, _scoreAddTextPosition;
    [SerializeField] int _maxLines;                // 表示上限
    [SerializeField] float _lineHeight = 40f;          // 行間
    private int _lastDisplayedSeconds = -1;

    public override void SetStageInfo(StageInfo info)
    {
        base.SetStageInfo(info);

        SetMission();
    }

    public void UpdateTime(double time)
    {
        // マイナスが来ても 00:00 で表示させる
        int timeCount = Mathf.Max(0, Mathf.CeilToInt((float)time));

        if (_lastDisplayedSeconds != timeCount)
        {
            TimeSpan formatRemainingTime = new TimeSpan(0, 0, timeCount);

            _remainingTimeText.SetText(formatRemainingTime.ToString(@"mm\:ss"));

            _lastDisplayedSeconds = timeCount;
        }
    }

    public void SetScoreText(int score)
    {
        Sequence sequence = DOTween.Sequence();
        
        _scoreText.SetText(score.ToString());

        sequence.Append(_scoreText.transform.DOLocalMoveY(_scoreTextPosition.y + 12f, 0.05f).SetEase(Ease.OutSine))
                .Append(_scoreText.transform.DOLocalMoveY(_scoreTextPosition.y, 0.2f).SetEase(Ease.InOutSine))
                .SetLink(_scoreText.gameObject);
    }

    public void SetAddScore(int score, string bonusScoreName = "")
    {
        ScoreAddText scoreAddText = Instantiate(_scoreAddPrefab, transform);
        _scoreAddTextList.Insert(0, scoreAddText);

        scoreAddText.SetDefault(this);

        scoreAddText.SetPosition(_scoreAddTextPosition + new Vector2(0f, 20f));
        scoreAddText.MovetoPosition(_scoreAddTextPosition);

        scoreAddText.AddScoreAnimation(score, bonusScoreName);

        MoveScoreAdd();
    }

    public void UpdateMissionBallsAmount(ItemId itemId, int amount)
    {
        foreach(StageSelectMission mission in _missionList)
        {
            if(mission.item == itemId)
            {
                if (amount <= 0)
                {
                    mission.CompleteAnimation();
                    return;
                }
                mission.SetAmount(amount);
                return;
            }
        }
    }

    public void MoveScoreAdd()
    {
        // まずはヌル掃除（逆順でRemove）
        for (int i = _scoreAddTextList.Count - 1; i >= 0; i--)
        {
            if (_scoreAddTextList[i] == null)
            {
                _scoreAddTextList.RemoveAt(i);
            }
        }

        Vector2 basePos = _scoreAddTextPosition;

        for (int i = 0; i < _scoreAddTextList.Count; i++)
        {
            ScoreAddText current = _scoreAddTextList[i];
            if (current == null) continue;

            // 表示上限を超えた行は順次フェードアウト
            if (i >= _maxLines)
            {
                current.FadeOut();
            }
            // 上から順に等間隔で並べる
            current.MovetoPosition(basePos - new Vector2(0f, i * _lineHeight));

        }
    }

    public void RemoveScoreAddList(ScoreAddText scoreAddText)
    {
        if (scoreAddText != null)
        {
            _scoreAddTextList.Remove(scoreAddText);
            // 抜けた分を詰める
            MoveScoreAdd();
        }
    }

    public override void ClearInfo()
    {
        base.ClearInfo();

        foreach (ScoreAddText scoreAddText in _scoreAddTextList)
        {
            if (scoreAddText != null)
            {
                Destroy(scoreAddText.gameObject);
            }
        }
        _scoreAddTextList.Clear();
    }
}
