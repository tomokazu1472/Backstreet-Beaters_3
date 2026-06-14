using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class BeatCtrl : MonoBehaviour
{
    [SerializeField] BeatAction _beatAction;
    // メトロノーム音源
    [SerializeField] AudioClip _metronomeClip;
    // BGM音源
    private static GameBGM _gameBGM;

    // 再生状態
    static bool _isPlaying;
    public static bool isPlaying => _isPlaying;
    [SerializeField] bool _metronome;

    // 拍
    private static int _beatCount;
    [SerializeField, Tooltip("表示用")] private int beatCount;
    public static int BeatCount => _beatCount;
    private static float _bpm;
    public static bool isMuteBeat;
    private static double _nextBeatTime = double.MaxValue;
    private static double _bgmStartTime;
    private static List<int> _muteBeatList;
    private static bool _isLooping;
    public static bool isLooping => _isLooping;

    // 一時停止時間
    private static double _pauseTime;
    public static void SetBGMInfo(GameBGM bgm)
    {
        _gameBGM = bgm;
        _beatCount = 0;
        _bpm = bgm.BPM;
        _muteBeatList = _gameBGM.muteBeatList;
    }

    public static void ClearBGMInfo()
    {
        _isPlaying = false;
        _isLooping = false;
        _gameBGM = null;
        _beatCount = 0;
        _nextBeatTime = double.MaxValue;
    }

    // BGMを開始させる
    public static void StartBGM()
    {
        _beatCount = 0;

        SoundManager.Instance.PlayBGM(_gameBGM);
        SetBGMLoop(true);

        _isWaitingforWarp = true;
        _isLooping = true;
    }

    public static void SetBGMStartTime(double time)
    {
        _bgmStartTime = time + _gameBGM.bgmSourceDelay;
        SetNextBeatTime();
        _isPlaying = true;
        //Debug.Log($"nextTime : {_nextBeatTime}, dsp : {AudioSettings.dspTime}, count : {_beatCount}");
    }

    public static void SetBGMLoop(bool loop)
    {
        _isLooping = loop;
    }

    // 次のアクションの時間を設定する
    static void SetNextBeatTime()
    {
        _beatCount++;
        _nextBeatTime = _bgmStartTime + _beatCount * 60d / _bpm;
    }

    // updateだと呼び出しが安定しないのでFixedUpdateを使う
    void FixedUpdate()
    {
        if (_isPlaying)
        {
            // 現在時刻が設定した次の拍の時間を超えたとき
            if (AudioSettings.dspTime + Time.fixedDeltaTime >= _nextBeatTime)
            {
                //Debug.Log($"nextTime : {_nextBeatTime}, dsp : {AudioSettings.dspTime}, count : {_beatCount}");

                // ゲームの動作をさせる
                StartCoroutine(WaitForBeatActionCorutine((float)(_nextBeatTime - AudioSettings.dspTime)));
                // 次の拍の時間を設定する
                SetNextBeatTime();
            }
        }
        beatCount = _beatCount;//インスペクター表示用
    }

    IEnumerator WaitForBeatActionCorutine(float waitTime)
    {
        // メトロノームの音を鳴らす
        if (_metronome) SoundManager.Instance.PlaySFXScheduled(_metronomeClip, AudioSettings.dspTime + waitTime);
        yield return new WaitForSeconds(waitTime);


        GameAction();
        CheckWarp();
    }
    
    public static float GetOneBeatTime()
    {
        return 60f / _bpm;
    }

    public static double GetRemainingTime()
    {
        //ゲームの残り時間
        //ループ中だったら0を返す
        if (_isLooping || _isWaitingforWarp) return 0;

        return (_gameBGM.gameFinishBeat / _bpm * 60) - (AudioSettings.dspTime - _bgmStartTime);
    }

    public static int GetNextSwitchableBeat()
    {
        if (_gameBGM.introSwitchableBeat == null || _gameBGM.introSwitchableBeat.Count == 0) return 0;

        // 最初に nextBeat 以上になる拍を見つける（最も近い未来の拍）
        int candidate = -1;
        for (int i = 0; i < _gameBGM.introSwitchableBeat.Count; i++)
        {
            int b = _gameBGM.introSwitchableBeat[i];
            if (b >= _beatCount)
            {
                candidate = b;
                break;
            }
        }

        return candidate;
    }

    private static bool _isWaitingforWarp;
    void CheckWarp()
    {
        //Debug.Log(GetNextSwitchableBeat() + " : " + _beatCount);

        if (!_isLooping && _isWaitingforWarp && _beatCount == GetNextSwitchableBeat())
        {
            WarpBeat();
            double warpTime = _beatCount * 60d / _bpm;
            SoundManager.Instance.WarpSoundTime((float)warpTime);

            _isLooping = false;
            _isWaitingforWarp = false;
        }
        
        if (_isLooping && _beatCount >= _gameBGM.loopPoint)
        {
            ResetBeat();
            double resetTime = _beatCount * 60d / _bpm;
            SoundManager.Instance.WarpSoundTime((float)resetTime);
        }
    }

    void GameAction()
    {
        isMuteBeat = _muteBeatList.Contains(_beatCount);
        _beatAction.DoBeatAction();
    }

    // 一時停止する
    public static void PauseBGM()
    {
        if (!_isPlaying) return;

        SoundManager.Instance.PauseBGM();

        _pauseTime = AudioSettings.dspTime;
        _isPlaying = false;

    }

    // 一時停止を再開させる
    public static void ResumeBGM()
    {
        if (_isPlaying) return;

        double pausedDur = AudioSettings.dspTime - _pauseTime;

        // BGM開始時間に停止していた時間を加算してビート整合を取る
        _bgmStartTime += pausedDur;
        SetNextBeatTime();

        SoundManager.Instance.ResumeBGM();
        _isPlaying = true;

    }

    public static void WarpBeat()
    {
        int warpBeat = _gameBGM.loopPoint;
        //切替後にSetNextBeatTime()が呼び出されて+1されるので-1しとく
        _beatCount = warpBeat - 1;

        _bgmStartTime = AudioSettings.dspTime - (_beatCount * 60 / _bpm)
        + _gameBGM.bgmSourceDelay;
    }

    public static void ResetBeat()
    {
        int loopBackBeat = _gameBGM.loopStartPoint;
        //切替後にSetNextBeatTime()が呼び出されて+1されるので-1しとく
        _beatCount = loopBackBeat - 1;
        _bgmStartTime = AudioSettings.dspTime - (_beatCount * 60 / _bpm)
        + _gameBGM.bgmSourceDelay;
    }
}
