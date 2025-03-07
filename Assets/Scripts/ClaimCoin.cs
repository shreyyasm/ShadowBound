using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class ClaimCoin : MonoBehaviour
{
    public string ID;
    public int CLaimID;
    public GameData gameData;
    public AudioSource audioSource;
    public AudioClip clip;
    // Start is called before the first frame update
    void Start()
    {
       CLaimID = PlayerPrefs.GetInt(ID);
        if(CLaimID == 1)
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ClaimCoins(int coins)
    {

        PlayerProgression.Instance.coins += coins;
        PlayerProgression.Instance.coinText.text = "Coins:" + PlayerProgression.Instance.coins.ToString();
        gameData.enemyStats.coins += coins;
        gameData.SaveStats();
        gameObject.SetActive(false);
        audioSource.PlayOneShot(clip);
        PlayerPrefs.SetInt(ID, 1);
    }
}
