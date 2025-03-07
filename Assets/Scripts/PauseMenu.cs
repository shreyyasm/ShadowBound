using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject ShopMenu;

    public AudioSource audioSource;
    public AudioClip clip;

    public bool MenuOpened;
    public GameObject PauseScreen;
    public void PauseGame()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(!MenuOpened)
            {
                PauseScreen.SetActive(true);
                MenuOpened = true;
            }
            else
            {
                PauseScreen.SetActive(false);
                MenuOpened = false;
            }

        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PauseGame();
    }
    public void LoadNextChapter()
    {       
        if(PlayerPrefs.GetInt("LastCompletedChapter") != 8)
        {
            SceneManager.LoadScene("Level " + (PlayerPrefs.GetInt("LastCompletedChapter") + 1));
            audioSource.PlayOneShot(clip);
        }
        if (PlayerPrefs.GetInt("LastCompletedChapter") == 8)
        {
            SceneManager.LoadScene("To Be Continued");
            audioSource.PlayOneShot(clip);
        }
        
    }
    public void ReloadLevel()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneIndex);
        audioSource.PlayOneShot(clip);
    }
    public void OpenShop()
    {
        ShopMenu.SetActive(true);
        audioSource.PlayOneShot(clip);
    }
    public void CloseShop()
    {
        ShopMenu.SetActive(false);
        audioSource.PlayOneShot(clip);
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Menu");
        audioSource.PlayOneShot(clip);
    }
}
