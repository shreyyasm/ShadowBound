//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using Sequence;
//using Sequence.Wallet;
//using Sequence.Indexer;
//using Sequence.Minter;

//public class BlockchainManager : MonoBehaviour
//{
//    private string playerWallet;
//    private Dictionary<string, int> tokenBalances = new Dictionary<string, int>();
    
//    public string abilityTokenContract = "0xYourTokenContractAddress";
//    public string abilityTokenId = "1"; // Unique ID for the ability token
//    public int abilityPrice = 10; // Cost of the ability in coins

//    async void Start()
//    {
//        await InitializeSequenceWallet();
//    }

//    async System.Threading.Tasks.Task InitializeSequenceWallet()
//    {
//        var wallet = await SequenceWallet.Login();
//        if (wallet != null)
//        {
//            playerWallet = wallet.address;
//            Debug.Log("Wallet connected: " + playerWallet);
//            await FetchPlayerBalance();
//        }
//        else
//        {
//            Debug.LogError("Failed to connect wallet.");
//        }
//    }

//    async System.Threading.Tasks.Task FetchPlayerBalance()
//    {
//        if (string.IsNullOrEmpty(playerWallet)) return;
        
//        var balances = await SequenceIndexer.GetTokenBalances(playerWallet);
//        tokenBalances.Clear();
        
//        foreach (var balance in balances)
//        {
//            tokenBalances[balance.tokenId] = balance.amount;
//        }
        
//        Debug.Log("Balance updated: " + tokenBalances.ToString());
//    }

//    public async void BuyAbility()
//    {
//        if (!tokenBalances.ContainsKey("COIN") || tokenBalances["COIN"] < abilityPrice)
//        {
//            Debug.LogError("Not enough coins to buy ability.");
//            return;
//        }
        
//        await MintAbilityToken();
//    }
    
//    async System.Threading.Tasks.Task MintAbilityToken()
//    {
//        var mintRequest = new MintRequest()
//        {
//            to = playerWallet,
//            contractAddress = abilityTokenContract,
//            tokenId = abilityTokenId,
//            amount = 1
//        };
        
//        var result = await PermissionedMinter.MintToken(mintRequest);
//        if (result.success)
//        {
//            Debug.Log("Ability purchased and token minted!");
//            await FetchPlayerBalance();
//        }
//        else
//        {
//            Debug.LogError("Minting failed: " + result.error);
//        }
//    }
//}
