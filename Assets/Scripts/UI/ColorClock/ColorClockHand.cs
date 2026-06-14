using UnityEngine;

public class ColorClockHand : MonoBehaviour
{
    [SerializeField] public RectTransform rectTransform;
    public void SetRotation(float rotation)
    {
        rectTransform.localRotation = Quaternion.Euler(0f, 0f, rotation);
    }
}
