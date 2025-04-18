using System.Collections;
using System.Collections.Generic;
using MurderMystery.Managers;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Animation Settings")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    [Header("Interaction Settings")]
    public float interactionDistance = 1.5f;
    public LayerMask interactionLayer;

    // References
    private Rigidbody2D rb;
    private GameManager gameManager;
    private RoomManager roomManager;

    // Input management
    private Vector2 movement;

    void Start()
    {
        // Get component references
        rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        // Get manager references
        gameManager = FindObjectOfType<GameManager>();
        roomManager = FindObjectOfType<RoomManager>();
    }

    void Update()
    {
        // Handle player input
        HandleMovementInput();
        HandleInteractionInput();
    }

    void FixedUpdate()
    {
        // Apply movement
        MovePlayer();
    }

    private void HandleMovementInput()
    {
        // Get movement input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Normalize to prevent diagonal speed boost
        if (movement.magnitude > 1)
        {
            movement.Normalize();
        }

        // Update animation parameters
        if (animator != null)
        {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetFloat("Speed", movement.sqrMagnitude);
        }

        // Update facing direction (flip sprite)
        if (movement.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (movement.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
        {
            TryInteract();
        }
    }

    private void MovePlayer()
    {
        // Move the player using physics
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void TryInteract()
    {
        // Get facing direction
        Vector2 facingDirection = movement.normalized;
        if (facingDirection == Vector2.zero)
        {
            // Default to facing down if not moving
            facingDirection = Vector2.down;
        }

        // Cast a ray to check for interactable objects
        RaycastHit2D hit = Physics2D.Raycast(
            rb.position,
            facingDirection,
            interactionDistance,
            interactionLayer
        );

        // Debug the ray
        Debug.DrawRay(rb.position, facingDirection * interactionDistance, Color.red, 0.5f);

        if (hit.collider != null)
        {
            // Check if interacting with NPC
            NPCController npc = hit.collider.GetComponent<NPCController>();
            if (npc != null)
            {
                gameManager.InteractWithNPC(npc.npcName);
                return;
            }

            // Check if interacting with door
            DoorInteraction door = hit.collider.GetComponent<DoorInteraction>();
            if (door != null)
            {
                gameManager.MoveToRoom(door.targetRoomName);
                return;
            }

            // Check if interacting with clue
            ClueInteraction clue = hit.collider.GetComponent<ClueInteraction>();
            if (clue != null)
            {
                clue.Interact();
                return;
            }
        }
    }

    // Teleport player to a specific room
    public void TeleportToRoom(string roomName)
    {
        Vector2 roomPosition = roomManager.GetRoomPosition(roomName);
        transform.position = roomPosition;
    }
}