using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;
public class PlayerProgression : MonoBehaviour
{
    public static PlayerProgression Instance;
    public GameData stats;
    [System.Serializable]
    public class Ability
    {
        [Header("Ability Stats")]
        public string name;
        public int cardsHave;
        public int cardsNeed;
        public bool unlocked;
        public int cardvalue;

        [Header("Ability UI")]
        public GameObject MainCard;
        public Slider cardSlider;
        public TextMeshProUGUI cardsText;
        public GameObject BuyButton;
        public GameObject UnlockedText;
        public GameObject Shine;
        public TextMeshProUGUI BuyText;
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
        Instance = this;
       
    }
    void Start()
    {
        LoadGameData();
        UpdateUI();
        UpdateInventory();
    }
    public void LoadGameData()
    {
        playerLevel = stats.enemyStats.LevelIndex;
        playerXP = stats.enemyStats.playerXP;

        coins = stats.enemyStats.coins;
        abilities[0].unlocked = stats.enemyStats.AbilityUnlocked[0];
        abilities[1].unlocked = stats.enemyStats.AbilityUnlocked[1];
        abilities[2].unlocked = stats.enemyStats.AbilityUnlocked[2];
        abilities[3].unlocked = stats.enemyStats.AbilityUnlocked[3];
        abilities[4].unlocked = stats.enemyStats.AbilityUnlocked[4];
        abilities[0].cardsHave = stats.enemyStats.AbilityCardsHave[0];
        abilities[1].cardsHave = stats.enemyStats.AbilityCardsHave[1];
        abilities[2].cardsHave = stats.enemyStats.AbilityCardsHave[2];
        abilities[3].cardsHave = stats.enemyStats.AbilityCardsHave[3];
        abilities[4].cardsHave = stats.enemyStats.AbilityCardsHave[4];
        UpdateAllUI();
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
        stats.enemyStats.playerXP = playerXP;
        stats.SaveStats();
    }

    void CheckLevelUp()
    {
        while (playerLevel < xpThresholds.Count && playerXP >= xpThresholds[playerLevel])
        {
            playerLevel++;
            coins += 50; // Reward on level up
            stats.enemyStats.coins = coins;
            stats.enemyStats.LevelIndex = playerLevel;
            stats.enemyStats.playerXP = playerXP;
            stats.SaveStats();
        }
    }

    void UpdateUI()
    {
        levelText.text = "Level: " + playerLevel;
        xpText.text = "XP: " + playerXP +"/" + xpThresholds[playerLevel];
        coinText.text = "Coins: " + coins;
    }

   
    public float popScale = 1.2f;
    public float popDuration = 0.3f;
    public GameObject NotEnoughMoney;

    public void BuyCard(int abilityIndex)
    {
        if (coins >= abilities[abilityIndex].cardvalue)
        {
            coins -= abilities[abilityIndex].cardvalue;
            stats.enemyStats.coins = coins;
            AddCard(abilityIndex);
            audioSource.PlayOneShot(BuySFX);
            stats.SaveStats();

        }
        else
        {
            NotEnoughMoney.SetActive(true);
            LeanTween.delayedCall(3f, () => { NotEnoughMoney.SetActive(false); });
        }
    }

    public void AddcardFromChest(int abilityIndex)
    {
            AddCard(abilityIndex);
    }
    public void AddCard( int index)
    {

        if (abilities[index].cardsHave < abilities[index].cardsNeed)
        {
           
            abilities[index].cardsHave++;
            StartCoroutine(PopEffect());
            //UpdateCardUI(index);
            stats.enemyStats.AbilityCardsHave[index] = abilities[index].cardsHave;
            stats.SaveStats();
            if (abilities[index].cardsHave >= abilities[index].cardsNeed)
            {
                abilities[index].unlocked = true;
                stats.enemyStats.AbilityUnlocked[index] = true;
                abilities[index].BuyButton.SetActive(false);
                abilities[index].UnlockedText.SetActive(true);
                abilities[index].cardSlider.gameObject.SetActive(false);
                abilities[index].Shine.gameObject.SetActive(true);
                audioSource.PlayOneShot(UnlockedSFX);
                stats.SaveStats();
            }
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
            Debug.Log(abilities[index].BuyText);
            //abilities[index].BuyText.text = "Buy " + abilities[index].cardvalue;
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
    public List<GameObject> Inventory;
    public void UpdateInventory()
    {
        for(int index = 0;index < Inventory.Count;index++)
        {
            if(abilities[index].unlocked)
            {
                Inventory[index].SetActive(true);
            }
        }
    }
}
