using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountDownCtrl : MonoBehaviour
{
    [SerializeField] Gamepause _gamepause;
    [SerializeField] BallSwap _ballSwap;
    [SerializeField] ScreenParts _screenParts;
    [SerializeField] StartBonus _startBonus;
    [SerializeField] Image _countDownImage;
    [SerializeField] RectTransform _countDownTransform;
    [SerializeField] TextMeshProUGUI _countDownText;
    [SerializeField] List<Sprite> _CountSprites;

    void Start()
    {
        Init();
    }
    
    public void Init()
    {
        _countDownImage.color = new Color(1, 1, 1, 0);
    }
    
    public void CountDownStart(int CountDownStart)
    {
        int countdownTime = CountDownStart;
        float oneBeatTime = BeatCtrl.GetOneBeatTime();
        _screenParts.HideCountDownText();

        void CountDownStep()
        {
            if (GameMenu.currentMenu != MenuName.Gameplay)
            {
                return; // ポーズ中に戻された場合、カウントダウンを中止
            }
            if (countdownTime > 0)
            {
                Sequence sequence = DOTween.Sequence();
    
                if ( _CountSprites.Count >= countdownTime) _countDownImage.sprite = _CountSprites[countdownTime];
                _countDownImage.transform.localScale = new Vector3(2f, 2f, 1f);
                _countDownImage.color = new Color(1, 1, 1, 0);

                sequence.Append(_countDownImage.transform.DOScale(Vector3.one, BeatCtrl.GetOneBeatTime() * 0.8f).SetEase(Ease.InCubic))
                        .Join(_countDownImage.DOFade(1f, BeatCtrl.GetOneBeatTime() * 0.7f))
                        .SetLink(_countDownImage.gameObject);

                countdownTime--;

                DOVirtual.DelayedCall(oneBeatTime, CountDownStep); // 再帰的に呼び出し
            }
            else
            {
                //GO!
                _countDownImage.sprite = _CountSprites[0];
                _countDownImage.DOFade(0, 0.3f).SetDelay(0.2f);
                _gamepause.SetGamePlaying(true);
                _startBonus.StartCount();
                _ballSwap.swapLock = false;
            }
        }
        CountDownStep(); // カウントダウン開始
    }

    
}
