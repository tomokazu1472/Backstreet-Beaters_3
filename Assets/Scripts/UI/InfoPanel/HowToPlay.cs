using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HowToPlay : MonoBehaviour
{
    [SerializeField] RectTransform _discription;
    [SerializeField] GameProgress _gameProgress;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] CanvasGroup _canvasGroup;
    [SerializeField] Image _discriptionImage;
    [SerializeField] TextMeshProUGUI _pageNumText;
    [SerializeField] Sprite[] _imageList;
    [SerializeField] VanillaButton _nextButton, _backButton;
    private int _pageNum;
    private Sequence _sequence;

    public void Init()
    {
        _pageNum = 0;
        _discriptionImage.sprite = _imageList[_pageNum];
        
        _pageNumText.SetText($"{_pageNum + 1}/{_imageList.Length}");

        _nextButton.InitButton();
        _backButton.InitButton();
    }

    public void MoveToNext()
    {
        if (_pageNum >= _imageList.Length - 1)
        {
            _sequence = DOTween.Sequence();

            _sequence.AppendCallback(() =>
            {
                FadeOut(0.4f);
            })
            .AppendInterval(0.2f)
            .AppendCallback(() =>
            {
                _gameProgress.ShowBoostSelect();
                gameObject.SetActive(false);
            })
            .SetLink(gameObject);
        }
        GotoPage(_pageNum + 1);
    }

    public void MoveToPrevious()
    {
        GotoPage(_pageNum - 1);
    }

    void GotoPage(int page)
    {
        if (page < 0 || page > _imageList.Length - 1) return;

        _pageNum = page;
        _discriptionImage.sprite = _imageList[page];
        _pageNumText.SetText($"{_pageNum + 1}/{_imageList.Length}");

        _nextButton.InitButton();
        _backButton.InitButton();

        _backButton.gameObject.SetActive(_pageNum > 0);
        
        if (page >= _imageList.Length - 1)
        {
            _nextButton.SetButtonText("わかった！");
        }
        else
        {
            _nextButton.SetButtonText("つぎへ");
        }
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
