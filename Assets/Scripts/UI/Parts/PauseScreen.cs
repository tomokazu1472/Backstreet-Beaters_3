using UnityEngine;
using System.Collections.Generic;

public class PauseScreen : MonoBehaviour
{
    [SerializeField] PauseButton _pauseButton;
    [SerializeField] CommonPanel _pauseScreen;
    [SerializeField] List<ButtonBase> _pauseScreenButton;

    void Start()
    {
        _pauseButton.InitButton();
    }
    
    public void ShowPauseMenu()
    {
        _pauseButton.SetPauseIcon();
        _pauseScreen.SetAlpha(0.5f);
        _pauseScreen.gameObject.SetActive(true);

        foreach (ButtonBase button in _pauseScreenButton)
        {
            button.gameObject.SetActive(true);
            button.InitButton();
        }
    }

    public void HidePauseMenu()
    {
        _pauseButton.SetResumeIcon();
        _pauseScreen.SetAlpha(0f);
        _pauseScreen.gameObject.SetActive(false);

        foreach (ButtonBase button in _pauseScreenButton)
        {
            button.gameObject.SetActive(false);
        }

    }

    public void GameQuitButtonPressed()
    {
        BeatCtrl.ClearBGMInfo();
    }

}
