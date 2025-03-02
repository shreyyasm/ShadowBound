using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class GameStatsData
{
    [Header("Level")]
    public string Level;
    public int LevelIndex;

    [Header("Currency")]
    public int coins;
    public int playerXP;

    [Header("Cards")]  
    public List<int> AbilityCardsHave;
    public List<bool> AbilityUnlocked;
    public List<bool> claimedNFT;
    public List<string> claimedNFT_URL;
}

public class GameData : MonoBehaviour
{
    private string filePath;
    public GameStatsData enemyStats;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "enemyStats.json");
        //SaveStats();
        LoadStats();
    }

    public void SaveStats()
    {
        string json = JsonUtility.ToJson(enemyStats, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Enemy stats saved: " + filePath);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //enemyStats.claimedNFT[0] = true;

            //enemyStats.claimedNFT[1] = true;

            //enemyStats.claimedNFT[2] = true;
            //enemyStats.claimedNFT[3] = false;
            //enemyStats.claimedNFT[4] = false;
            //enemyStats.claimedNFT_URL = new List<string>() { "", "", "", "", "" };
            //SaveStats();
        }

    }
    public void LoadStats()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            enemyStats = JsonUtility.FromJson<GameStatsData>(json);
            Debug.Log("Enemy stats loaded");
        }
        else
        {
            Debug.Log("No save file found, creating default stats.");
            enemyStats = new GameStatsData()
            {
                Level = "Level 1",
                LevelIndex = 1,
                playerXP = 0,
                coins = 100000,
                AbilityUnlocked = new List<bool>() { false, false, false, false, false },
                claimedNFT = new List<bool>() { false, false, false, false, false },
                AbilityCardsHave = new List<int>() { 0, 0, 0, 0, 0 },
                claimedNFT_URL = new List<string>() { "", "", "", "", "" }
            };
            SaveStats();
        }
    }
}
