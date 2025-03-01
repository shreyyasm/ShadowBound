using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Whalepass;

public class WhalePassAPI : MonoBehaviour
{
    public static WhalePassAPI instance;
    public string GameId = ""; // Replace with your Game ID
    public string ApiKey = ""; // Replace with your API Key
    public string BattlePassId = ""; // Replace with your Battlepass ID

    public TMP_InputField nameInputField;
    public string playerId = "PlayerName";
    public GameObject playerNameMenu; // Assign this in the Inspector

    public int currentLevel;
    public int CurrentTotalExp;
    public int CurrentExp;
    public int NextLevelExp;
    public int ExpRequiredLastlevel;

   // public LevelManager levelManager;
    public int CurrentLevel
    {
        get => currentLevel;
        set
        {
            if (currentLevel != value) // Only trigger when the value actually changes
            {
                currentLevel = value;
                OnLevelChanged?.Invoke(currentLevel); // Trigger event
            }
        }
    }
    // Event triggered when the level changes
    public event Action<int> OnLevelChanged;
    private void Awake()
    {
        if(instance == null)
            instance = this;

        // Load the saved name if it exists
        if (PlayerPrefs.HasKey(playerId))
        {
            string savedName = PlayerPrefs.GetString(playerId);
            if (!string.IsNullOrEmpty(savedName))
            {
                playerId = PlayerPrefs.GetString(playerId);
                playerNameMenu.SetActive(false); // Hide menu if name exists
                EnrollPlayer();
                CheckPlayer_Inventory();
                GettingBattlePass();
                PlayerBaseResponse();
                return;
            }
        }
        playerNameMenu.SetActive(true); // Show menu if no name is set

       
        //LeanTween.delayedCall(2f, () => { levelManager.UpdateUI(); });
        
    }


    public void SavePlayerName()
    {
        // Save the player name in PlayerPrefs
        string playerName = nameInputField.text;
        if (!string.IsNullOrEmpty(playerName))
        {
            PlayerPrefs.SetString(playerId, playerName);
            PlayerPrefs.Save();
            playerNameMenu.SetActive(false); // Hide menu after saving
            EnrollPlayer();
            CheckPlayer_Inventory();
            GettingBattlePass();
            PlayerBaseResponse();
        }
    }


    public void EnrollPlayer()
    {
        WhalepassSdkManager.enroll(playerId, response =>
        {
            Debug.Log(response.succeed);
        });
    }
    public void AddExp(int exp)
    {
        WhalepassSdkManager.updateExp(playerId, exp, response =>
        {
            Debug.Log(response.succeed);
            Debug.Log(response.responseBody);
        });
    }
    public void CheckPlayer_Inventory()
    {
        WhalepassSdkManager.getPlayerInventory(playerId, response =>
        {
            Debug.Log(response.succeed);
            Debug.Log(response.responseBody);
        });
    }
    public void RedirectPlayer_Rewards()
    {
        WhalepassSdkManager.getPlayerRedirectionLink(playerId, response =>
        {
            
            Application.OpenURL(response.link.redirectionLink);
        });
    }
    public void GettingBattlePass()
    {
        WhalepassSdkManager.getBattlepass(BattlePassId, false, false, response =>
        {
           
        });
    }
    public void CompletingChallenge(string challengeID)
    {
        WhalepassSdkManager.completeChallenge(playerId, challengeID, response =>
        {
            Debug.Log(response.succeed);
            Debug.Log(response.responseBody);
        });
    }
    public void PlayerBaseResponse()
    {
        WhalepassSdkManager.getPlayerBaseProgress(playerId, response =>
        {
            
            NextLevelExp = (int)response.result.expRequiredForNextLevel;
            CurrentTotalExp = (int)response.result.currentExp;
            CurrentLevel = (int)response.result.lastCompletedLevel;
            CurrentExp = (int)response.result.currentExp - (int)response.result.expRequiredForLastLevel;
            ExpRequiredLastlevel = (int)response.result.expRequiredForLastLevel;
            //levelManager._playerLevel = (int)response.result.lastCompletedLevel;
            
        });
    }
}
