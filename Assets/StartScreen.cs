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

        string stringBalance = await CheckBalance(address);
        float floatBalance = float.Parse(stringBalance);

        if(floatBalance>0)
        {
            HasNFT.SetActive(true);
        }
        else
        {
            NoNFT.SetActive(true);
        }
    }
    public async Task<string> CheckBalance(string address)
    {
        Contract contract = sdk.GetContract("0xA33A55BC3F2b255234F1EC4f2B760c397209d98d");
        string balance = await contract.Read<string>("balanceOf", address, 0);
        return balance;
    }
}
