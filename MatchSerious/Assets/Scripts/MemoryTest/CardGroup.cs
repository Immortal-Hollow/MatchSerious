using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGroup : MonoBehaviour
{
    [SerializeField] private List<CardSingleUI> cardSingleUIList = new List<CardSingleUI>(); // List to hold all card objects in the group
    [SerializeField] private List<CardSingleUI> selectedCardList = new List<CardSingleUI>(); // List to hold selected cards for comparison

    [SerializeField] private Sprite cardIdle; // The sprite displayed when the card is in its idle state (back side)
    [SerializeField] private Sprite cardActive; // The sprite displayed when the card is active (front side)

    public event EventHandler OnCardMatch; // Event triggered when a match is found

    // Method to subscribe a CardSingleUI to the card group
    public void Subscribe(CardSingleUI cardSingleUI)
    {
        // Initialize the cardSingleUIList if it is null
        if (cardSingleUIList == null)
        {
            cardSingleUIList = new List<CardSingleUI>();
        }

        // Add the card to the list if it is not already added
        if (!cardSingleUIList.Contains(cardSingleUI))
        {
            cardSingleUIList.Add(cardSingleUI);
        }
    }

    // Method to handle a card being selected
    public void OnCardSelected(CardSingleUI cardSingleUI)
    {
        // Add the selected card to the list of selected cards
        selectedCardList.Add(cardSingleUI);

        // Animate the card to show its front side (flipping)
        cardSingleUI.Select();

        // Change the background sprite of the card to the active state
        cardSingleUI.GetCardFrontBackground().sprite = cardActive;

        // Check if two cards are selected
        if (selectedCardList.Count == 2)
        {
            // If they match, mark them as matched
            if (CheckIfMatch())
            {
                // Disable interaction and mark as matched for both cards
                foreach (CardSingleUI cardSingle in selectedCardList)
                {
                    cardSingle.DisableCardBackButton();
                    cardSingle.SetObjectMatch(); // Mark the card as matched
                }

                // Clear the selected cards list after a match
                selectedCardList = new List<CardSingleUI>();

                // Trigger the OnCardMatch event
                OnCardMatch?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                // If they do not match, run the coroutine to reset the cards
                StartCoroutine(DontMatch());
            }
        }

        // Reset the background of selected cards after the operation
        ResetTabs();
    }

    // Method to reset the background of the selected cards to idle
    public void ResetTabs()
    {
        // Loop through all selected cards and reset their background sprite to idle
        foreach (CardSingleUI cardSingleUI in selectedCardList)
        {
            // Only reset if there are fewer than 3 cards selected (we expect only 2 cards at most)
            if (selectedCardList != null && selectedCardList.Count < 3) continue;

            // Set the card's back background to the idle state
            cardSingleUI.GetCardBackBackground().sprite = cardIdle;
        }
    }

    // Coroutine to handle the scenario when two cards do not match
    private IEnumerator DontMatch()
    {
        // Wait for 0.5 seconds before resetting the cards
        yield return new WaitForSeconds(0.5f);

        // Deselect the cards if they do not match
        foreach (CardSingleUI cardSingleUI in selectedCardList)
        {
            cardSingleUI.Deselect(); // Animate the card to flip back
        }

        // Clear the selected cards list after deselection
        selectedCardList = new List<CardSingleUI>();
    }

    // Method to check if the selected cards match
    private bool CheckIfMatch()
    {
        // Get the first card in the selected list
        CardSingleUI firstCard = selectedCardList[0];

        // Compare the selected cards to check if they have the same name
        foreach (CardSingleUI cardSingleUI in selectedCardList)
        {
            // If any of the selected cards has a different name, they don't match
            if (cardSingleUI.name != firstCard.name)
            {
                return false;
            }
        }

        // If all selected cards match, return true
        return true;
    }
}
