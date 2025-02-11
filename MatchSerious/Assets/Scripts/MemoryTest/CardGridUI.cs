using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardGridUI : MonoBehaviour
{
    // Define a Card class to hold card's name and image
    [System.Serializable]
    public class Card
    {
        public string cardName; // The name of the card
        public Sprite cardImage; // The image associated with the card
    }

    // Serialize and make these lists editable in the Unity Inspector
    [SerializeField] private List<Card> cardList = new List<Card>(); // List to store all available cards
    [SerializeField] private List<Card> cardListToSort = new List<Card>(); // List to store cards that need to be sorted
    [SerializeField] private Transform cardContainer; // The container where cards will be placed in the UI
    [SerializeField] private Transform cardPrefab; // The prefab used to instantiate each card in the grid

    // Start is called before the first frame update
    private void Start()
    {
        cardPrefab.gameObject.SetActive(false); // Hide the cardPrefab initially, as we will instantiate it dynamically
    }

    // OnEnable is called when the object becomes active
    private void OnEnable()
    {
        FillGrid(); // Fill the grid with cards when the object is enabled
    }

    // Function to fill the grid with cards
    private void FillGrid()
    {
        int cardsToShow = 0;

        // Determine how many cards to show based on the selected difficulty
        switch (MemoryGameManagerUI.Instance.GetDifficulty())
        {
            case DifficultyEnum.Easy:
                cardsToShow = 6; // For easy difficulty, show 6 cards
                break;
            case DifficultyEnum.Normal:
                cardsToShow = 6; // For normal difficulty, show 6 cards
                break;
            case DifficultyEnum.Hard:
                cardsToShow = 6; // For hard difficulty, show 6 cards
                break;
            default:
                break; // Default case if no difficulty is selected
        }

        // Add each card twice to the list (so they can be matched in pairs)
        for (int i = 0; i < cardsToShow; i++)
        {
            cardListToSort.Add(cardList[i]); // Add the card
            cardListToSort.Add(cardList[i]); // Add the card again for matching
        }

        // Create a new instance of System.Random to randomize the order of the cards
        System.Random rnd = new System.Random();

        // Shuffle the card list randomly
        IOrderedEnumerable<Card> randomized = cardListToSort.OrderBy(i => rnd.Next());

        // Iterate through the randomized list of cards
        foreach (Card card in randomized)
        {
            // Instantiate a new cardPrefab for each card in the shuffled list
            Transform cardTransform = Instantiate(cardPrefab, cardContainer);
            cardTransform.gameObject.SetActive(true); // Make the newly instantiated card visible
            cardTransform.name = card.cardName; // Set the name of the card to the card's name
            cardTransform.GetComponent<CardSingleUI>().SetCardImage(card.cardImage); // Set the card's image
        }
    }
}
