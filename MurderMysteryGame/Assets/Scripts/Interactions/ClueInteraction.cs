using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MurderMystery.Models;

public class ClueInteraction : MonoBehaviour
{
    [Header("Clue Data")]
    public Clue clueData;
    public string clueDescription;
    public bool hasBeenDiscovered = false;

    // References
    private GameManager gameManager;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // Set initial appearance
        UpdateVisuals();
    }

    public void Initialize(Clue clue)
    {
        clueData = clue;
        clueDescription = clue.Description;
        hasBeenDiscovered = clue.IsFound;

        // Add a collider for interaction
        BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(1f, 1f);
        collider.isTrigger = true;

        // Update visuals
        UpdateVisuals();
    }

    public void Interact()
    {
        if (!hasBeenDiscovered && clueData != null)
        {
            // Discover the clue
            hasBeenDiscovered = true;
            clueData.IsFound = true;

            // Show clue discovery UI
            ShowClueDiscoveryUI();

            // Update visuals
            UpdateVisuals();
        }
        else
        {
            // Already discovered - just show info
            ShowClueInfo();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if it's the player
        if (other.CompareTag("Player"))
        {
            // Show interaction prompt
            ShowInteractionPrompt();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Check if it's the player
        if (other.CompareTag("Player"))
        {
            // Hide interaction prompt
            HideInteractionPrompt();
        }
    }

    private void UpdateVisuals()
    {
        if (spriteRenderer != null)
        {
            // Adjust appearance based on discovery state
            if (hasBeenDiscovered)
            {
                spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f); // Semi-transparent
            }
            else
            {
                spriteRenderer.color = Color.white;
            }
        }
    }

    private void ShowClueDiscoveryUI()
    {
        // In a complete implementation, show a UI panel with the clue info
        Debug.Log($"Clue Discovered: {clueDescription}");

        // You could trigger a UI animation or sound effect here
    }

    private void ShowClueInfo()
    {
        // In a complete implementation, show the clue info again
        Debug.Log($"Clue Info: {clueDescription}");
    }

    private void ShowInteractionPrompt()
    {
        // In a complete implementation, show a UI prompt
        Debug.Log("Press E to examine clue");
    }

    private void HideInteractionPrompt()
    {
        // In a complete implementation, hide the UI prompt
        Debug.Log("Interaction prompt hidden");
    }
}