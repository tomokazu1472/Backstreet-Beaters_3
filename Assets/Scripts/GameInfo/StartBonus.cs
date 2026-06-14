using System.Collections;
using UnityEngine;
public enum StartBonusType
{
    RandomUp,
    BonusItem,
    ComboAsist
}

public class StartBonus : MonoBehaviour
{
    [SerializeField] ComboCountCtrl _comboCountCtrl;
    [SerializeField] BallField _ballField;
    [SerializeField] ItemSpawn _itemSpawn;
    [SerializeField] ColorClock _colorClock;
    [SerializeField] int _bonusComboMargin; 
    [SerializeField] int _bonusBallWeight;
    [SerializeField] int _spawnItemAmount;
    private StartBonusType _currentBonusType;

    public void SetBonusEffect(int bonusEffect)
    {
        _currentBonusType = (StartBonusType)bonusEffect;
    }
    
    public void ApplyBonusEffect()
    {
        switch (_currentBonusType)
        {
            case StartBonusType.RandomUp:
                _colorClock.SetTargetWeight(_bonusBallWeight, true);
                break;
            case StartBonusType.BonusItem:
                PlaceBonusItems(_spawnItemAmount);
                break;
            case StartBonusType.ComboAsist:
                _comboCountCtrl.SetComboMargin(_bonusComboMargin);
                break;
            default:
                break;
        }
    }

    void PlaceBonusItems(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector2Int itemPos = _ballField.GetRandomBall().position;

            _itemSpawn.SpawnItem(BallWeight.GetRandomItemId(), itemPos);
        }
    }

    public void StartCount()
    {
        StartCoroutine(WeightTimer(30));
    }
    
    IEnumerator WeightTimer(int time)
    {
        yield return new WaitForSecondsRealtime(time);
        _colorClock.SetTargetWeight(30, false);
    }
}
