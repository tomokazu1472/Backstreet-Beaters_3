using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;

public class TitleScreen : MonoBehaviour
{
    [SerializeField, Header("スクリプト")] Image _titleLogoImage;
    [SerializeField] TitleSpeaker _LSpeaker, _RSpeaker;
    [SerializeField] List<ButtonBase> _titleButtons;
    [SerializeField] Image _whitePanel, _fadeOutBlackPanel, _openingBlackPanel;
    [SerializeField] ButtonBase _backToTitleButton;
    [SerializeField] RectTransform _titleLogo, _titleBackground;
    [SerializeField, Header("アニメーション設定")] AnimationCurve _buttonVibeCurve, _logoVibeCurve;
    [SerializeField] Vector2 _LspeakerPos, _RspeakerPos;
    [SerializeField] float _openingAnimationDelay;
    [SerializeField] float _transitionEndPositionX = -1500;
    [SerializeField] float _speakerEnterDuration, _speakerExitDuration;
    bool _isButtonMoving = false;


    public void OpeningAnimation()
    {
        Sequence sequence = DOTween.Sequence();

        _fadeOutBlackPanel.color = new Color(0f, 0f, 0f, 0f);
        _openingBlackPanel.color = new Color(0f, 0f, 0f, 1f);
        _whitePanel.color = new Color(1f, 1f, 1f, 0f);

        _titleLogo.gameObject.SetActive(true);
        _titleLogoImage.color = new Color(1f, 1f, 1f, 0f);
        SetActiveTitleButton(false);
        _backToTitleButton.gameObject.SetActive(false);

        sequence.AppendInterval(_openingAnimationDelay)
                .AppendCallback(() =>
                {
                    BeatCtrl.StartBGM();
                })
                .AppendInterval(0.2f)
                .Append(_titleLogo.DOScale(new Vector3(1f, 1f, 1f), 1f).From(3f).SetEase(Ease.InCubic))
                .Join(_titleLogoImage.DOFade(1f, 0.8f).From(0f).SetEase(Ease.InCubic))
                .AppendCallback(() =>
                {
                    _whitePanel.color = new Color(1f, 1f, 1f, 1f);
                    _openingBlackPanel.color = new Color(0f, 0f, 0f, 0f);
                })
                .AppendInterval(0.3f)
                .Append(_whitePanel.DOFade(0f, 2.0f))
                .JoinCallback(() =>
                {
                    BackToTitleSpeakerTransition();
                    BackToTitleButtonTransition();
                })
                .SetLink(gameObject).SetUpdate(true);

    }

    public void GoToTitleButtonPressed()
    {
        BackToTitleButtonTransition();
        BackToTitleLogoTransition();
        BackToTitleSpeakerTransition();
        _backToTitleButton.gameObject.SetActive(false);
    }

    void SetActiveTitleButton(bool active)
    {
        foreach (ButtonBase button in _titleButtons)
        {
            button.gameObject.SetActive(active);
        }
    }

    public void VibeTitleScreen()
    {
        if (BeatCtrl.BeatCount % 2 == 0 && !BeatCtrl.isMuteBeat)
        {
            VibeTitleButton();
            _LSpeaker.VibeSpeaker();
            _RSpeaker.VibeSpeaker();
        }
        else if (BeatCtrl.BeatCount % 2 == 1 && !BeatCtrl.isMuteBeat)
        {
            VibeLogo();
            _LSpeaker.VibeSpeaker();
            _RSpeaker.VibeSpeaker();
        }
    }
    void VibeLogo()
    {
        DOTween.Complete(_titleLogoImage);
        _titleLogoImage.rectTransform.DOScale(new Vector3(1.1f, 1.1f, 1f), 0.3f)
            .SetLink(gameObject)
            .SetEase(_logoVibeCurve)
            .OnComplete(() => {
                _titleLogoImage.rectTransform.localScale = new Vector3(1f, 1f, 1f);
            });
    }

    void VibeTitleButton()
    {
        if (_isButtonMoving) return;

        Sequence sequence = DOTween.Sequence();
        foreach (ButtonBase button in _titleButtons)
        {
            sequence.Join(button.rectTransform.DOScale(new Vector3(1.05f, 1.05f, 1f), 0.3f).SetEase(_buttonVibeCurve)
                .OnComplete(() =>
                {
                    button.rectTransform.localScale = new Vector3(1f, 1f, 1f);
                }));
        }
    }
    void BackToTitleLogoTransition()
    {
        Sequence sequence = DOTween.Sequence();

        _titleLogo.gameObject.SetActive(true);
        _titleBackground.gameObject.SetActive(true);
        _titleLogo.localPosition = new Vector2(_transitionEndPositionX, _titleLogo.localPosition.y);
        _titleBackground.localPosition = new Vector2(_transitionEndPositionX, _titleLogo.localPosition.y);

        sequence.Append(_titleLogo.DOLocalMove(new Vector2(0f, _titleLogo.localPosition.y), 0.2f).SetEase(Ease.InCubic))
                .Join(_titleBackground.DOLocalMove(new Vector2(0f, _titleLogo.localPosition.y), 0.2f).SetEase(Ease.InCubic))
                .Append(_titleLogo.DOScaleX(0.7f, 0.05f).SetEase(Ease.OutQuad))
                .Join(_titleBackground.DOScaleX(0.7f, 0.05f).SetEase(Ease.OutQuad))
                .Join(_titleLogo.DOLocalMoveX(120f, 0.05f).SetEase(Ease.OutQuad))
                .Join(_titleBackground.DOLocalMoveX(120f, 0.05f).SetEase(Ease.OutQuad))
                .Append(_titleLogo.DOScaleX(1.0f, 0.2f).SetEase(Ease.OutExpo))
                .Join(_titleBackground.DOScaleX(1.0f, 0.2f).SetEase(Ease.OutExpo))
                .Join(_titleLogo.DOLocalMoveX(0f, 0.2f).SetEase(Ease.OutExpo))
                .Join(_titleBackground.DOLocalMoveX(0f, 0.2f).SetEase(Ease.OutExpo))
                .SetLink(gameObject).SetUpdate(true);
    }
    
    void BackToTitleButtonTransition()
    {
        Sequence sequence = DOTween.Sequence();
        _isButtonMoving = true;

        // ボタンを初期位置に設定
        foreach (ButtonBase button in _titleButtons)
        {
            button.gameObject.SetActive(true);
            button.InitButton();
            button.rectTransform.localPosition = new Vector2(_transitionEndPositionX, button.rectTransform.localPosition.y);
        }

        int index = 0; // アニメーションの遅延を管理するためのカウンタ
        foreach (ButtonBase button in _titleButtons.AsEnumerable().Reverse())
        {
            //画面外から登場してくるアニメーション
            Sequence subSequence = DOTween.Sequence();
            subSequence.Append(button.rectTransform.DOLocalMove(new Vector2(0f, button.rectTransform.localPosition.y), 0.2f).SetEase(Ease.InCubic))
                    .Append(button.rectTransform.DOScaleX(0.7f, 0.05f).SetEase(Ease.OutQuad))
                    .Join(button.rectTransform.DOLocalMoveX(120f, 0.05f).SetEase(Ease.OutQuad))
                    .Append(button.rectTransform.DOScaleX(1.0f, 0.2f).SetEase(Ease.OutExpo))
                    .Join(button.rectTransform.DOLocalMoveX(0f, 0.2f).SetEase(Ease.OutExpo))
                    .SetLink(gameObject).SetUpdate(true);

            //各ボタンのアニメーションを少しずつ遅らせて実行
            sequence.Insert(0.1f * index, subSequence);

            sequence.OnComplete(() =>
            {
                _isButtonMoving = false;
            });
            index++;
        }
    }
    void BackToTitleSpeakerTransition()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(_LSpeaker.transform.DOLocalMove(_LspeakerPos, _speakerEnterDuration).From(new Vector2(_transitionEndPositionX, _LspeakerPos.y)).SetEase(Ease.OutCubic))
                .Join(_RSpeaker.transform.DOLocalMove(_RspeakerPos, _speakerEnterDuration).From(new Vector2(_transitionEndPositionX, _RspeakerPos.y)).SetEase(Ease.OutCubic))
                .SetLink(gameObject).SetUpdate(true);
    }

    public void StartGameTransition()
    {
        _backToTitleButton.gameObject.SetActive(true);
        _backToTitleButton.InitButton();
        Sequence sequence = DOTween.Sequence();
        _isButtonMoving = true;

        sequence.Append(_titleLogo.DOLocalMoveX(_transitionEndPositionX, 0.2f).SetEase(Ease.InSine))
                .Join(_titleBackground.DOLocalMoveX(_transitionEndPositionX, 0.2f).SetEase(Ease.InSine))
                .Join(_LSpeaker.transform.DOLocalMoveX(_transitionEndPositionX, _speakerExitDuration).SetEase(Ease.InCubic))
                .Join(_RSpeaker.transform.DOLocalMoveX(_transitionEndPositionX, _speakerExitDuration).SetDelay(0.2f).SetEase(Ease.InCubic))
                .SetLink(gameObject).SetUpdate(true);

        // ボタンを逆順に移動先の位置にアニメーション
        int index = 0; // アニメーションの遅延を管理するためのカウンタ
        foreach (ButtonBase button in _titleButtons.AsEnumerable())
        {
            Sequence subSequence = DOTween.Sequence();
            subSequence.Append(button.rectTransform.DOLocalMoveX(_transitionEndPositionX, 0.2f).SetEase(Ease.InSine))
                    .AppendCallback(() =>
                    {
                        button.gameObject.SetActive(false);
                        _titleLogo.gameObject.SetActive(false);
                        _titleBackground.gameObject.SetActive(false);
                    });
                    
            sequence.Insert(0.1f * index, subSequence).SetLink(gameObject).SetUpdate(true);
            index++;
        }

        // 全てのアニメーションが完了した後にコールバックを実行
        sequence.OnComplete(() =>
        {
            _isButtonMoving = false;
        });
    }

    void Update()
    {
        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            BackToTitleLogoTransition();
            BackToTitleButtonTransition();
        }
    }
}
