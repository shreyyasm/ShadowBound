using UnityEngine;
using TMPro;
using DG.Tweening;

public class CardDisplay : MonoBehaviour
{
    public TextMeshProUGUI abilityNameText;
    public GameObject cardVisual;
    private Vector3 originalScale;

    private void Start()
    {
        originalScale = cardVisual.transform.localScale;
        cardVisual.SetActive(false);
    }

    public void ShowCard(int abilityID)
    {
        cardVisual.SetActive(true);
        abilityNameText.text = "Ability " + abilityID;

        // Pop-up animation
        cardVisual.transform.localScale = Vector3.zero;
        cardVisual.transform.DOScale(originalScale, 0.3f).SetEase(Ease.OutBack);
    }
}
