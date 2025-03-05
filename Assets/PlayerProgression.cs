using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;
using static PlayerProgression;
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

    public List<GameObject> ClaimCoinsButtons;
    private void Awake()
    {
        Instance = this;
       
    }
    void Start()
    {
        LoadGameData();
        UpdatePlayerLevelUI();
        //UpdateInventory();
    }
    public void LoadGameData()
    {
        playerLevel = WhalePassAPI.instance.currentLevel;
        playerXP = WhalePassAPI.instance.CurrentTotalExp;

        coins = stats.enemyStats.coins;
        coinText.text = "Coins:" + coins.ToString();
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
       
        LeanTween.delayedCall(1f, () => { 
            UpdatePlayerLevelUI(); 
            if(xpSlider != null)
                xpSlider.value = WhalePassAPI.instance.CurrentExp; 
            if(xpMegaSlider != null)
                xpMegaSlider.value = WhalePassAPI.instance.CurrentTotalExp; 
            CheckLevelUp(); });
       
    }
    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Space))
        //{
        //    GainXP(100);
        //}
    }
    public void GainXP(int amount)
    {
        coinText.text = "Coins:" + coins.ToString();
        playerXP += amount;
        CheckLevelUp();
        UpdatePlayerLevelUI();
        stats.enemyStats.playerXP = playerXP;
        WhalePassAPI.instance.AddExp(amount);
        WhalePassAPI.instance.PlayerBaseResponse();
        playerLevel = WhalePassAPI.instance.currentLevel;
        stats.SaveStats();
    }

    public Slider xpSlider;
    public Slider xpMegaSlider; // Slider to visually represent XP progress
    public void UpdatePlayerLevelUI()
    {
        if (xpSlider != null)
        {
            if (WhalePassAPI.instance.CurrentLevel < 8)
            {
                playerLevel = WhalePassAPI.instance.currentLevel;
                levelText.text = $"Level: {WhalePassAPI.instance.CurrentLevel}";
                xpText.text = $"Next Level: {WhalePassAPI.instance.CurrentExp} / {WhalePassAPI.instance.NextLevelExp - WhalePassAPI.instance.ExpRequiredLastlevel}";
                xpSlider.value = WhalePassAPI.instance.CurrentExp;
                if( xpMegaSlider != null )
                    xpMegaSlider.value = WhalePassAPI.instance.CurrentTotalExp;
                xpSlider.maxValue = WhalePassAPI.instance.NextLevelExp;
                coinText.text = "Coins:" + coins.ToString();
            }
            else
            {
                levelText.text = $"Level: {WhalePassAPI.instance.CurrentLevel}";
                xpText.text = $"Levels Completed";
            }

        }

    }

    void CheckLevelUp()
    {
        if(ClaimCoinsButtons.Count != 0)
        {
            for (int i = 0; i < playerLevel; i++)
            {

                ClaimCoinsButtons[i].GetComponent<Button>().interactable = true;
                LeanTween.delayedCall(3f, () => {
                    UpdatePlayerLevelUI();
                    if (xpSlider != null)
                        xpSlider.value = WhalePassAPI.instance.CurrentExp;
                    if (xpMegaSlider != null)
                        xpMegaSlider.value = WhalePassAPI.instance.CurrentTotalExp;
                });
            }
        }
       
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
            coinText.text = "Coins:" +coins.ToString();
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
    public ChestManager ChestManager;
    public AudioClip suspenseRewardSFX;
    int tempcardIndex;
    public void AddCard( int index)
    {

        if (abilities[index].cardsHave < abilities[index].cardsNeed)
        {
           
            abilities[index].cardsHave++;
            tempcardIndex = index;
            StartCoroutine(PopEffect());
            UpdateCardUI(index);
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

                LeanTween.delayedCall(1f, () => {
                    GameManager.instance.OpenCardUnlockedScreen();
                    audioSource.PlayOneShot(suspenseRewardSFX);
                });
               
                LeanTween.delayedCall(3.5f, () => { ChestManager.CardDisplayUnlocked[index].ShowCard(index); ChestManager.MoveCardUnlocked(index, index); });
            }
        }
    }
    IEnumerator PopEffect()
    {
        Vector3 originalScale = abilities[tempcardIndex].MainCard.transform.localScale;
        Vector3 originalScaleSlider = abilities[tempcardIndex].cardSlider.transform.localScale;
        Vector3 targetScale = originalScale * popScale;
        Vector3 targetScaleSlider = originalScaleSlider * popScale;
        float elapsedTime = 0f;

        while (elapsedTime < popDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / popDuration);
            abilities[tempcardIndex].MainCard.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            abilities[tempcardIndex].cardSlider.transform.localScale = Vector3.Lerp(originalScaleSlider, targetScale, t);
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < popDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / popDuration);
            abilities[tempcardIndex].MainCard.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            abilities[tempcardIndex].cardSlider.transform.localScale = Vector3.Lerp(targetScaleSlider, originalScale, t);
            yield return null;
        }
        abilities[tempcardIndex].MainCard.transform.localScale = new Vector3(1,1,1);
        abilities[tempcardIndex].cardSlider.transform.localScale = new Vector3(1, 1, 1);
    }

    public void UpdateCardUI(int index)
    {
        abilities[index].cardSlider.value = (float)abilities[index].cardsHave / abilities[index].cardsNeed;
        abilities[index].cardsText.text = abilities[index].cardsHave + "/" + abilities[index].cardsNeed;
    }
    public  void UpdateAllUI()
    {
        for (int index = 0; index < abilities.Count; index++)
        {
            //Debug.Log(abilities[index].BuyText);
            //abilities[index].BuyText.text = "Buy " + abilities[index].cardvalue;
            abilities[index].cardSlider.value = (float)abilities[index].cardsHave / abilities[index].cardsNeed;
            abilities[index].cardsText.text = abilities[index].cardsHave + "/" + abilities[index].cardsNeed;
            if (abilities[index].cardsHave >= abilities[index].cardsNeed)
            {
                abilities[index].BuyButton.SetActive(false);
                abilities[index].UnlockedText.SetActive(true);
                abilities[index].cardSlider.gameObject.SetActive(false);
                abilities[index].Shine.gameObject.SetActive(true);
               
            }
        }
    }
  
}
