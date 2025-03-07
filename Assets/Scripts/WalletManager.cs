//using System.Threading.Tasks;
//using UnityEngine;
//using Thirdweb;
//using Thirdweb.Unity;

//public class WalletManager : MonoBehaviour
//{
//    private ThirdwebSDK sdk;
//    private Wallet activeWallet;
//    public string contractAddress = "YOUR_NFT_CONTRACT_ADDRESS"; // Replace with your contract address

//    void Start()
//    {
//        // Get the Thirdweb SDK instance from the ThirdwebManager prefab
//        sdk = ThirdwebManager.Instance.SDK;
//    }

//    public async Task<string> ConnectWallet()
//    {
//        try
//        {
//            // Define wallet connection options (using Metamask or other wallets)
//            activeWallet = await sdk.Wallet.Connect(new WalletConnection(WalletProvider.Metamask, new Chain("avalanche-fuji")));

//            string address = activeWallet.Account.Address;
//            Debug.Log("Connected Wallet Address: " + address);
//            return address;
//        }
//        catch (System.Exception e)
//        {
//            Debug.LogError("Wallet Connection Failed: " + e.Message);
//            return null;
//        }
//    }

//    public async Task<bool> CheckNFTOwnership()
//    {
//        try
//        {
//            if (activeWallet == null)
//            {
//                Debug.LogError("Wallet is not connected.");
//                return false;
//            }

//            string walletAddress = activeWallet.Account.Address;

//            // Get NFT contract
//            var contract = await sdk.GetContract(contractAddress);

//            // Check NFT balance of the user
//            var balance = await contract.ERC721.BalanceOf(walletAddress);
//            Debug.Log("NFT Balance: " + balance);

//            return balance > 0;
//        }
//        catch (System.Exception e)
//        {
//            Debug.LogError("Failed to check NFT ownership: " + e.Message);
//            return false;
//        }
//    }

//    public async Task ClaimNFT()
//    {
//        try
//        {
//            if (activeWallet == null)
//            {
//                Debug.LogError("No wallet connected.");
//                return;
//            }

//            // Get NFT contract
//            var contract = await sdk.GetContract(contractAddress);

//            // Claim an NFT (assuming the contract supports minting)
//            await contract.ERC721.Claim(1);

//            Debug.Log("NFT Claimed Successfully!");
//        }
//        catch (System.Exception e)
//        {
//            Debug.LogError("Failed to claim NFT: " + e.Message);
//        }
//    }
//}
