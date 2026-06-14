using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BallColor")]
public class BallColor : ScriptableObject
{
    public List<Color> ballColor;
    [Range(0f, 1f)] public float animationTime;

    public Color GetBallColor(ItemId itemId)
    {
        if (ballColor.Count > (int)itemId)
        {
            return ballColor[(int)itemId];
        }
        //アイテムなど当てはまる色がなかったとき
        return Color.white;
    }
}
