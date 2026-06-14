using DG.Tweening;
using UnityEngine;

public class AppSystem : MonoBehaviour
{
    [SerializeField] private int _frameRate = 60;
    private void Awake()
    {
        //Tweenの上限突破
        DOTween.SetTweensCapacity(1000, 500);

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public static AppSystem Instance { get; private set; }
    void Start()
    {
        Application.targetFrameRate = _frameRate;
    }
    public static void SetTimeScale(float scale)
    {
        Time.timeScale = scale;
    }

    
    public void ApplicationQuit()
    {
        Debug.Log("アプリケーション終了");
        Application.Quit();
    }
}
