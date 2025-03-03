using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChapterManager : MonoBehaviour
{
    public static ChapterManager instance;
    public int totalChapters = 10;
    public Transform chapterParent;

    public List<Button> chapterButtons = new List<Button>();
    public List<GameObject> lockedUIElements = new List<GameObject>();
    public int unlockedChapters;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        LoadProgress();
        UpdateAllButtons();
    }


    void UpdateButtonState(Button button, GameObject lockedUI, int index)
    {
        bool isUnlocked = index < unlockedChapters;
        button.interactable = isUnlocked;
        button.GetComponentInChildren<Text>().color = isUnlocked ? Color.white : Color.gray;
        lockedUI.SetActive(!isUnlocked);
    }

    public void SelectChapter(int chapterIndex)
    {
        if (chapterIndex <= unlockedChapters)
        {
            //Debug.Log("Loading Chapter: " + (chapterIndex + 1));
            SceneManager.LoadScene("Level " + (chapterIndex));
        }
    }

    public void CompleteChapter(int chapterIndex)
    {
        if (chapterIndex + 1 > unlockedChapters && chapterIndex + 1 < totalChapters)
        {
            unlockedChapters = chapterIndex + 1;
            SaveProgress();
            UpdateAllButtons();
        }
    }

    void LoadProgress()
    {
        unlockedChapters = PlayerPrefs.GetInt("UnlockedChapters", 1);
    }

    void SaveProgress()
    {
        PlayerPrefs.SetInt("UnlockedChapters", unlockedChapters);
        PlayerPrefs.Save();
    }

    void UpdateAllButtons()
    {
        for (int i = 0; i < chapterButtons.Count; i++)
        {
            bool isUnlocked = i < unlockedChapters;
            chapterButtons[i].interactable = isUnlocked;
            lockedUIElements[i].SetActive(!isUnlocked);
        }
    }
}
