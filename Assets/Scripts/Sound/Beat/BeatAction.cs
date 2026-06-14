using System;
using UnityEngine;

public class BeatAction : MonoBehaviour
{
    static Action _beatAction;

    /// <summary>
    /// ビートが進むごとに呼ばれるアクション
    /// </summary>
    public void DoBeatAction()
    {
        _beatAction?.Invoke();
    }

    public static void AddBeatAction(Action action)
    {
        _beatAction += action;
    }

    public static void ClearBeatAction()
    {
        _beatAction = null;
    }
}
