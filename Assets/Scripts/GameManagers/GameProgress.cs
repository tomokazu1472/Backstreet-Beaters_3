using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameProgress : MonoBehaviour
{
    [SerializeField, Header("スクリプト")] SongInfoPanel _songInfoPanel;
    [SerializeField] MissionWindow _missionWindow;
    [SerializeField] ScreenParts _screenParts;
    [SerializeField] PauseScreen _pauseScreen;
    [SerializeField] HowToPlay _howToPlay;
    [SerializeField] BoostSelect _boostSelect;
    [SerializeField] ResultScreen _resultScreen;
    [SerializeField] GameBeatAction _gameBeatAction;
    [SerializeField] BallSwap _ballSwap;
    [SerializeField] CommonPanel _blackScreen, _windowEmphasisPanel;
    [SerializeField, Header("アニメーション設定")] float _songInfoTime;
    [SerializeField] float _startTransitonfadeInTime;
    [SerializeField] float _songInfoFadeTime;
    [SerializeField] float _missionInfoFadeTime;
    [SerializeField, Range(0f, 1f), Tooltip("ミッションの表示時間率（ミッション：カウントダウン準備）")] float _missionDisplayTime;
    public void GameStart()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.AppendCallback(() =>
                {
                    _pauseScreen.HidePauseMenu();
                    _screenParts.HideCountDownText();

                    _blackScreen.SetAlpha(1f);
                    _blackScreen.FadePanel(0f, _startTransitonfadeInTime);

                    _songInfoPanel.SetAlpha(0f);
                    _songInfoPanel.FadeIn(_songInfoFadeTime);

                    _missionWindow.SetAlpha(0f);
                    _windowEmphasisPanel.SetAlpha(0.4f);

                    _howToPlay.SetAlpha(0f);
                    _howToPlay.Init();
                    _howToPlay.gameObject.SetActive(false);

                    _resultScreen.HideResultScreen();
                    _boostSelect.SetAlpha(0f);
                    _boostSelect.gameObject.SetActive(false);
                })
                .AppendInterval(1f)
                .AppendCallback(() =>
                {
                    BeatCtrl.StartBGM();
                })
                .AppendInterval(_songInfoTime)
                .AppendCallback(() =>
                {
                    _songInfoPanel.FadeOut(_songInfoFadeTime);
                    _howToPlay.gameObject.SetActive(true);
                    _howToPlay.FadeIn(0.4f);
                });
    }

    public void ShowBoostSelect()
    {
        _boostSelect.gameObject.SetActive(true);
        _boostSelect.FadePanel(1f, _songInfoFadeTime);
    }

    public void StartGamePlay()
    {
        BeatCtrl.SetBGMLoop(false);

        Sequence sequence = DOTween.Sequence();

        sequence.AppendCallback(() =>
        {
            _missionWindow.FadeIn(_missionInfoFadeTime);
        })
        .AppendCallback(() =>
        {
            _boostSelect.gameObject.SetActive(false);
        })
        .AppendInterval(3f)
        .AppendCallback(() =>
        {
            _missionWindow.FadeOut(_missionInfoFadeTime);
            _windowEmphasisPanel.FadePanel(0f, _missionInfoFadeTime);
            _screenParts.FadeInCountDownText();
            _screenParts.SetCountDownText("Ready?");
        });
    }

    public void GameFinish()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.AppendCallback(() =>
                {
                    _screenParts.DisplayCountDownText();
                    _screenParts.SetCountDownText("Finish!");
                    _gameBeatAction.isGameFinished = true;
                    _ballSwap.swapLock = true;
                })
                .AppendInterval(3f)
                .AppendCallback(() =>
                {
                    _screenParts.FadeOutCountDownText();
                    _resultScreen.ShowResultScreen();
                });
    }
    

    void Update()
    {
        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            Debug.Log("switch");
            StartGamePlay();
        }
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            Debug.Log("finish");
            GameFinish();
        }
    }
}
