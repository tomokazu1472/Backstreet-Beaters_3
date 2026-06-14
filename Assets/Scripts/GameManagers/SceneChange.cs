using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneChange : MonoBehaviour
{

    // シーン名を指定して切り替える
    public static void ChangeScene(string menuName)
    {
        DOTween.KillAll();
        GameMenu.ClearInitAction();
        BeatAction.ClearBeatAction();
        SoundManager.Instance.PauseBGM();
        BeatCtrl.ClearBGMInfo();
        AppSystem.SetTimeScale(1f);

        switch (menuName)
        {
            case "GamePlay":
                SceneManager.LoadScene(menuName);
                GameMenu.SetMenu(MenuName.Gameplay);
                Debug.Log("現在のシーン：" + menuName);
                break;
            case "Menu":
                SceneManager.LoadScene(menuName);
                GameMenu.SetMenu(MenuName.Title);
                Debug.Log("現在のシーン：" + menuName);
                break;
            default:
                Debug.Log(menuName + "というシーンは存在しません");
                break;
        }
    }
}
