using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static Abilities;
using static UnityEngine.Rendering.DebugUI;

public class hoverButtons : MonoBehaviour
{
    public static hoverButtons instance;
    public List<Transform> Buttons; // Drag UI Card Transforms in Inspector
    public RectTransform container; // Assign the UI container in the Inspector
    public Transform CardsResetPos;

    public Vector3 selectedScale = new Vector3(1.2f, 1.2f, 1f);
    public Vector3 normalScale = Vector3.one;
    private int previousAbilityIndex = -1;
    public float spacing = 100f; // Adjust for better alignment
    public AudioClip abilityChangeSound;

    public AudioSource audioSource;
    public int currentAbilityIndex = 0;
    public List<GameObject> Playerabilities = new List<GameObject>();
    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        //HandleAbilitySelection();
        UpdateCardUI();
    }
    //void HandleAbilitySelection()
    //{
    //    float scroll = Input.GetAxis("Mouse ScrollWheel");
    //    if (scroll != 0 && Playerabilities.Count > 0)
    //    {

    //        // Find the next unlocked ability
    //        int newIndex = currentAbilityIndex;
    //        do
    //        {
    //            newIndex = (newIndex + (scroll > 0 ? 1 : -1) + Playerabilities.Count) % Playerabilities.Count;
    //            audioSource.PlayOneShot(abilityChangeSound, 0.5f);


    //        } while (!Playerabilities[newIndex].activeInHierarchy);

    //        // Update ability state
    //        currentAbilityIndex = newIndex;


    //        if (newIndex != currentAbilityIndex)
    //        {
    //            // Reset previous card scale
    //            if (previousAbilityIndex >= 0 && previousAbilityIndex < Buttons.Count)
    //            {
    //                Buttons[previousAbilityIndex].localScale = normalScale;
    //            }

    //            // Scale up new selected card
    //            if (newIndex < Buttons.Count)
    //            {
    //                Buttons[newIndex].localScale = selectedScale;
    //            }

    //            previousAbilityIndex = currentAbilityIndex;
    //            currentAbilityIndex = newIndex;
    //        }
    //    }


    //}
    public void HoverSound()
    {
        audioSource.PlayOneShot(abilityChangeSound, 0.3f);
    }
    public void UpdateCardUI()
    {
        // List to store only active (unlocked) ability cards and their original indices
        List<Transform> activeCards = new List<Transform>();
        List<int> activeIndices = new List<int>(); // Store original indices

        for (int i = 0; i < Playerabilities.Count; i++)
        { 
            if (Playerabilities[i].activeInHierarchy && i < Buttons.Count)
            {
                activeCards.Add(Buttons[i]);
                activeIndices.Add(i); // Keep track of original index
            }
        }

        if (activeCards.Count == 0) return; // Exit if no active cards

        // Find the correct index of the selected ability within activeCards
        int selectedActiveIndex = activeIndices.IndexOf(currentAbilityIndex);

        // Calculate total height of active cards
        float totalHeight = 0f;
        List<float> cardHeights = new List<float>();

        for (int i = 0; i < activeCards.Count; i++)
        {
            bool isSelected = (i == selectedActiveIndex); // Check against mapped index
            Vector3 targetScale = isSelected ? selectedScale : normalScale;

            // Smoothly transition scale
            activeCards[i].localScale = Vector3.Lerp(activeCards[i].localScale, targetScale, Time.deltaTime * 10f);

            // Store height for alignment calculation
            float cardHeight = (isSelected ? selectedScale.y : normalScale.y) * activeCards[i].GetComponent<RectTransform>().sizeDelta.y;
            cardHeights.Add(cardHeight);
            totalHeight += cardHeight;
        }

        // Add spacing between cards
        totalHeight += (activeCards.Count - 1) * spacing;

        // Align cards at the center of the container vertically
        float containerTopEdge = container.rect.height / 2f;
        float startY = containerTopEdge - totalHeight / 2f;

        for (int i = 0; i < activeCards.Count; i++)
        {
            bool isSelected = (i == selectedActiveIndex);
            float cardHeight = cardHeights[i];

            // Compute target position
            Vector3 targetPosition = new Vector3(activeCards[i].localPosition.x, startY - cardHeight / 2, activeCards[i].localPosition.z);

            // Smoothly move the cards
            activeCards[i].localPosition = Vector3.Lerp(activeCards[i].localPosition, targetPosition, Time.deltaTime * 10f);

            startY -= cardHeight + spacing; // Move to next position downward
        }
    }
}

