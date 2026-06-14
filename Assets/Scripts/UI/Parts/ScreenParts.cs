using UnityEngine;
using TMPro;
using DG.Tweening;

public class ScreenParts : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _trackTitleText;
    [SerializeField] TextMeshProUGUI _countDownText;
    public void SetTrackTitle(string text)
    {
        _trackTitleText.SetText(text);
    }

    public void SetCountDownText(string text)
    {
        _countDownText.SetText(text);
    }

    public void FadeOutCountDownText()
    {
        _countDownText.DOFade(0, 0.4f)
        .SetEase(Ease.OutQuad)
        .SetDelay(0.3f)
        .SetLink(_countDownText.gameObject);
    }

    public void FadeInCountDownText()
    {
        _countDownText.alpha = 0;
        _countDownText.DOFade(1, 0.3f)
        .SetEase(Ease.InQuad)
        .SetLink(_countDownText.gameObject);
    }

    public void DisplayCountDownText()
    {
        _countDownText.alpha = 1;
    }

    public void HideCountDownText()
    {
        _countDownText.alpha = 0;
    }

}
