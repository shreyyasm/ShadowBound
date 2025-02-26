using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject ChaptersMenu;
    public GameObject AbilitiesMenu;
    public GameObject BuyChestMenu;
    public GameObject PlayerLevelMenu;
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
    }
    public void OpenAbilitiesMenu()
    {
        AbilitiesMenu.SetActive(true);
    }
    public void CloseAbilitiesMenu()
    {
        AbilitiesMenu.SetActive(false);
    }
    public void OpenChestMenu()
    {
        BuyChestMenu.SetActive(true);
    }
    public void CloseChestMenu()
    {
        BuyChestMenu.SetActive(false);
    }

}
