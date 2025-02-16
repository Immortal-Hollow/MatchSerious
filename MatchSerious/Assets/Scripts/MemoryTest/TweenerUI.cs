using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// Enum defining possible animation types
public enum UIAnimationType
{
    MoveTransform,  // Move UI element
    Scale,          // Scale UI element
    ScaleX,         // Scale UI element only on X axis
    ScaleY,         // Scale UI element only on Y axis
    Fade,           // Fade UI element in or out
    Size,           // Change size of the UI element
    Rotation        // Rotate UI element
}

public class TweenerUI : MonoBehaviour
{
    public GameObject objectToAnimate;   // The object to animate
    private RectTransform rectTransform; // RectTransform of the object to animate
    public UIAnimationType animationType; // Type of animation to apply
    public Ease easeType;                 // Type of easing for animation
    public float duration;                // Duration of the animation
    public float delay;                   // Delay before the animation starts

    public bool loop;                     // Should the animation loop indefinitely
    public bool pingpong;                 // Should the animation alternate back and forth

    public bool startPositionOffset;     // Should we start from a custom position
    public Vector3 from;                 // Starting position/scale/rotation/etc.
    public Vector3 to;                   // Target position/scale/rotation/etc.

    private Tweener tweener;              // Reference to the tweener for controlling animation

    public bool showOnEnable;            // Should animation play when the object is enabled?
    public bool workOnDisable;           // Should animation play when the object is disabled?

    private void Awake()
    {
        Init();  // Initialize the object and its components

        // If no offset is set, apply animation to move the object
        if (!startPositionOffset && animationType == UIAnimationType.MoveTransform)
        {
            SwapDirection();   // Swap the from and to values
            MoveAbsolute();    // Perform absolute movement
            SwapDirection();   // Swap the from and to values back
        }
    }

    // Initialize the necessary components (e.g., RectTransform)
    private void Init()
    {
        if (objectToAnimate == null)
        {
            objectToAnimate = gameObject;  // Use the current gameObject if none is specified
        }

        if (rectTransform == null)
        {
            rectTransform = objectToAnimate.GetComponent<RectTransform>();  // Get RectTransform component
        }
    }

    private void OnEnable()
    {
        if (showOnEnable)
        {
            Show();  // Play the animation when the object is enabled
        }
    }

    public void OnDisable()
    {
        if (workOnDisable)
        {
            Disable();  // Play the animation when the object is disabled
        }
    }

    // Clean up the tweener when the object is destroyed
    private void OnDestroy()
    {
        if (tweener != null)
        {
            tweener.Kill();  // Kill the current tweener if it exists
            tweener = null;
        }
    }

    // Show method to start the animation
    public TweenerUI Show()
    {
        Init();  // Ensure components are initialized

        HandleTween();  // Start the appropriate animation
        return this;
    }

    // Method to handle the tween based on animation type
    private void HandleTween()
    {
        // Switch between different animation types
        switch (animationType)
        {
            case UIAnimationType.MoveTransform:
                MoveAbsolute();  // Handle movement animation
                break;
            case UIAnimationType.Scale:
                Scale();         // Handle scaling animation
                break;
            case UIAnimationType.ScaleX:
                Scale();         // Handle scaling only on X-axis
                break;
            case UIAnimationType.ScaleY:
                Scale();         // Handle scaling only on Y-axis
                break;
            case UIAnimationType.Fade:
                Fade();          // Handle fade animation
                break;
            case UIAnimationType.Size:
                Size();          // Handle size change animation
                break;
            case UIAnimationType.Rotation:
                Rotation();      // Handle rotation animation
                break;
            default:
                break;
        }

        // Set up looping if necessary
        if (loop)
        {
            tweener.SetLoops(-1);  // Infinite loop
        }

        // Set up pingpong (alternating) if necessary
        if (pingpong)
        {
            tweener.SetLoops(-1, LoopType.Yoyo);  // Alternating back and forth
        }
    }

    // Rotation animation
    private void Rotation()
    {
        if (startPositionOffset)
        {
            rectTransform.rotation = Quaternion.Euler(from);  // Set rotation from 'from' if offset is set
        }

        tweener = rectTransform.DORotate(to, duration)  // Perform rotation animation
            .SetEase(easeType)  // Apply easing
            .SetDelay(delay);    // Set delay before starting animation
    }

    // Size animation (change in RectTransform size)
    private void Size()
    {
        if (startPositionOffset)
        {
            rectTransform.sizeDelta = from;  // Set starting size if offset is set
        }

        tweener = rectTransform.DOSizeDelta(to, duration)  // Perform size change animation
            .SetEase(easeType)  // Apply easing
            .SetDelay(delay);    // Set delay before starting animation
    }

    // Fade animation (change in alpha)
    private void Fade()
    {
        if (gameObject.GetComponent<CanvasGroup>() == null)
        {
            gameObject.AddComponent<CanvasGroup>();  // Add CanvasGroup component if it doesn't exist
        }

        if (startPositionOffset)
        {
            objectToAnimate.GetComponent<CanvasGroup>().alpha = from.x;  // Set starting alpha if offset is set
        }

        tweener = objectToAnimate.GetComponent<CanvasGroup>().DOFade(to.x, duration)  // Perform fade animation
            .SetEase(easeType)  // Apply easing
            .SetDelay(delay);    // Set delay before starting animation
    }

    // Scale animation (change in RectTransform scale)
    private void Scale()
    {
        if (startPositionOffset)
        {
            rectTransform.localScale = from;  // Set starting scale if offset is set
        }

        tweener = rectTransform.DOScale(to, duration)  // Perform scale animation
            .SetEase(easeType)  // Apply easing
            .SetDelay(delay);    // Set delay before starting animation
    }

    // Move animation (change in position)
    private void MoveAbsolute()
    {
        if (startPositionOffset)
        {
            rectTransform.transform.position = from;  // Set starting position if offset is set
        }

        tweener = rectTransform.DOAnchorPos(to, duration)  // Perform position change animation
            .SetEase(easeType)  // Apply easing
            .SetDelay(delay);    // Set delay before starting animation
    }

    // Method to set delay for animation
    public TweenerUI SetDelay(int newDelay)
    {
        delay = newDelay;  // Set new delay

        return this;
    }

    // Swap the 'from' and 'to' values for reverse animation
    public TweenerUI SwapDirection()
    {
        var temp = from;
        from = to;
        to = temp;

        return this;
    }

    // Force a reset of the tweener and stop the animation
    public TweenerUI ForceReset()
    {
        tweener.Kill();  // Kill the current animation tweener

        return this;
    }

    // Disable the object after animation completes
    public TweenerUI Disable()
    {
        SwapDirection();  // Swap from and to values for reverse animation

        HandleTween();  // Handle the animation

        tweener.OnComplete(() =>
        {
            SwapDirection();  // Reverse the direction back
            gameObject.SetActive(false);  // Deactivate the object after animation
        });

        return this;
    }

    // Disable the specific game object after animation completes
    public TweenerUI Disable(GameObject gameObject)
    {
        SwapDirection();  // Swap from and to values for reverse animation

        HandleTween();  // Handle the animation

        tweener.OnComplete(() =>
        {
            SwapDirection();  // Reverse the direction back
            gameObject.SetActive(false);  // Deactivate the specified game object
        });

        return this;
    }

    // Disable and execute a custom action after animation completes
    public TweenerUI Disable(Action onCompleteAction)
    {
        SwapDirection();  // Swap from and to values for reverse animation

        HandleTween();  // Handle the animation

        tweener.OnComplete(() =>
        {
            onCompleteAction?.Invoke();  // Invoke the custom action when animation completes
        });

        return this;
    }

    // Set an action to be invoked when the animation completes
    public TweenerUI OnComplete(Action onCompleteAction)
    {
        tweener.OnComplete(() =>
        {
            onCompleteAction?.Invoke();  // Invoke the provided action when animation completes
        });

        return this;
    }
}
