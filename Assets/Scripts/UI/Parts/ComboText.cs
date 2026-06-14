using DG.Tweening;
using TMPro;
using UnityEngine;

public class ComboText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _comboText;

    public void SetPositon(Vector2 pos)
    {
        _comboText.rectTransform.position = pos;
    }

    public void DisplayText(int comboCount)
    {
        _comboText.SetText(comboCount.ToString() + " Combo!");
        _comboText.rectTransform.localScale = new Vector2(0f, 0f);

        _comboText.rectTransform.DOScale(1.0f, 0.4f)
            .SetLink(gameObject)
            .SetEase(Ease.OutBack);

        _comboText.DOFade(0f, 0.5f)
            .SetLink(gameObject)
            .SetDelay(0.4f)
            .OnComplete(() =>
            {
                if (gameObject != null) {
                    Destroy(gameObject);
                }
            });
    }
}
