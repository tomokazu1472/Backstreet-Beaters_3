using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CommonPanel : MonoBehaviour
{
    [SerializeField] Image _panel;

    public void FadePanel(float alpha, float duration, Ease ease = Ease.InOutCirc)
    {
        _panel.DOFade(alpha, duration)
        .SetLink(gameObject)
        .SetEase(ease);
    }

    public void SetAlpha(float alpha)
    {
        Color color = _panel.color;
        color.a = alpha;
        _panel.color = color;
    }

    public void SetRaycast(bool value)
    {
        _panel.raycastTarget = value;
    }
}
