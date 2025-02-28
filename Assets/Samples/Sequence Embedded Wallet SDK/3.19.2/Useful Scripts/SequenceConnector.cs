using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Game.Scripts;
using Sequence;
using Sequence.EmbeddedWallet;
using Sequence.Relayer;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Samples.Scripts
{
    /// <summary>
    /// Attach this to a GameObject in your scene. It will automatically capture a SequenceWallet when it is created and setup all event handlers (fill in your own logic).
    /// This mono behaviour will persist between scenes and is accessed via SequenceConnector.Instance singleton.
    /// </summary>
    public class SequenceConnector : MonoBehaviour
    {
        public Chain Chain = Chain.TestnetArbitrumSepolia;
        public static SequenceConnector Instance { get; private set; }
        
        public SequenceWallet Wallet { get; private set; }
        public IIndexer Indexer { get; private set; }

      
        public const string ContractAddress = "0xcbc370bf5b168e72e4fdd91f9977322d3e2ead59";
        public const string CollectibleTokenId = "0";

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }

            SequenceWallet.OnWalletCreated += OnWalletCreated;
            Indexer = new ChainIndexer(Chain);



            //IIndexer chainIndexer = new ChainIndexer(Chain.Avalanche);
            //GetTokenBalancesReturn balances = await chainIndexer.GetTokenBalances(new GetTokenBalancesArgs(Wallet));
            //TokenBalance[] tokenBalances = balances.balances;
        }

        private void OnWalletCreated(SequenceWallet wallet)
        {
            Wallet = wallet;
            Wallet.OnSendTransactionComplete += OnSendTransactionCompleteHandler;
            Wallet.OnSendTransactionFailed += OnSendTransactionFailedHandler;
            Wallet.OnSignMessageComplete += OnSignMessageCompleteHandler;
            Wallet.OnDeployContractComplete += OnDeployContractCompleteHandler;
            Wallet.OnDeployContractFailed += OnDeployContractFailedHandler;
            Wallet.OnDropSessionComplete += OnDropSessionCompleteHandler;
            Wallet.OnSessionsFound += OnSessionsFoundHandler;

            GetTokenBalances();

            //SceneManager.LoadScene("Menu");

        }

        private void OnDestroy()
        {
            if (Wallet == null) return;
            Wallet.OnSendTransactionComplete -= OnSendTransactionCompleteHandler;
            Wallet.OnSendTransactionFailed -= OnSendTransactionFailedHandler;
            Wallet.OnSignMessageComplete -= OnSignMessageCompleteHandler;
            Wallet.OnDeployContractComplete -= OnDeployContractCompleteHandler;
            Wallet.OnDeployContractFailed -= OnDeployContractFailedHandler;
            Wallet.OnDropSessionComplete -= OnDropSessionCompleteHandler;
            Wallet.OnSessionsFound -= OnSessionsFoundHandler;
        }
        [SerializeField] TextMeshProUGUI _tokensAmountText;
        private void SetTokenAmountsText()
        {          
            _tokensAmountText.text = GetTokens().ToString();
        }
        public uint GetTokens()
        {
            BigInteger tokenId = BigInteger.Parse(SequenceConnector.CollectibleTokenId);
            Debug.Log(_tokenBalances.Count);
            if (_tokenBalances.TryGetValue(tokenId, out var tokenBalance))
            {
                return (uint)tokenBalance.balance;
                
            }
            else
            {
                Debug.Log("Naah");
                return 0;           
            }
        }

        private Dictionary<BigInteger, TokenBalance> _tokenBalances = new Dictionary<BigInteger, TokenBalance>();
        private async Task GetTokenBalances(Page page = null)
        {
            if (page == null)
            {
                page = new Page();
            }
            string walletAddress = Wallet.GetWalletAddress();
            GetTokenBalancesReturn balances = await Indexer.GetTokenBalances(new GetTokenBalancesArgs(Wallet.GetWalletAddress(), ContractAddress, false, page));
            int uniqueTokens = balances.balances.Length;
            Debug.Log(balances.balances.Length);
            for (int i = 0; i < uniqueTokens; i++)
            {
                _tokenBalances[balances.balances[i].tokenID] = balances.balances[i];
               
            }
            if (balances.page.more)
            {
                await GetTokenBalances(balances.page);
                
            }
            SetTokenAmountsText();
        }
        private void OnSendTransactionCompleteHandler(SuccessfulTransactionReturn result) {
            // Do something
        }

        private void OnSendTransactionFailedHandler(FailedTransactionReturn result) {
            // Do something
        }
        
        private void OnSignMessageCompleteHandler(string result) {
            // Do something
        }
        
        private void OnDeployContractCompleteHandler(SuccessfulContractDeploymentReturn result) {
            Address newlyDeployedContractAddress = result.DeployedContractAddress;

            // Do something
        }

        private void OnDeployContractFailedHandler(FailedContractDeploymentReturn result) {
            // Do something
        }
        
        private void OnDropSessionCompleteHandler(string sessionId) {
            // Do something
        }
        
        private void OnSessionsFoundHandler(Session[] sessions) {
            // Do something
        }
    }
}