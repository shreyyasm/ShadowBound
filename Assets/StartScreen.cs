using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Thirdweb;
using System.Threading.Tasks;

public class StartScreen : MonoBehaviour
{
    private ThirdwebSDK sdk;
    public GameObject HasNFT;
    public GameObject NoNFT;

    public Prefab_NFT nftPrefab;
    // Start is called before the first frame update
    void Start()
    {
        sdk = new ThirdwebSDK("Avalanche Testnet");

        HasNFT.SetActive(false);
        NoNFT.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public async void ToggleStartScreen(GameObject connectedState, GameObject DisconnectedState, string address)
    {
        connectedState.SetActive(true);
        DisconnectedState.SetActive(false);

        Contract contract = sdk.GetContract("0xA33A55BC3F2b255234F1EC4f2B760c397209d98d");

        string stringBalance = await CheckBalance(address, contract);
        float floatBalance = float.Parse(stringBalance);

        if(floatBalance>0)
        {
            HasNFT.SetActive(true);
        }
        else
        {
            NoNFT.SetActive(true);
            GetNFTMedia(contract);
        }
    }
    public async Task<string> CheckBalance(string address, Contract contract)
    {
        string balance = await contract.Read<string>("balanceOf", address, 0);
        print(balance);
        return balance;
    }
    public async void GetNFTMedia(Contract contract)
    {
        NFT nft = await contract.ERC1155.Get("0");
        Prefab_NFT nftPrefabScript = nftPrefab.GetComponent<Prefab_NFT>();
        nftPrefabScript.LoadNFT(nft);
    }
}
