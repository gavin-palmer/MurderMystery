using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MurderMystery.Models;

namespace MurderMystery.Managers
{
    public class DialogueManagerUnity : MonoBehaviour
    {
        [Header("UI References")]
        public GameObject dialoguePanel;
        public Text speakerNameText;
        public Text dialogueText;
        public RectTransform optionsPanel;
        public GameObject optionButtonPrefab; 

        // Current dialogue state
        private Person currentSpeaker;
        private bool isInDialogue = false;
        private List<DialogueOption> currentOptions = new List<DialogueOption>();

        // References
        private GameManager gameManager;

        void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();

            // Initialize UI
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
        }

        public void StartDialogue(Person person)
        {
            if (person == null || dialoguePanel == null) return;

            currentSpeaker = person;
            isInDialogue = true;

            // Show dialogue UI
            dialoguePanel.SetActive(true);

            // Initialize dialogue manager if needed
            if (person.Dialogue == null)
            {
                person.Dialogue = new MurderMystery.Dialogue.DialogueManager(person.PersonalityType);
            }

            // Show initial NPC statement
            string initialStatement = person.GenerateStatement(null);
            UpdateDialogueText(person.Name, initialStatement);

            // Show dialogue options
            ShowDialogueOptions(person);

            // Disable player movement during dialogue
            DisablePlayerMovement();
        }

        public void EndDialogue()
        {
            if (!isInDialogue || dialoguePanel == null) return;

            isInDialogue = false;
            currentSpeaker = null;

            // Hide dialogue UI
            dialoguePanel.SetActive(false);

            // Enable player movement
            EnablePlayerMovement();
        }

        private void UpdateDialogueText(string speakerName, string text)
        {
            if (speakerNameText != null)
            {
                speakerNameText.text = speakerName;
            }

            if (dialogueText != null)
            {
                dialogueText.text = text;
            }
        }

        private void ShowDialogueOptions(Person person)
        {
            // Clear existing options
            ClearDialogueOptions();

            // Get dialogue options
            currentOptions = person.Dialogue.GetPlayerTextOptions();

            // Add exit option if not present
            bool hasExitOption = false;
            foreach (var option in currentOptions)
            {
                if (option.NextNodeID == "exit" ||
                    (option.Variations != null && option.Variations.Count > 0 &&
                     option.Variations[0].NextNodeID == "exit"))
                {
                    hasExitOption = true;
                    break;
                }
            }

            if (!hasExitOption && currentOptions != null)
            {
                var exitOption = new DialogueOption
                {
                    Text = $"Thank you for your time, {person.Name}. I'll let you get back to your evening.",
                    NextNodeID = "exit"
                };
                currentOptions.Add(exitOption);
            }

            // Create option buttons
            foreach (var option in currentOptions)
            {
                CreateOptionButton(option);
            }
        }

        private void CreateOptionButton(DialogueOption option)
        {
            if (optionsPanel == null || optionButtonPrefab == null || string.IsNullOrEmpty(option.Text))
                return;

            // Instantiate button
            GameObject buttonObj = Instantiate(optionButtonPrefab, optionsPanel);
            Button button = buttonObj.GetComponent<Button>();
            Text buttonText = buttonObj.GetComponentInChildren<Text>();

            // Set button text
            if (buttonText != null)
            {
                buttonText.text = option.Text;
            }

            // Set button click handler
            if (button != null)
            {
                button.onClick.AddListener(() => OnDialogueOptionClicked(option));
            }
        }

        private void ClearDialogueOptions()
        {
            if (optionsPanel == null) return;

            // Destroy all child objects
            foreach (Transform child in optionsPanel)
            {
                Destroy(child.gameObject);
            }
        }

        private void OnDialogueOptionClicked(DialogueOption option)
        {
            if (currentSpeaker == null) return;

            // Show player's choice
            UpdateDialogueText("You", option.Text);

            // Check if this is an exit option
            if (option.NextNodeID == "exit" ||
                (option.Variations != null && option.Variations.Count > 0 &&
                 option.Variations[0].NextNodeID == "exit"))
            {
                // Wait a moment then end dialogue
                StartCoroutine(DelayedEndDialogue(1.5f));
                return;
            }

            // Give NPC time to "respond"
            StartCoroutine(ShowNPCResponse(option));
        }

        private IEnumerator ShowNPCResponse(DialogueOption playerChoice)
        {
            // Wait a moment for readability
            yield return new WaitForSeconds(1.0f);

            // Get NPC response
            string response = currentSpeaker.GenerateStatement(playerChoice);

            // Show NPC response
            UpdateDialogueText(currentSpeaker.Name, response);

            // Wait a moment before showing options
            yield return new WaitForSeconds(0.5f);

            // Show new dialogue options
            ShowDialogueOptions(currentSpeaker);
        }

        private IEnumerator DelayedEndDialogue(float delay)
        {
            yield return new WaitForSeconds(delay);
            EndDialogue();
            gameManager.EndInteraction();
        }

        private void DisablePlayerMovement()
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.enabled = false;
            }
        }

        private void EnablePlayerMovement()
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.enabled = true;
            }
        }
    }
}