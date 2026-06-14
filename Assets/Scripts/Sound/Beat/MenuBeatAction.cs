using UnityEngine;

public class MenuBeatAction : MonoBehaviour
{
    [SerializeField] TitleScreen _titleScreen;

    void Start()
    {
        BeatAction.AddBeatAction(() =>
        {
            DoBeatAction();
        });
    }

    public void DoBeatAction()
    {
        _titleScreen.VibeTitleScreen();
    }
}
