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
    public List<int> AbilityCardsHave;
    public List<bool> AbilityUnlocked;
    

}
