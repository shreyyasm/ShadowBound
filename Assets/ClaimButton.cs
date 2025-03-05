using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClaimButton : MonoBehaviour
{
    public PlayerInventory inventory;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ClaimNFT(int Index)
    {
        inventory.ClaimNFT(Index);
        gameObject.SetActive(false);
    }
}
