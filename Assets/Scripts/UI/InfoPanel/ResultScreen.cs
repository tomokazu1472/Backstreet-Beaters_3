using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultScreen : InfoPanelCommon
{
    [SerializeField] GameBeatAction _gameBeatAction;
    [SerializeField] GameInfo _gameInfo;
    [SerializeField] BallField _ballField;
    [SerializeField] Camera _mainCamera;
    [SerializeField] TextMeshProUGUI _resultTitleText, _scoreText, _rateText, _scoreTitleText;
    [SerializeField] Image _rankBackGround;
    [SerializeField] ButtonBase _pauseButton, _gameFinishButton;
    [SerializeField] ColorClock _colorClock;
    [SerializeField, Header("アニメーション設定")] Vector2 _panelEnterPos;
    [SerializeField] Vector2 _panelDefaultPos;
    [SerializeField] Vector3 _cameraDefaultPos;
    [SerializeField] Vector3 _cameraShiftPos;
    [SerializeField] float _colorClockExitPosX, _colorClockPosX;
    [SerializeField] float _pauseButtonExitPos, _pauseButtonPos;
    [SerializeField] float _rateTextPosX;
    [SerializeField] float _panelEnterDuration;
    [SerializeField] float _enterDuration;
    [SerializeField] float _cameraEnterDuration;
    [SerializeField] float _scoreCountDuration;

    public override void SetStageInfo(StageInfo info)
    {
        base.SetStageInfo(info);

        SetMission();
    }

    public void HideResultScreen()
    {
        rectTransform.localPosition = _panelEnterPos;
        _mainCamera.transform.position = _cameraDefaultPos;
        SetAlpha(0f);
    }

    public void ShowResultScreen()
    {
        HideResultScreen();
        SetAlpha(1f);
        _gameFinishButton.InitButton();
        _gameFinishButton.SetButtonSelectable(false);
        _resultTitleText.alpha = 0f;

        Sequence sequence = DOTween.Sequence();

        _scoreText.SetText("0");

        sequence.Append(rectTransform.DOLocalMove(_panelDefaultPos, _panelEnterDuration).SetEase(Ease.OutCubic))
                .Join(_mainCamera.transform.DOMove(_cameraShiftPos, _cameraEnterDuration).SetEase(Ease.InOutQuad))
                .Join(_colorClock.rectTransform.DOLocalMoveX(_colorClockExitPosX, _cameraEnterDuration).SetEase(Ease.InQuad))
                .Join(_pauseButton.rectTransform.DOLocalMoveX(_colorClockExitPosX, _cameraEnterDuration).SetEase(Ease.InQuad))
                .Append(_resultTitleText.DOFade(1f, _enterDuration).SetEase(Ease.OutCubic))
                .Join(_resultTitleText.rectTransform.DOLocalMoveX(-63f, _enterDuration).From(_rateTextPosX).SetEase(Ease.OutCubic))
                .AppendInterval(0.6f)
                .AppendCallback(() =>
                {
                    _ballField.ActivateAllItems();
                    StartCoroutine(GameFinishBonusAction());
                });

    }

    void UpdateScore()
    {
        Sequence sequence = DOTween.Sequence();

        int index = 0; // アニメーションの遅延を管理するためのカウンタ

        //パネル側に記録されているミッション
        foreach (StageSelectMission mission in _missionList)
        {
            //gameinfoのミッションを探す
            foreach (KeyValuePair<ItemId, int> pair in _gameInfo.MissionBallsCount)
            {
                Sequence subSequence = DOTween.Sequence();

                if (mission.item == pair.Key)
                {
                    subSequence.Append(mission.gameObject.transform.DOScale(new Vector3(1.3f, 1.3f, 1f), 0.2f).SetEase(Ease.OutBack));
                    //減らす数字
                    int targetAmount = _gameInfo.MissionBallsCount[mission.item];
                    //ミッションの数を減らすアニメーション
                    subSequence.Append(DOVirtual.Int(mission.amount, Mathf.Max(0, targetAmount), 0.5f, (value) =>
                    {
                        mission.SetAmount(value);

                    }).SetEase(Ease.Linear));

                    if (targetAmount <= 0)
                    {
                        subSequence.AppendCallback(() =>
                        {
                            mission.CompleteAnimation();
                        });
                    }

                    subSequence.Append(mission.gameObject.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InCubic));
                }

                sequence.Insert(0.2f * index, subSequence).SetLink(gameObject);
                index++;
            }
        }
        sequence.Append(DOVirtual.Int(0, _gameInfo.score, _scoreCountDuration, (value) =>
                {
                    _scoreText.SetText($"{value}");
                }).SetEase(Ease.InOutQuad))
                .AppendCallback(() =>
                {
                    _gameFinishButton.SetButtonSelectable(true);
                    SaveScore();
                });

    }
    
    void SaveScore()
    {
        int currentHighScore = PlayerPrefs.GetInt(_stageInfo.stageName, 0);
        
        if (currentHighScore < _gameInfo.score)
        {
            PlayerPrefs.SetInt(_stageInfo.stageName, _gameInfo.score);
            PlayerPrefs.Save();
        }
    }
    
    IEnumerator GameFinishBonusAction()
    {
        while (_gameBeatAction.TryActivateItems())
        {
            yield return new WaitForSeconds(BeatCtrl.GetOneBeatTime());
        }
        //ごり押しでアイテムを消す
        while (_gameBeatAction.TryActivateItems())
        {
            yield return new WaitForSeconds(BeatCtrl.GetOneBeatTime());
        }
        UpdateScore();
    }
}
