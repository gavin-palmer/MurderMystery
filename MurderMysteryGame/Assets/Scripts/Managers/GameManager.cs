using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MurderMystery;
using MurderMystery.Models;
using MurderMystery.Generators;
using MurderMystery.Dialogue;

namespace MurderMystery.Managers
{
    public class GameManager : MonoBehaviour
    {
        [Header("Game Settings")]
        public bool debugMode = true;

        [Header("Game State")]
        private GameState gameState;
        private Mystery currentMystery;
        private Dictionary<string, Room> mansion;

        // References to other managers
        private RoomManager roomManager;
        private DialogueManagerUnity dialogueManager;
        private NPCManager npcManager;

        // Reference to the player
        private PlayerController player;

        void Awake()
        {
            // Get references
            roomManager = GetComponent<RoomManager>();
            dialogueManager = GetComponent<DialogueManagerUnity>();
            npcManager = GetComponent<NPCManager>();
            player = FindObjectOfType<PlayerController>();

            if (debugMode)
            {
                Debug.Log("Game Manager initialized in debug mode");
            }
        }

        void Start()
        {
            StartNewGame();
        }

        public void StartNewGame()
        {
            Debug.Log("Starting new game...");

            // Generate a new mystery
            currentMystery = MysteryGenerator.CreateMystery();

            // Setup mansion layout
            var mansionGenerator = new MansionGenerator();
            mansion = mansionGenerator.GenerateMansionLayout(currentMystery);

            // Place NPCs
            mansionGenerator.PlacePeople(currentMystery.People, currentMystery.Timeline);

            // Create game state
            gameState = new GameState(currentMystery, mansion);

            // Initialize the room layouts
            roomManager.GenerateRooms(mansion);

            // Setup NPCs
            npcManager.SetupNPCs(currentMystery.People);

            // Put player in starting room (usually "Foyer")
            player.TeleportToRoom("Foyer");

            if (debugMode)
            {
                DebugPrintMysteryDetails();
            }
        }

        // Used to move between rooms
        public void MoveToRoom(string roomName)
        {
            if (gameState != null)
            {
                gameState.MoveToRoom(roomName);
                player.TeleportToRoom(roomName);
                roomManager.ActivateRoom(roomName);
            }
        }

        // Start a conversation with an NPC
        public void InteractWithNPC(string npcName)
        {
            if (gameState != null)
            {
                Person person = currentMystery.People.Find(p => p.Name == npcName);
                if (person != null)
                {
                    gameState.InterviewPerson(person);
                    dialogueManager.StartDialogue(person);
                }
            }
        }

        // End the conversation with an NPC
        public void EndInteraction()
        {
            if (gameState != null)
            {
                gameState.TerminateInterview();
                dialogueManager.EndDialogue();
            }
        }

        // Gets visible clues in current room
        public List<Clue> GetVisibleCluesInCurrentRoom()
        {
            if (gameState != null)
            {
                return gameState.GetVisibleCluesInCurrentRoom();
            }
            return new List<Clue>();
        }

        // Gets NPCs in current room
        public List<Person> GetPeopleInCurrentRoom()
        {
            if (gameState != null)
            {
                return gameState.GetPeopleInCurrentRoom();
            }
            return new List<Person>();
        }

        private void DebugPrintMysteryDetails()
        {
            if (currentMystery != null)
            {
                Debug.Log($"Murder mystery generated!");
                Debug.Log($"Victim: {currentMystery.Victim.Name}");
                Debug.Log($"Murderer: {currentMystery.Murderer.Name}");
                Debug.Log($"Weapon: {currentMystery.Weapon}");
                Debug.Log($"Room: {currentMystery.Room}");
                Debug.Log($"Motive: {currentMystery.Motive}");
                Debug.Log($"Number of rooms: {mansion.Count}");
                Debug.Log($"Number of characters: {currentMystery.People.Count}");
            }
        }
    }
}