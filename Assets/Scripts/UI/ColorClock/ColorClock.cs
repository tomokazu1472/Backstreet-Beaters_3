using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;

public class ColorClock : MonoBehaviour
{
    [SerializeField] public RectTransform rectTransform;
    [SerializeField] ColorClockHand _colorClockHand;
    [SerializeField] BallWeight _ballWeight;
    [SerializeField] TextMeshProUGUI _discriptionText;
    [SerializeField, Header("何拍で一回転か")] private int _beatsPerTurn = 4;
    [SerializeField, Header("回転方向（時計回りなら true）")] private bool _clockwise = true;
    [SerializeField] private bool _pauseHand;
    [SerializeField] List<RectTransform> _colorImages;
    [SerializeField] List<Color> _textColors;
    [SerializeField] int _targetWeight = 30;
    bool _isBonus;
    Ease _rotationEase = Ease.Linear;

    // 内部
    private float _baseAngleZ;

    public void PauseHand(bool isPause)
    {
        _pauseHand = isPause;
    }

    public void SetTargetWeight(int amount, bool bonus)
    {
        _targetWeight = amount;
        _isBonus = bonus;
    }

    public void RotateToNextBeatAngle()
    {
        // 現在の拍数に基づく“正規の目標角”を計算してスナップ誤差を防ぐ
        int stepIndex = BeatCtrl.BeatCount % _beatsPerTurn;     // 0,1,2,3...
        float stepDeg = 360f / _beatsPerTurn;                   // 4拍→90°
        float dir = _clockwise ? -1f : 1f;                      // 2Dで時計回りは -Z

        float targetZ = _baseAngleZ + dir * (stepIndex * stepDeg);
        float duration = Mathf.Max(0.0001f, BeatCtrl.GetOneBeatTime());

        if (_pauseHand)
        {
            _colorClockHand.DOPause();
            _rotationEase = Ease.OutBack;
            return;
        }

        PopColorImage(targetZ);

        Sequence sequence = DOTween.Sequence();

        // 線形で1拍ぶんかけて次の角度へ
        sequence.Append(_colorClockHand.rectTransform.DOLocalRotate(
                new Vector3(_colorClockHand.rectTransform.localEulerAngles.x, _colorClockHand.rectTransform.localEulerAngles.y, targetZ),
                duration, RotateMode.Fast).SetEase(_rotationEase));

        _rotationEase = Ease.Linear;
    }

    void PopColorImage(float rotateZ)
    {
        //今針が刺している色を指定
        int currentColorIndex = (int)(-rotateZ / 360 * _colorImages.Count);
        SetWeight(currentColorIndex);

        //Debug.Log(currentColorIndex + " : " + rotateZ);
        RectTransform currentColor = _colorImages[currentColorIndex];

        currentColor.DOScale(1.2f, 0.3f).SetEase(Ease.OutBack)
                .SetUpdate(true);

        //残りを基の位置に戻す
        foreach (RectTransform rectTransform in _colorImages)
        {
            if (rectTransform != currentColor)
            {
                rectTransform.DOScale(1.0f, 0.2f).SetEase(Ease.OutCirc);
            }
        }
    }

    void SetWeight(int index)
    {
        _ballWeight.SetDefaultWeight();
        _ballWeight.ChangeBallWeight((ItemId)index, _targetWeight);

        string colorName;
        switch (index) {
            case 0:
                colorName = "赤色";
                break;
            case 1:
                colorName = "青色";
                break;
            case 2:
                colorName = "緑色";
                break;
            case 3:
                colorName = "黄色";
                break;
            default:
                colorName = "？";
                break;
        }
        Color textColor = _textColors[index];
        string targetWord = colorName;
        string coloredWord = $"<color=#{ColorUtility.ToHtmlStringRGB(textColor)}>{targetWord}</color>";

        if (_isBonus)
        {
            _discriptionText.text = $"{coloredWord}の\n出現確率超UP!!";
        }
        else
        {
            _discriptionText.text = $"{coloredWord}の\n出現確率UP!!";
        }
    }
}
