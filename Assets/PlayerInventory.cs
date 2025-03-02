using System;
using System.Collections;
using System.Collections.Generic;
using Thirdweb.Unity.Examples;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;
    public GameData stats;
    [System.Serializable]
    public class AbilitiesUnlocked
    {
        public int AbilityIndex;
        public GameObject CardFront;
        public GameObject Empty;
        public GameObject ClaimButton;
        public GameObject claimedText;
        public bool claimedNFT;
        public GameObject TransactionButton;
        public string NFT_url;


    }
    public List<AbilitiesUnlocked> Inventory;

    private void Awake()
    {
        Instance = this;
       

    }
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Inventory.Count; i++)
        {
            Inventory[i].claimedNFT = stats.enemyStats.claimedNFT[i];
            Inventory[i].NFT_url = stats.enemyStats.claimedNFT_URL[i];
        }
     
        LeanTween.delayedCall(1f, () => { UpdateInventory(); });
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateInventory()
    {
        for (int index = 0; index < Inventory.Count; index++)
        {
            if (PlayerProgression.Instance.abilities[index].unlocked)
            {
                Inventory[index].CardFront.SetActive(true);
                Inventory[index].Empty.SetActive(false);
            }
           
        }
        for (int index = 0; index < Inventory.Count; index++)
        {
            if (Inventory[index].claimedNFT)
            {
                Inventory[index].claimedText.SetActive(true);
                Inventory[index].TransactionButton.SetActive(true);
            }
            else
                Inventory[index].ClaimButton.SetActive(true);
        }
    }
    public void ClaimNFT(int index)
    {
        BlockchainManager.instance.ClaimAbilityNFT(index);
        Inventory[index].NFT_url = BlockchainManager.instance.SendTransaction_URL();
        stats.enemyStats.claimedNFT_URL[index] = BlockchainManager.instance.SendTransaction_URL();
        stats.SaveStats();

    }
    public void OpenNFT_Transaction(int index)
    {
        Application.OpenURL(Inventory[index].NFT_url);
    }
}
