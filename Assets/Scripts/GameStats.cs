using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameStats", menuName = "CreateStatsData/GameStats")]
public class GameStats : ScriptableObject
{
    public int PlayerLevel;
    public int PlayerXP;

    [Header("Money")]
    public int Coins;

    [Header("Cards")]

    public int Ability1Cards;
    public int Ability2Cards;
    public int Ability3Cards;
    public int Ability4Cards;
    public int Ability5Cards;
}
