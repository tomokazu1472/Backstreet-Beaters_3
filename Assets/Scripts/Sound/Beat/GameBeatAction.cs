using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameBeatAction : MonoBehaviour
{
    [SerializeField] BallField _ballField;
    [SerializeField] BallSwap _ballSwap;
    [SerializeField] SpawnCtrl _spawnCtrl;
    [SerializeField] ItemSpawn _itemSpawn;
    [SerializeField] DeleteCtrl _deleteCtrl;
    [SerializeField] ComboCountCtrl _comboCountText;
    [SerializeField] ColorClock _colorClock;
    public bool isGameFinished = false;

    /// <summary>
    /// ビートが進むごとに呼ばれるアクション
    /// </summary>
    
    void Start()
    {
        BeatAction.AddBeatAction(() =>
        {
            DoBeatAction();
        });
    }

    public void DoBeatAction()
    {
        if (!Gamepause.isGamePlaying || isGameFinished)
        {
            WaitingGameAction();
            return;
        }

        _colorClock.RotateToNextBeatAngle();
        if (TryGameAction())
        {
            //_colorClock.PauseHand(true);
        }
        else
        {
            //_colorClock.PauseHand(false);
            _comboCountText.StopCombo();
        }
    }

    private void WaitingGameAction()
    {
        if (_deleteCtrl.isAnimating || _ballSwap.isSwapping || _ballField.IsMoving()) return; // アニメーション中は何もしない
        if (_ballField.FallBalls()) return;
        if (!isGameFinished) _spawnCtrl.SpawnBalls();
    }
    

    private bool TryGameAction()
    {
        if (_deleteCtrl.DeleteMatchGroup())
        {
            return true;
        }
        //_ballField.VibeBalls();
        if (TryActivateItems())
        {
            return true;
        }
        if (_deleteCtrl.isAnimating || _ballSwap.isSwapping || _ballField.IsMoving()) return true; // アニメーション中は何もしない
        if (_ballField.FallBalls()) return false;
        if (!isGameFinished) _spawnCtrl.SpawnBalls();
        return false;
    }

    public bool TryActivateItems()
    {
        List<Ball> Items = _itemSpawn.GetAllItems();

        if (Items == null || Items.Count == 0)
        {
            return false; // アイテムがない場合は何もしない
        }

        foreach (Ball item in Items)
        {
            if (item != null && item.isMoving == false && item.isActivated == true)
            {
                item.Activate();
                _deleteCtrl.DeleteBallAnimated(item);
                _comboCountText.CountUpCombo(item);
                return true; // 一つのアイテムをアクティブにしたら終了
            }
        }
        return false; // アクティブにできるアイテムがなかった
    }
}
