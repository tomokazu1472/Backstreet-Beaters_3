using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Gamepause : MonoBehaviour
{
    [SerializeField] BallSwap _ballSwap;
    [SerializeField] GameProgress _gameProgress;
    [SerializeField] ScreenParts _screenParts;
    [SerializeField] GameReset _gameReset;
    [SerializeField] PauseScreen _pauseScreen;
    [SerializeField] CountDownCtrl _countDownCtrl;
    static bool _isGamePlaying;
    public static bool isGamePlaying => _isGamePlaying;

    void Start()
    {
        GameMenu.AddInitAction(() =>
        {
            InitializeGame();
        });

        BeatAction.AddBeatAction(() =>
        {
            CheckCountDown();
        });

        GameMenu.InitMenu();
    }

    public void ReloadGame()
    {
        SceneChange.ChangeScene("GamePlay");
    }

    void InitializeGame()
    {
        AppSystem.SetTimeScale(1f);

        SetGamePlaying(false);
        _ballSwap.swapLock = true;

        _gameReset.ResetGame();

        _gameProgress.GameStart();
    }

    public void PauseButtonPressed()
    {
        if (GameMenu.currentMenu == MenuName.Gameplay)
        {
            PauseGame();
        }
        else if (GameMenu.currentMenu == MenuName.Gamepause)
        {
            ResumeGame();
        }
    }

    public void ResumeGame()
    {
        _pauseScreen.HidePauseMenu();
        CountdownToResume();
    }

    public void SetGamePlaying(bool value)
    {
        _isGamePlaying = value;
    }
    
    public void PauseGame()
    {
        Debug.Log("Pause");
        _ballSwap.swapLock = true;
        BeatCtrl.PauseBGM();
        _pauseScreen.ShowPauseMenu();

        //ザ・ワールド
        AppSystem.SetTimeScale(0f);
    }

    void CheckCountDown()
    {
        if (GameInfo.stageInfo.bgmInfo.gameFinishBeat ==
        BeatCtrl.BeatCount) {
            Debug.Log("CountDownFinish" + BeatCtrl.BeatCount);
            _gameProgress.GameFinish();
        }
        else if (GameInfo.stageInfo.bgmInfo.gameStartBeat ==
        BeatCtrl.BeatCount + GameInfo.stageInfo.bgmInfo.countDownBeat) {
            Debug.Log("CountDownStart" + BeatCtrl.BeatCount);
            _countDownCtrl.CountDownStart(GameInfo.stageInfo.bgmInfo.countDownBeat);
        }
    }

    void CountdownToResume()
    {
        float oneBeatTime = BeatCtrl.GetOneBeatTime();
        int countdownTime = 3;
        _screenParts.DisplayCountDownText();

        void CountDownStep()
        {
            if (GameMenu.currentMenu != MenuName.Gameplay)
            {
                return; // ポーズ中に戻された場合、カウントダウンを中止
            }

            if (countdownTime > 0)
            {
                _screenParts.SetCountDownText(countdownTime.ToString());
                countdownTime--;

                DOVirtual.DelayedCall(oneBeatTime, CountDownStep); // 再帰的に呼び出し
            }
            else
            {
                _screenParts.SetCountDownText("Go!");
                _screenParts.FadeOutCountDownText();

                DOVirtual.DelayedCall(oneBeatTime, () =>
                {
                    // ゲームを動かす
                    AppSystem.SetTimeScale(1f);
                    BeatCtrl.ResumeBGM();

                    if (_isGamePlaying) _ballSwap.swapLock = false;
                });
            }
        }

        CountDownStep(); // カウントダウン開始
    }

    public void GameTimeUp()
    {
        _ballSwap.swapLock = true;
        SetGamePlaying(false);
        _screenParts.DisplayCountDownText();
        _screenParts.SetCountDownText("Time Up!");
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            PauseButtonPressed();
        }
    }
}
