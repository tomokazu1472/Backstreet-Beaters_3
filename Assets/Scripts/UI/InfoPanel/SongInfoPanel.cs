using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SongInfoPanel : MonoBehaviour
{
    [SerializeField] DifficulityParams _difficulityParams;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] CanvasGroup _canvasGroup;
    [SerializeField] TextMeshProUGUI _songTitle;
    [SerializeField] TextMeshProUGUI _songArtist;
    [SerializeField] TextMeshProUGUI _difficulityText;
    [SerializeField] Image _difficulityBackground;

    public void SetSongInfo(StageInfo stageInfo)
    {
        _songTitle.SetText(stageInfo.bgmInfo.songName);
        _songArtist.SetText(stageInfo.bgmInfo.author);
        _difficulityText.SetText(stageInfo.difficulity.ToString());
        _difficulityBackground.color = _difficulityParams.GetDifficulityColor(stageInfo.difficulity);
    }
    
    public void SetPosition(Vector2 pos)
    {
        rectTransform.localPosition = pos;
    }

    public void MovetoPosition(Vector2 pos)
    {
        rectTransform.DOLocalMove(pos, 0.3f).SetEase(Ease.OutCirc);
    }

    public void SetAlpha(float alpha)
    {
        _canvasGroup.alpha = alpha;
    }

    public void FadeOut(float duration)
    {
        _canvasGroup.DOFade(0, duration).SetEase(Ease.InOutSine);
    }

    public void FadeIn(float duration)
    {
        _canvasGroup.DOFade(1, duration).SetEase(Ease.InOutSine);
    }

}
