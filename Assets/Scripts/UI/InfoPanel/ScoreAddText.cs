using UnityEngine;
using DG.Tweening;

public class ScoreAddText : MonoBehaviour
{
    [SerializeField] RectTransform _rectTransform;
    [SerializeField] TMPro.TextMeshProUGUI _scoreAddText;
    InfoPanel _infoPanel;
    Vector2 _targetPosition;
    Sequence _sequence;
    private bool _isFadingOut;

    public void SetDefault(InfoPanel infoPanel)
    {
        _infoPanel = infoPanel;
        _isFadingOut = false;
        // 既存シーケンスが残っていれば念のためKill
        if (_sequence != null && _sequence.IsActive()) {
            _sequence.Kill();
            _sequence = null;
        }
    }

    public void SetPosition(Vector2 position)
    {
        _rectTransform.anchoredPosition = position;
    }

    public void MovetoPosition(Vector2 position)
    {
        _targetPosition = position;
        _rectTransform.DOAnchorPos(_targetPosition, 0.3f, true)
            .SetLink(gameObject)
            .SetEase(Ease.OutCubic);
    }

    public void AddScoreAnimation(int score, string bonusScoreName = "")
    {
        // 既存があれば破棄（多重Append防止）
        if (_sequence != null && _sequence.IsActive()) {
            _sequence.Kill();
        }
        _isFadingOut = false;

        _sequence = DOTween.Sequence().SetLink(gameObject);

        _scoreAddText.SetText(string.IsNullOrEmpty(bonusScoreName)
            ? $" +{score}"
            : $"{bonusScoreName} +{score}");

        _scoreAddText.alpha = 0f;

        _sequence.Append(_scoreAddText.DOFade(1f, 0.2f)
            .SetLink(gameObject)
            .SetEase(Ease.OutQuad));

        _sequence.Join(_rectTransform.DOAnchorPos(_targetPosition, 0.3f, true)
            .SetLink(gameObject)
            .SetEase(Ease.OutCubic));

        _sequence.AppendInterval(1.5f);

        _sequence.Append(_scoreAddText.DOFade(0f, 0.4f)
            .SetLink(gameObject)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                // リストからの除去は存在する場合のみ
                if (_infoPanel != null) {
                    _infoPanel.RemoveScoreAddList(this);
                }
                // ここで一度だけDestroy
                if (gameObject != null) {
                    Destroy(gameObject);
                }
            }));
    }

    public void FadeOut()
    {
        // すでにフェード開始済み or シーケンス未生成なら何もしない
        if (_isFadingOut || _sequence == null || !_sequence.IsActive()) return;
        _isFadingOut = true;

        _scoreAddText.DOFade(0f, 0.2f)
            .SetLink(gameObject)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                // リストからの除去は存在する場合のみ
                if (_infoPanel != null) {
                    _infoPanel.RemoveScoreAddList(this);
                }
                // ここで一度だけDestroy
                if (gameObject != null) {
                    Destroy(gameObject);
                }
            });
    }
}
