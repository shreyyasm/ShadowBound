using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;
public class PlayerProgression : MonoBehaviour
{
    public GameStats stats;
    [System.Serializable]
    public class Ability
    {
        [Header("Ability Stats")]
        public string name;
        public int cardsHave;
        public int cardsNeed;
        public bool unlocked;

        [Header("Ability UI")]
        public GameObject MainCard;
        public Slider cardSlider;
        public TextMeshProUGUI cardsText;
        public GameObject BuyButton;
        public GameObject UnlockedText;
        public GameObject Shine;
    }

    public int playerLevel = 1;
    public int playerXP = 0;
    public int coins = 0;
    public List<int> xpThresholds = new List<int> { 0, 100, 300, 600, 1000 };
    public List<Ability> abilities = new List<Ability>();
    public TextMeshProUGUI levelText, xpText, coinText;
    public GameObject chestPanel, shopPanel, chestRewardContainer;
    public GameObject abilityCardPrefab;

    public AudioSource audioSource;
    public AudioClip BuySFX;
    public AudioClip UnlockedSFX;
    private void Awake()
    {
        LoadGameData();
    }
    public void LoadGameData()
    {
        playerLevel = stats.PlayerLevel;
        playerXP = stats.PlayerXP;

        coins = stats.Coins;

        abilities[0].cardsHave = stats.Ability1Cards;
        abilities[1].cardsHave = stats.Ability2Cards;
        abilities[2].cardsHave = stats.Ability3Cards;
        abilities[3].cardsHave = stats.Ability4Cards;
        abilities[4].cardsHave = stats.Ability5Cards;
        //UpdateAllUI();


    }
    void Start()
    {
        UpdateUI();
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GainXP(100);
        }
    }
    public void GainXP(int amount)
    {
        playerXP += amount;
        CheckLevelUp();
        UpdateUI();
        stats.PlayerXP = playerXP;
    }

    void CheckLevelUp()
    {
        while (playerLevel < xpThresholds.Count && playerXP >= xpThresholds[playerLevel])
        {
            playerLevel++;
            coins += 50; // Reward on level up
            stats.Coins = coins;
            stats.PlayerLevel = playerLevel;
            stats.PlayerXP = playerXP;
        }
    }

  
    public void OpenChest()
    {
        chestPanel.SetActive(true);
        chestRewardContainer.SetActive(true);
        for (int i = 0; i < 3; i++)
        {
            int abilityIndex = GetRandomAbilityIndex();
            GameObject card = Instantiate(abilityCardPrefab, chestRewardContainer.transform);
            card.GetComponentInChildren<Text>().text = abilities[abilityIndex].name;
        }
        UpdateUI();
    }

    int GetRandomAbilityIndex()
    {
        int maxIndex = Mathf.Clamp(playerLevel, 1, abilities.Count) - 1;
        return UnityEngine.Random.Range(0, maxIndex + 1);
    }

    void UpdateUI()
    {
        levelText.text = "Level: " + playerLevel;
        xpText.text = "XP: " + playerXP +"/" + xpThresholds[playerLevel];
        coinText.text = "Coins: " + coins;
    }

   
    public float popScale = 1.2f;
    public float popDuration = 0.3f;

    public void BuyCard(int abilityIndex)
    {
        if (coins >= 100)
        {
            coins -= 100;
            stats.Coins = coins;
            AddCard(abilityIndex);
            audioSource.PlayOneShot(BuySFX);
           
        }
    }


    public void AddCard( int index)
    {

        if (abilities[index].cardsHave < abilities[index].cardsNeed)
        {
           
           abilities[index].cardsHave++;
            StartCoroutine(PopEffect());
            UpdateCardUI(index);
            stats.Ability1Cards = abilities[index].cardsHave;
        }
        if (abilities[index].cardsHave == abilities[index].cardsNeed)
        {
            abilities[index].BuyButton.SetActive(false);
            abilities[index].UnlockedText.SetActive(true);
            abilities[index].cardSlider.gameObject.SetActive(false);
            abilities[index].Shine.gameObject.SetActive(true);
            audioSource.PlayOneShot(UnlockedSFX);
        }


    }

    IEnumerator PopEffect()
    {
        Vector3 originalScale = abilities[0].MainCard.transform.localScale;
        Vector3 originalScaleSlider = abilities[0].cardSlider.transform.localScale;
        Vector3 targetScale = originalScale * popScale;
        Vector3 targetScaleSlider = originalScaleSlider * popScale;
        float elapsedTime = 0f;

        while (elapsedTime < popDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / popDuration);
            abilities[0].MainCard.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            abilities[0].cardSlider.transform.localScale = Vector3.Lerp(originalScaleSlider, targetScale, t);
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < popDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / popDuration);
            abilities[0].MainCard.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            abilities[0].cardSlider.transform.localScale = Vector3.Lerp(targetScaleSlider, originalScale, t);
            yield return null;
        }
    }

    void UpdateCardUI(int index)
    {
        abilities[index].cardSlider.value = (float)abilities[index].cardsHave / abilities[index].cardsNeed;
        abilities[index].cardsText.text = abilities[index].cardsHave + "/" + abilities[index].cardsNeed;
    }
    public  void UpdateAllUI()
    {
        for (int index = 0; index < abilities.Count; index++)
        {
            abilities[index].cardSlider.value = (float)abilities[index].cardsHave / abilities[index].cardsNeed;
            abilities[index].cardsText.text = abilities[index].cardsHave + "/" + abilities[index].cardsNeed;
            if (abilities[index].cardsHave == abilities[index].cardsNeed)
            {
                abilities[index].BuyButton.SetActive(false);
                abilities[index].UnlockedText.SetActive(true);
                abilities[index].cardSlider.gameObject.SetActive(false);
                abilities[index].Shine.gameObject.SetActive(true);
            }
        }
    }
  
}
