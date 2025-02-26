using UnityEngine;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using System.Collections;

public class ChestManager : MonoBehaviour
{
    
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


    public void OpenChest()
    {
        StartCoroutine(DisplayChestRewards());
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
    private void MoveCard(int abilityID, int cardIndex)
    {
        if (cardIndex >= chestCards.Count || cardIndex >= cardHolderPositions.Count) return;

        GameObject card = chestCards[cardIndex];
        Transform targetPosition = cardHolderPositions[cardIndex];

        card.transform.SetAsLastSibling(); // Bring card to front for visibility

        // Move the card from chest to the holder
        card.transform.DOMove(targetPosition.position, 0.1f).SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                LeanTween.delayedCall(1f, () => {
                    card.transform.DOScale(1.2f, 0.15f).SetEase(Ease.OutBack).OnComplete(() =>
                    {

                        card.transform.DOScale(1f, 0.1f);
                    });
                });
                // Pop-up effect after reaching position
               
            });
    }
}
