using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BoostSelect : MonoBehaviour
{
    [SerializeField] GameProgress _gameProgress;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] CanvasGroup _canvasGroup;
    [SerializeField] List<BoostSelectButton> _boostButtons;
    [SerializeField] ButtonBase _confirmButton;
    [SerializeField] bool _isAbleToConfirm;
    public bool isAbleToConfirm => _isAbleToConfirm;

    public void SetBoostSelectPanel()
    {
        foreach (BoostSelectButton boostButton in _boostButtons)
        {
            boostButton.InitButton();
            boostButton.SetSelected(false);
        }

        //選択するまでの間決定できないようにする
        _isAbleToConfirm = false;

        _confirmButton.InitButton();
        _confirmButton.SetButtonSelectable(false);
        (_confirmButton as VanillaButton).SetButtonText("どれか一つを選択");
    }

    public void OnBoostSelectButtonPressed(ButtonBase boostButton)
    {
        foreach (BoostSelectButton button in _boostButtons)
        {
            if (button == boostButton)
            {
                button.SetSelected(true);
                _isAbleToConfirm = true;
                _confirmButton.SetButtonSelectable(true);
                (_confirmButton as VanillaButton).SetButtonText("これでOK!");
            }
            else
            {
                button.SetSelected(false);
            }
        }
    }

    public void ConfirmButtonPressed()
    {
        if (_isAbleToConfirm)
        {
            FadePanel(0f, 0.5f);
            _gameProgress.StartGamePlay();
        }
    }

    public void FadePanel(float targetAlpha, float duration)
    {
        _canvasGroup.DOFade(targetAlpha, duration).SetEase(Ease.OutSine);
    }

    public void SetAlpha(float alpha)
    {
        _canvasGroup.alpha = alpha;
    }
}
