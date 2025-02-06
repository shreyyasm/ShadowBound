using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats",  menuName = "CreateStatsData/PlayerStats")]
public class PlayerStats :ScriptableObject
{
    public string Level;
    public int MoveSpeed;
    public int DashSpeed;
    public float HealthModifer;
    public float MaxHealth;
}
