using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    [SerializeField] private Camera _targetCamera;
    [SerializeField] private Vector2 _windowAspect = new Vector2(16f, 9f);

    //画面を強制的に16:9にして、余白を黒帯にする
    private void Start()
    {
        if(_targetCamera == null) return;

        float targetAspect = _windowAspect.x / _windowAspect.y;
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scale = windowAspect / targetAspect;

        Rect rect = _targetCamera.rect;

        if(scale < 1f)
        {
            rect.width = 1f;
            rect.height = scale;
            rect.x = 0f;
            rect.y = (1f - scale) * 0.5f;
        }
        else
        {
            float inverseScale = 1f / scale;
            rect.width = inverseScale;
            rect.height = 1f;
            rect.x = (1f - inverseScale) * 0.5f;
            rect.y = 0f;
        }

        _targetCamera.rect = rect;
    }
}
