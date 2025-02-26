using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

public class CardDisplay : MonoBehaviour
{
    public TextMeshProUGUI abilityNameText;
    public GameObject cardVisual;
    private Vector3 originalScale;
    public List<GameObject> CardFront;
    public List<GameObject> CardBack;

    public CardFlip cardFlip;

    private void Start()
    {
        originalScale = cardVisual.transform.localScale;
        cardVisual.SetActive(false);
    }

    public void ResetCards()
    {
        foreach(GameObject i in CardFront)
        {
            i.SetActive(false);
        }
        foreach(GameObject i in CardBack)
        {
            i.SetActive(false);
        }
    }
    public void ShowCard(int abilityID)
    {
        cardVisual.SetActive(true);
        abilityNameText.text = "Ability " + abilityID;
        CardFront[abilityID].SetActive(true);
        cardFlip.frontCard =   CardFront[abilityID];
        cardFlip.backCard = CardBack[abilityID];

        // Pop-up animation
        cardVisual.transform.localScale = Vector3.zero;
        cardVisual.transform.DOScale(originalScale, 0.3f).SetEase(Ease.OutBack);
        PlayerProgression.Instance.AddcardFromChest(abilityID);
        
    }
}
