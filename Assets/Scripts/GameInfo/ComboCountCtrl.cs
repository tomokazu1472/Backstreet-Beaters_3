using UnityEngine;
using System.Collections.Generic;

public class ComboCountCtrl : MonoBehaviour
{
    [SerializeField, Header("スクリプト")] ComboText _comboTextPrefab;
    [SerializeField] AudioClip _comboSoundClip;
    [SerializeField, Header("コンボ")] int _comboCount;
    public int comboCount => _comboCount;
    [SerializeField, Tooltip("コンボが切れてからコンボが無効になるまでの猶予")] int _comboMargin;
    int comboStopCount;

    public void SetComboMargin(int amount)
    {
        _comboMargin = amount;
    }

    public void StopCombo()
    {
        if (comboStopCount > _comboMargin)
        {
            _comboCount = 0;
            comboStopCount = 0;
        }
        comboStopCount++;
    }

    public void ResetCombo()
    {
        _comboCount = 0;
    }

    public void CountUpCombo(Ball centerball)
    {
        comboStopCount = 0;

        if (_comboCount != 0)
        {
            ComboText comboText = Instantiate(_comboTextPrefab, transform);
            comboText.SetPositon(centerball.transform.position);
            comboText.DisplayText(_comboCount);

            //コンボの音の高さ(12平均律)
            float soundPitch = Mathf.Min(Mathf.Pow(2f, (_comboCount - 1) / 12f), 3f);

            SoundManager.Instance.PlaySFX(_comboSoundClip, soundPitch);
            ParticleGenerator.Play(ParticleKey.Combo, centerball.transform.position);
        }

        _comboCount++;
    }

    public void DeleteAllComboText()
    {
        ComboText[] comboTexts = GetComponentsInChildren<ComboText>();
        foreach (ComboText comboText in comboTexts)
        {
            Destroy(comboText.gameObject);
        }
    }
}
