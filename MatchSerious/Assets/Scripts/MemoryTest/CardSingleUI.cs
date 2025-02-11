using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardSingleUI : MonoBehaviour
{
    private CardGroup cardGroup; // Reference to the parent CardGroup to handle card interactions

    [SerializeField] private Button cardBackButton; // The button associated with the card back
    [SerializeField] private Image cardBackBackground; // Background image for the card back
    [SerializeField] private Image cardFrontBackground; // Background image for the card front
    [SerializeField] private Image cardFrontImage; // Image displayed on the front of the card

    [SerializeField] private GameObject cardBack; // The GameObject for the card back
    [SerializeField] private GameObject cardFront; // The GameObject for the card front

    private bool objectMatch; // Flag to track whether the card has been matched

    [Header("DoTween Animation")]
    [SerializeField] private Vector3 selectRotation = new Vector3(); // Rotation for when the card is selected
    [SerializeField] private Vector3 deselectRotation = new Vector3(); // Rotation for when the card is deselected
    [SerializeField] private float duration = 0.25f; // Duration for card rotation animation
    private Tweener[] tweener = new Tweener[3]; // Array to store Tween animations for rotation

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Ensure the cardGroup reference is set by finding it in the parent
        if (cardGroup == null)
        {
            cardGroup = transform.parent.GetComponent<CardGroup>();
        }

        // Subscribe this card to the cardGroup if it's found
        if (cardGroup != null)
        {
            cardGroup.Subscribe(this);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Add listener to handle when the cardBack button is clicked
        cardBackButton.onClick.AddListener(OnClick);

        // Set initial rotation so the card starts as face down (back side up)
        transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));

        // Start the coroutine to handle hiding the card after a delay
        StartCoroutine(WaitingToHide());

        // Subscribe to the MemoryGameManager to receive updates
        MemoryGameManagerUI.Instance.Subscribe(this);
    }

    // This method is triggered when the cardBack button is clicked
    private void OnClick()
    {
        // Notify the CardGroup that this card has been selected
        cardGroup.OnCardSelected(this);
    }

    // Method to select the card (animate flipping to front)
    public void Select()
    {
        // Animate the card rotation to select it (flip it over to show front)
        tweener[0] = transform.DORotate(selectRotation, duration)
            .SetEase(Ease.InOutElastic)
            .OnUpdate(CheckSelectHalfDuration); // Check if half of the rotation is completed
    }

    // Method to deselect the card (animate flipping to back)
    public void Deselect()
    {
        // Animate the card rotation to deselect it (flip it back to show the back)
        tweener[1] = transform.DORotate(deselectRotation, duration)
            .SetEase(Ease.InOutElastic)
            .OnUpdate(CheckDeselectHalfDuration); // Check if half of the rotation is completed
    }

    // Coroutine to handle the card hiding logic after a 3-second delay
    private IEnumerator WaitingToHide()
    {
        yield return new WaitForSeconds(3f); // Wait for 3 seconds

        // Animate the card to deselect after waiting
        tweener[2] = transform.DORotate(deselectRotation, duration)
            .SetEase(Ease.InOutElastic)
            .OnUpdate(CheckWaitingToHide); // Check if half of the rotation is completed
    }

    // Checks if the deselection animation (waiting to hide) has passed half its duration
    private void CheckWaitingToHide()
    {
        float elapsed = tweener[2].Elapsed(); // Get the elapsed time of the current tween

        float halfDuration = tweener[2].Duration() / 2f; // Calculate half the duration

        if (elapsed >= halfDuration) // If half the time has passed
        {
            cardFront.SetActive(false); // Hide the front of the card
            cardBack.SetActive(true); // Show the back of the card
        }
    }

    // Checks if the selection animation has passed half its duration
    private void CheckSelectHalfDuration()
    {
        float elapsed = tweener[0].Elapsed(); // Get the elapsed time of the current tween

        float halfDuration = tweener[0].Duration() / 2f; // Calculate half the duration

        if (elapsed >= halfDuration) // If half the time has passed
        {
            cardBack.SetActive(false); // Hide the back of the card
            cardFront.SetActive(true); // Show the front of the card
        }
    }

    // Checks if the deselection animation has passed half its duration
    private void CheckDeselectHalfDuration()
    {
        float elapsed = tweener[1].Elapsed(); // Get the elapsed time of the current tween

        float halfDuration = tweener[1].Duration() / 2f; // Calculate half the duration

        if (elapsed >= halfDuration) // If half the time has passed
        {
            cardFront.SetActive(false); // Hide the front of the card
            cardBack.SetActive(true); // Show the back of the card
        }
    }

    // Getter for the card's back background image
    public Image GetCardBackBackground() => cardBackBackground;

    // Getter for the card's front background image
    public Image GetCardFrontBackground() => cardFrontBackground;

    // Method to mark the card as a match
    public void SetObjectMatch()
    {
        objectMatch = true; // Set the objectMatch flag to true
    }

    // Method to set the image for the card's front
    public void SetCardImage(Sprite sprite)
    {
        cardFrontImage.sprite = sprite; // Set the sprite for the front of the card
    }

    // Getter to check if the card has been matched
    public bool GetObjectMatch() => objectMatch;

    // Disable interaction with the cardBack button
    public void DisableCardBackButton() => cardBackButton.interactable = false;
}
