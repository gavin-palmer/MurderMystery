using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    public string targetRoomName;
    
    private GameManager gameManager;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if it's the player
        if (other.CompareTag("Player"))
        {
            // Option 1: Auto-transition on touch
            // gameManager.MoveToRoom(targetRoomName);
            
            // Option 2: Show interaction prompt
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
    
    public void Interact()
    {
        // Move to the target room
        gameManager.MoveToRoom(targetRoomName);
    }
    
    private void ShowInteractionPrompt()
    {
        // In a complete implementation, show a UI prompt
        Debug.Log($"Press E to enter {targetRoomName}");
    }
    
    private void HideInteractionPrompt()
    {
        // In a complete implementation, hide the UI prompt
        Debug.Log("Interaction prompt hidden");
    }
}