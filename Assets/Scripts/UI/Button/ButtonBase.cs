using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public RectTransform rectTransform;
    // インスペクターから登録できるイベント
    public UnityEvent onClick;
    public UnityEvent onEnter;
    public UnityEvent onExit;
    [SerializeField] protected bool _isSelectable = true;
    public bool isSelectable => _isSelectable;

    // --- Pointer イベント（→ 子クラスで override して演出） ---
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        onEnter?.Invoke();
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        onExit?.Invoke();
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke();
    }

    public virtual void InitButton()
    {
        rectTransform.localScale = Vector3.one;
    }

    public virtual void SetButtonSelectable(bool isEnable)
    {
        if (isEnable)
        {
            _isSelectable = true;
        }
        else
        {
            _isSelectable = false;
        }
    }

    public void PlaySound(AudioClip audioClip)
    {
        if (audioClip != null)
        {
            SoundManager.Instance.PlaySFX(audioClip);
        }
    }
}
