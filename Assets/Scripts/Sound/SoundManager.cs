using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    [SerializeField] private AudioSource _BGMSource;
    [SerializeField] private AudioSource _SFXSource;
    private double _loopPoint = float.MaxValue;
    private double _loopBackPoint = float.MaxValue;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンまたぎでも保持したい場合
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(AudioClip clip, float pitch = 1f)
    {
        if (clip != null)
        {
            AudioSource temp = gameObject.AddComponent<AudioSource>();
            temp.volume = _SFXSource.volume;
            temp.pitch = pitch;
            temp.PlayOneShot(clip);
            Destroy(temp, clip.length / pitch); // 再生後に破棄
        }
    }

    public void PlaySFXScheduled(AudioClip clip, double startTime)
    {
        if (clip != null)
        {
            _SFXSource.clip = clip;
            _SFXSource.PlayScheduled(startTime);
        }
    }

    public void PlayBGM(GameBGM bgm, float resumeTime = 0)
    {

        if (bgm.audioClip != null)
        {
            _BGMSource.clip = bgm.audioClip;
            _BGMSource.time = resumeTime;

            StartCoroutine(WaitBGMToPlay(bgm));
        }
    }

    IEnumerator WaitBGMToPlay(GameBGM bgm)
    {
        _BGMSource.Play();

        //再生されるまでのラグ
        yield return new WaitForSeconds((float)bgm.bgmStartDelay);
        yield return new WaitUntil(() => _BGMSource.isPlaying);

        //実際にBGMが再生され始めた時間
        double BGMStartTime = AudioSettings.dspTime;

        _loopPoint = (BeatCtrl.GetOneBeatTime() * bgm.loopPoint) + BGMStartTime;
        _loopBackPoint = (BeatCtrl.GetOneBeatTime() * bgm.loopStartPoint) + BGMStartTime;

        BeatCtrl.SetBGMStartTime(BGMStartTime);
    }

    public void PauseBGM()
    {
        if (_BGMSource.isPlaying)
        {
            _BGMSource.Pause();
        }
    }

    public void ResumeBGM()
    {
        _BGMSource.UnPause();
    }

    public void StopBGM()
    {
        _BGMSource.Stop();
    }

    public void WarpSoundTime(float time)
    {
        _BGMSource.time = time;
    }

    void Update()
    {
        if (_BGMSource.isPlaying) {
            if (BeatCtrl.isLooping && _BGMSource.time >= _loopPoint)
            {
                _BGMSource.time -= (float)(_loopPoint - _loopBackPoint);
                BeatCtrl.ResetBeat();
            }
        }
    }
}
