using System.Collections.Generic;
using UnityEngine;

public class BallWeight : MonoBehaviour
{
    [SerializeField] List<int> _ballWeights;
    [SerializeField] int _normalWeight;
    public void ChangeBallWeight(ItemId itemId, int weight)
    {
        _ballWeights[(int)itemId] = weight;
    }

    public static ItemId GetRandomBallId()
    {
        // CanSide 未満 = 色だけ
        int maxColor = (int)ItemId.CanSide; // Red..Yellow の個数
        return (ItemId)Random.Range(0, maxColor); // 右端は排他的
    }

    public static ItemId GetRandomItemId()
    {
        // CanSide 未満 = 色だけ
        int maxColor = (int)ItemId.CanSide; // Red..Yellow の個数
        int minColor = (int)ItemId.MirrorBall; // Red..Yellow の個数
        return (ItemId)Random.Range(minColor, maxColor); // 右端は排他的
    }

    public void SetDefaultWeight()
    {
        for (int i = 0; i < _ballWeights.Count; i++)
        {
            _ballWeights[i] = _normalWeight;
        }
    }

    public ItemId GetWeightedRandomBallId()
    {
        int total = 0;
        foreach (int w in _ballWeights) total += w;

        int r = Random.Range(0, total);
        for (int i = 0; i < _ballWeights.Count; i++)
        {
            if (r < _ballWeights[i])
                return (ItemId)i;
            r -= _ballWeights[i];
        }
        return GetRandomBallId(); // fallback
    }
}
