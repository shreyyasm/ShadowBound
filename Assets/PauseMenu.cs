using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject ShopMenu;

    public AudioSource audioSource;
    public AudioClip clip;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LoadNextChapter()
    {       
        SceneManager.LoadScene("Level " + (PlayerPrefs.GetInt("LastCompletedChapter")+ 1));
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
