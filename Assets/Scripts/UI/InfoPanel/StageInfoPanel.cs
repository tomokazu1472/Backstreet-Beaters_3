using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageInfoPanel : InfoPanelCommon
{
    [SerializeField] DifficulityParams _difficulityParams;
    [SerializeField] TextMeshProUGUI _songTitle;
    [SerializeField] TextMeshProUGUI _authorTitle;
    [SerializeField] TextMeshProUGUI _bpmText;
    [SerializeField] TextMeshProUGUI _highScoreText;
    [SerializeField] TextMeshProUGUI _nothingText;
    [SerializeField] TextMeshProUGUI _difficulityText;
    [SerializeField] Image _difficulityBackground;
    [SerializeField] ButtonBase _gameStartButton;
    [SerializeField] Vector2 _ChangedAnimationScale;
    [SerializeField] float _ChangedAnimationDuration;
    private Sequence _sequence;

    public override void SetStageInfo(StageInfo info)
    {
        //何も表示できないとき
        if (info == null)
        {
            _canvasGroup.alpha = 0f;
            _nothingText.gameObject.SetActive(true);
            return;
        }

        base.SetStageInfo(info);


        _canvasGroup.alpha = 1f;
        _nothingText.gameObject.SetActive(false);

        //それぞれもらった情報を入れる
        _songTitle.SetText(_stageInfo.bgmInfo.songName);
        _authorTitle.SetText($"作者 : {_stageInfo.bgmInfo.author}");
        _bpmText.SetText($"BPM : {_stageInfo.bgmInfo.BPM}");
        _gameStartButton.InitButton();
        _highScoreText.SetText($"ハイスコア : {PlayerPrefs.GetInt(_stageInfo.stageName, 0)}");

        _difficulityBackground.color = _difficulityParams.GetDifficulityColor(info.difficulity);
        _difficulityText.SetText(info.difficulity.ToString());

        SetMission();
    }
    
    public void InfoChangedAnimation()
    {
        _sequence?.Kill(true);
        _sequence = DOTween.Sequence();

        _sequence.Append(rectTransform.DOPunchScale(_ChangedAnimationScale, _ChangedAnimationDuration));
    }

    public void GameStartButtonPressed()
    {
        BeatCtrl.ClearBGMInfo();
        GameInfo.SetStageInfo(_stageInfo);
        SceneChange.ChangeScene("GamePlay");
    }
}
