using UnityEngine;

public class GameReset : MonoBehaviour
{
    [SerializeField] BallField _ballField;
    [SerializeField] DeleteCtrl _deleteCtrl;
    [SerializeField] SpawnCtrl _spawnCtrl;
    [SerializeField] GameInfo _gameInfo;
    [SerializeField] InfoPanel _infoPanel;
    [SerializeField] MissionWindow _missionWindow;
    [SerializeField] ComboCountCtrl _comboCountCtrl;
    [SerializeField] CountDownCtrl _countDownCtrl;
    [SerializeField] BallWeight _ballWeight;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void ResetGame()
    {
        _infoPanel.ClearInfo();
        _missionWindow.ClearInfo();
        _gameInfo.ResetGameInfo();
        _deleteCtrl.DeleteAllBalls();
        _countDownCtrl.Init();
        _ballWeight.SetDefaultWeight();
        _ballField.ClearField();
        _comboCountCtrl.ResetCombo();
        _comboCountCtrl.DeleteAllComboText();
        _spawnCtrl.SpawnBallsStart();
    }
}