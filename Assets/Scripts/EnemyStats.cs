using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "CreateStatsData/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    public string Level;
    public int LevelIndex;

    [Header("NPC MovementStats")]
    public int NPCSpeed;
    public float RoamRadius;
    public float RoamDelay;

    [Header("ConsumeStats")]
    public int MoveSpeed;   
    public int DashSpeed;

    [Header("Vision Settings")]
    public float visionRange = 10f;
    public float visionAngle = 45f;

    [Header("Chase Settings")]
    public float chaseSpeed = 4f;
    public float normalSpeed = 2f;
    public float alertDelay = 2f;

    [Header("Control & Stun")]
    public float controlTime = 5f; // Time before NPC gets stunned if controlled
    public float stunDuration = 3f;
    public float searchTime = 4f;


}
