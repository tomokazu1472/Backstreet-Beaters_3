using UnityEngine;

public class MenuReset : MonoBehaviour
{
    [SerializeField] TitleScreen _titleScreen;
    [SerializeField] LoopBGM _loopBGM;
    void Start()
    {
        BeatCtrl.SetBGMInfo(_loopBGM.gameBGM);
        
        GameMenu.AddInitAction(() =>
        {
            _titleScreen.OpeningAnimation();
        });

        GameMenu.InitMenu();
    }
}
