using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameBGM
{
    [Tooltip("音源")] public AudioClip audioClip;
    [Tooltip("曲名")] public string songName;
    [Tooltip("作者名")] public string author;
    [Tooltip("BPM")] public float BPM;
    [Tooltip("曲の開始時間調整"), Range(-5.0f, 5.0f)] public double bgmStartDelay;
    [Tooltip("音源とゲームのズレ調整"), Range(-0.5f, 0.5f)] public double bgmSourceDelay;
    [Tooltip("音源の切り替え時間調整"), Range(-0.5f, 0.5f)] public double bgmSwitchDelay;
    [Tooltip("ゲームの開始させる拍")] public int gameStartBeat;
    [Tooltip("ゲームを終了させる拍")] public int gameFinishBeat;
    [Tooltip("カウントダウンの開始カウント(3とか4とか)")] public int countDownBeat;
    [Tooltip("ループに突入するときの拍")] public int loopStartPoint;
    [Tooltip("ループが一周するときの拍")] public int loopPoint;
    [Tooltip("イントロ音源の中で音源を切り替えても違和感がない拍")] public List<int> introSwitchableBeat;
    [Tooltip("曲のいいところで何か演出を入れるためのビート")] public List<int> muteBeatList;

}
