using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelClear : MonoBehaviour
{
    public static LevelClear Instance;
    public AudioSource audioSource;
    public AudioClip clearLevelSound;
    public int CurrentChapter;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Enemy"))
        {
            LevelCleared();
            audioSource.PlayOneShot(clearLevelSound);
        }
    }
    public void LevelCleared()
    {
        Transitioner.Instance.TransitionToFix();
        ChapterManager.instance.CompleteChapter(CurrentChapter);
        PlayerPrefs.SetInt("LastCompletedChapter", CurrentChapter);
        LeanTween.delayedCall(1f, () => { SceneManager.LoadScene("Level Clear"); });
       
    }
}
