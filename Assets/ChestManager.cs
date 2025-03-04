using UnityEngine;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using System.Collections;
using System;
using Random = UnityEngine.Random;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class ChestManager : MonoBehaviour
{
    public GameData EnemyStatsManager;
    [System.Serializable]
    public class LevelProbability
    {
        public int level;
        public List<AbilityProbability> abilityProbabilities;
    }

    [System.Serializable]
    public class AbilityProbability
    {
        public string abilityName;
        public int abilityID; // Unique ID for each ability
        [Range(0, 100)] public float probability; // Probability percentage
    }
    public int playerLevel = 1; // Current player level
    public List<LevelProbability> levelProbabilities; // Probability settings per level

    public List<AbilityProbability> GetProbabilitiesForLevel(int level)
    {
        foreach (var lvlProb in levelProbabilities)
        {
            if (lvlProb.level == level)
                return lvlProb.abilityProbabilities;
        }
        return null;
    }
    public List<CardDisplay> cardDisplays; // UI references for showing cards
    public AudioSource audioSource;
    public AudioClip CardMoveSFX;
    public AudioClip CardPopSFX;

    public GameObject CardsResetPos;


    public GameObject OpenChestIMG;
    public GameObject CloseChestIMG;
    bool chestOpened;
    public void OpenChest()
    {
        if (!chestOpened)
        {
            audioSource.PlayOneShot(OpenChestSFX);
            ResetCardPositions();
            StartCoroutine(DisplayChestRewards());
            chestOpened = true;
            OpenChestIMG.SetActive(true);
            CloseChestIMG.SetActive(false);
        }



    }
    public void ResetCardChest()
    {
        ResetCardPositions();
        GameManager.instance.CloseChestMenu();
        OpenChestIMG.SetActive(false);
        CloseChestIMG.SetActive(true);
        chestOpened = false;

        foreach (CardDisplay i in cardDisplays)
        {
            i.ResetCards();
        }
    }
    private IEnumerator DisplayChestRewards()
    {
        int level = playerLevel;
        List<AbilityProbability> probabilities = GetProbabilitiesForLevel(level);

        List<int> drawnAbilities = DrawCards(probabilities, 5); // Get 5 random cards



        for (int i = 0; i < drawnAbilities.Count; i++)
        {
            yield return new WaitForSeconds(0.2f); // Small delay for smooth animation
            MoveCard(drawnAbilities[i], i);
            cardDisplays[i].ShowCard(drawnAbilities[i]);
        }
    }

    private List<int> DrawCards(List<AbilityProbability> probabilities, int count)
    {
        List<int> drawnCards = new List<int>();
        for (int i = 0; i < count; i++)
        {
            drawnCards.Add(GetRandomAbility(probabilities));
        }
        return drawnCards;
    }

    private int GetRandomAbility(List<AbilityProbability> probabilities)
    {
        float totalWeight = 0;
        foreach (var prob in probabilities)
        {
            totalWeight += prob.probability;
        }

        float randomPoint = Random.Range(0, totalWeight);
        float currentSum = 0;

        foreach (var prob in probabilities)
        {
            currentSum += prob.probability;
            if (randomPoint <= currentSum)
            {
                return prob.abilityID;
            }
        }

        return probabilities[0].abilityID; // Default fallback
    }
    public List<GameObject> chestCards; // Cards inside the chest (pre-assigned)
    public List<Transform> cardHolderPositions; // Where the cards will go
    public AudioClip SuspenseRewardSFX;
    private void MoveCard(int abilityID, int cardIndex)
    {
        if (cardIndex >= chestCards.Count || cardIndex >= cardHolderPositions.Count) return;

        GameObject card = chestCards[cardIndex];
        Transform targetPosition = cardHolderPositions[cardIndex];

        card.transform.SetAsLastSibling(); // Bring card to front for visibility
        audioSource.PlayOneShot(CardMoveSFX, 0.4f);
        // Move the card from chest to the holder
        card.transform.DOMove(targetPosition.position, 0.1f).SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {

                LeanTween.delayedCall(1f, () => {
                    card.transform.DOScale(1.2f, 0.12f).SetEase(Ease.OutBack).OnComplete(() =>
                    {
                        audioSource.PlayOneShot(CardPopSFX, 0.4f);
                        card.transform.DOScale(1f, 0.1f);
                        //card.GetComponent<CardFlip>().slider.gameObject.SetActive(true);
                        //card.GetComponent<CardDisplay>().abilityNameText.gameObject.SetActive(true);
                        if (PlayerProgression.Instance.abilities[abilityID].unlocked && PlayerProgression.Instance.abilities[abilityID].cardsHave <= 10)
                        {
                            card.GetComponent<CardFlip>().shine.SetActive(true);
                            LeanTween.delayedCall(1f, () => {
                                GameManager.instance.OpenCardUnlockedScreen();
                                audioSource.PlayOneShot(SuspenseRewardSFX);
                            });

                            LeanTween.delayedCall(3.5f, () => { CardDisplayUnlocked[abilityID].ShowCard(abilityID); MoveCardUnlocked(abilityID, abilityID); });

                        }

                    });
                });
                // Pop-up effect after reaching position

            });
    }
    public void ResetCardPositions()
    {
        foreach (GameObject i in chestCards)
        {
            i.transform.position = CardsResetPos.transform.position;
            i.GetComponent<CardFlip>().slider.gameObject.SetActive(false);
            i.SetActive(false);
        }
       
    }
    public void CloseChest()
    {

        foreach (GameObject i in chestCards)
        {
            i.transform.position = CardsResetPos.transform.position;
            i.GetComponent<CardFlip>().slider.gameObject.SetActive(false);
            i.SetActive(false);
        }
        foreach (CardDisplay i in cardDisplays)
        {
            i.abilityNameText.gameObject.SetActive(false);
        }
    }
    public GameObject Moneytext;
    public AudioClip OpenChestSFX;
    public void BuyChest()
    {
        if (PlayerProgression.Instance.coins > 1000)
        {
            audioSource.PlayOneShot(PlayerProgression.Instance.BuySFX);
            PlayerProgression.Instance.coins -= 1000;
            EnemyStatsManager.enemyStats.coins = PlayerProgression.Instance.coins;
            GameManager.instance.OpenChestMenu();
            PlayerProgression.Instance.UpdateAllUI();
            EnemyStatsManager.SaveStats();

        }
        else
        {
            Moneytext.SetActive(true);
            LeanTween.delayedCall(3f, () => { Moneytext.SetActive(false); });

        }
    }
    bool opened;
    public GameObject freeChest;
    public GameObject freeChestOPened;
    public void OpenFreeChest()
    {
        if (!opened)
        {
            audioSource.PlayOneShot(PlayerProgression.Instance.BuySFX);
            EnemyStatsManager.enemyStats.coins = PlayerProgression.Instance.coins;
            GameManager.instance.OpenChestMenu();
            PlayerProgression.Instance.UpdateAllUI();
            for(int i = 0;i < 5;i++)
            {
                PlayerProgression.Instance.UpdateCardUI(i);
            }
            
            freeChest.SetActive(false);
            freeChestOPened.SetActive(true);
            opened = true;
        }


    }
    public GameObject RewardScreen;

    public void TakeRewards()
    {
        PlayerProgression.Instance.playerXP += 1000;
        PlayerProgression.Instance.coins += 1000;
        EnemyStatsManager.enemyStats.playerXP += 1000;
        EnemyStatsManager.enemyStats.coins += 1000;
       
       
        PlayerProgression.Instance.UpdatePlayerLevelUI();
        WhalePassAPI.instance.AddExp(1000);
        WhalePassAPI.instance.PlayerBaseResponse();
        playerLevel = WhalePassAPI.instance.currentLevel;
        EnemyStatsManager.SaveStats();
        RewardScreen.SetActive(false);

    }

    [Header("CardUnlocked")]
    public List<GameObject> chestCardsUnlocked; // Cards inside the chest (pre-assigned)
    public List<Transform> cardHolderPositionsUnlocked; // Where the cards will go
    public List<CardDisplay> CardDisplayUnlocked; // Where the cards will go
    public List<GameObject> ClaimButtons;

    private void Update()
    {
       
    }

    public void CardUnlocked(int index)
    {
        LeanTween.delayedCall(0.2f, () => { CardDisplayUnlocked[index].ShowCard(index); MoveCardUnlocked(index, index); });  
    }
    public void MoveCardUnlocked(int abilityID, int cardIndex)
    {
        if (cardIndex >= chestCardsUnlocked.Count || cardIndex >= cardHolderPositionsUnlocked.Count) return;

        GameObject card = chestCardsUnlocked[cardIndex];
        Transform targetPosition = cardHolderPositionsUnlocked[cardIndex];

        card.transform.SetAsLastSibling(); // Bring card to front for visibility
        audioSource.PlayOneShot(CardMoveSFX, 0.4f);
        // Move the card from chest to the holder
        card.transform.DOMove(targetPosition.position, 0.1f).SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {

                LeanTween.delayedCall(1f, () => {
                    card.transform.DOScale(1.2f, 0.12f).SetEase(Ease.OutBack).OnComplete(() =>
                    {
                        audioSource.PlayOneShot(CardPopSFX, 0.4f);
                        ClaimButtons[abilityID].SetActive(true);
                        card.transform.DOScale(1f, 0.1f);
                        card.GetComponent<CardDisplay>().abilityNameText.gameObject.SetActive(true);
                       

                    });
                });
                // Pop-up effect after reaching position

            });
    }
   
    public void ResetCardPositionsUnlocked()
    {
        foreach (GameObject i in chestCardsUnlocked)
        {
            i.transform.position = CardsResetPos.transform.position;
            i.GetComponent<CardFlip>().slider.gameObject.SetActive(false);
            i.SetActive(false);
        }
        foreach (CardDisplay i in CardDisplayUnlocked)
        {
            i.abilityNameText.gameObject.SetActive(false);
        }
    }
}
