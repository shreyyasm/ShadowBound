using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject ChaptersMenu;
    public GameObject InventoryMenu;
    public GameObject ShopMenu;
    public GameObject PlayerLevelMenu;
    public GameObject ChestScreen;

    public AudioSource audioSource;
    public AudioClip clip;
    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
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
