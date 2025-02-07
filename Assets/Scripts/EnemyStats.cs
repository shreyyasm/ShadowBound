using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "CreateStatsData/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    public string Level;

    [Header("NPC MovementStats")]
    public int NPCSpeed;
    public float RoamRadius;
    public float RoamDelay;

    [Header("NPC Health")]
    public float HealthModifer;
    public float MaxHealth;

    [Header("ConsumeStats")]
    public int MoveSpeed;   
    public int DashSpeed;
   

    

}
