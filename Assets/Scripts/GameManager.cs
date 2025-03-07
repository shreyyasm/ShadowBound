using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject ChaptersMenu;
    public GameObject InventoryMenu;
    public GameObject ShopMenu;
    public GameObject RewardsMenu;
    public GameObject ChestScreen;
    public GameObject CardUnlockedScreen;

    public AudioSource audioSource;
    public AudioClip clip;

    public TextMeshProUGUI ButtonNameChange;
    public int Cutscene = 0;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        Cutscene = PlayerPrefs.GetInt("CutScene");
        if (Cutscene == 1)
        {
            if(ButtonNameChange != null)
                ButtonNameChange.text = "Continue";
        }
    }
    public void PlayGame()
    {
        Cutscene = PlayerPrefs.GetInt("CutScene");
        if(Cutscene == 0)
            SceneManager.LoadScene("Cut scene");
        else
            SceneManager.LoadScene("Level " + (PlayerPrefs.GetInt("LastCompletedChapter") + 1));



    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void OpenCardUnlockedScreen()
    {
        CardUnlockedScreen.SetActive(true);

    }
    public void CloseCardUnlockedScreen()
    {
        CardUnlockedScreen.SetActive(false);
        audioSource.PlayOneShot(clip, 0.5f);
      
    }
    public void OpenRewardsMenu()
    {
        RewardsMenu.SetActive(true);
    }
    public void CloseRewardsMenu()
    {
        RewardsMenu.SetActive(false);
        audioSource.PlayOneShot(clip, 0.5f);
    }
    public void OpenChaptersMenu()
    {
        ChaptersMenu.SetActive(true);
    }
    public void CloseChaptersMenu()
    {
        ChaptersMenu.SetActive(false);
        audioSource.PlayOneShot(clip, 0.5f);
    }
    public void OpenShopMenu()
    {
        ShopMenu.SetActive(true);
    }
    public void CloseShopMenu()
    {
       ShopMenu.SetActive(false);
        audioSource.PlayOneShot(clip, 0.5f);
    }
    public void OpenInventoryMenu()
    {
        InventoryMenu.SetActive(true);
        PlayerInventory.Instance.LoadInventory();
    }
    public void CloseInventoryMenu()
    {
        InventoryMenu.SetActive(false);
        audioSource.PlayOneShot(clip, 0.5f);
    }
    public void OpenChestMenu()
    {
        ChestScreen.SetActive(true);
    }
    public void CloseChestMenu()
    {
        ChestScreen.SetActive(false);
        audioSource.PlayOneShot(clip, 0.5f);
    }


}
