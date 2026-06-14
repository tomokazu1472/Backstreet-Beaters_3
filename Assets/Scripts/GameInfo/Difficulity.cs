using System.Collections.Generic;
using UnityEngine;

public enum Difficulity
{
    Easy,
    Hard,
    Difficult,
    Extreme
}

[CreateAssetMenu(menuName = "DifficulityParams")]
public class DifficulityParams : ScriptableObject
{
    public List<Color> difficulityColor;

    public Color GetDifficulityColor(Difficulity difficulity)
    {
        return difficulityColor[(int)difficulity];
    }
}
