using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MurderMystery.Models;
using MurderMystery.Enums;
using MurderMystery.Managers;

public class NPCController : MonoBehaviour
{
    [Header("NPC Data")]
    public string npcName;
    public PersonalityType personalityType;
    public float moveSpeed = 1f;

    [Header("Movement Settings")]
    public bool randomMovement = true;
    public float movementPauseMin = 2f;
    public float movementPauseMax = 5f;
    public float movementDurationMin = 1f;
    public float movementDurationMax = 3f;
    public float movementRadius = 2f;

    // References
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Person personData;

    // Movement variables
    private Vector2 startPosition;
    private Vector2 targetPosition;
    private float movementTimer;
    private float pauseTimer;
    private bool isMoving = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        startPosition = transform.position;
        pauseTimer = Random.Range(movementPauseMin, movementPauseMax);
    }

    void Update()
    {
        if (randomMovement)
        {
            HandleRandomMovement();
        }
    }

    public void Initialize(Person person)
    {
        personData = person;
        npcName = person.Name;
        personalityType = person.PersonalityType;

        // Add a collider for interaction
        BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(1f, 1.5f);
        collider.offset = new Vector2(0, 0);

        // Add nameplate
        CreateNameplate();
    }

    public void SetVisuals(NPCVisualData visualData)
    {
        if (spriteRenderer != null && visualData.baseSprite != null)
        {
            spriteRenderer.sprite = visualData.baseSprite;
            spriteRenderer.color = visualData.tintColor;
        }

        if (animator != null && visualData.animatorController != null)
        {
            animator.runtimeAnimatorController = visualData.animatorController;
        }
    }

    private void CreateNameplate()
    {
        // Create nameplate game object
        GameObject nameplateObj = new GameObject("Nameplate");
        nameplateObj.transform.SetParent(transform);
        nameplateObj.transform.localPosition = new Vector3(0, 1.2f, 0);

        // Add TextMesh component
        TextMesh textMesh = nameplateObj.AddComponent<TextMesh>();
        textMesh.text = npcName;
        textMesh.fontSize = 40;
        textMesh.alignment = TextAlignment.Center;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.color = Color.black;
        textMesh.characterSize = 0.05f;

        // Make text face the camera
        textMesh.gameObject.AddComponent<Billboard>();
    }

    private void HandleRandomMovement()
    {
        if (isMoving)
        {
            // Move towards target position
            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            // Update animator if we have one
            if (animator != null)
            {
                Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
                animator.SetFloat("Horizontal", direction.x);
                animator.SetFloat("Vertical", direction.y);
                animator.SetFloat("Speed", direction.magnitude);

                // Flip sprite based on horizontal direction
                if (direction.x > 0.1f)
                {
                    spriteRenderer.flipX = false;
                }
                else if (direction.x < -0.1f)
                {
                    spriteRenderer.flipX = true;
                }
            }

            // Check if we've reached the target
            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMoving = false;
                pauseTimer = Random.Range(movementPauseMin, movementPauseMax);

                // Set animator to idle
                if (animator != null)
                {
                    animator.SetFloat("Speed", 0);
                }
            }
        }
        else
        {
            // Wait for pause timer
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0)
            {
                // Start a new movement
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                targetPosition = startPosition + randomDirection * Random.Range(0, movementRadius);
                isMoving = true;
                movementTimer = Random.Range(movementDurationMin, movementDurationMax);
            }
        }
    }

    public string GenerateStatement(DialogueOption playerDialogue)
    {
        // This is a simplified implementation
        if (personData != null && personData.Dialogue != null)
        {
            return personData.GenerateStatement(playerDialogue);
        }

        // Fallback responses based on personality if the dialogue system isn't initialized
        switch (personalityType)
        {
            case PersonalityType.Nervous:
                return "Oh! H-hello there... I wasn't expecting to talk to anyone.";
            case PersonalityType.Suspicious:
                return "What do you want? Why are you questioning everyone?";
            case PersonalityType.Arrogant:
                return "I suppose I can spare a moment if you absolutely must speak with me.";
            case PersonalityType.Analytical:
                return "I presume you're investigating the incident. Logical approach.";
            case PersonalityType.Sensitive:
                return "It's just awful what happened, isn't it? Everyone seems so shaken up.";
            case PersonalityType.Manipulative:
                return "What a pleasure to meet someone new during such unfortunate circumstances.";
            case PersonalityType.Defensive:
                return "I've already told everything I know. I was nowhere near that part of the house.";
            case PersonalityType.Gossipy:
                return "Oh my goodness, can you believe what's happened? Everyone's talking about it!";
            case PersonalityType.Reserved:
                return "Good evening. I presume you wish to speak with me regarding the unfortunate events.";
            case PersonalityType.Flirtatous:
                return "Well hello there. What a delightful surprise to be interviewed by someone like you.";
            default:
                return "Hello there. What can I help you with?";
        }
    }
}