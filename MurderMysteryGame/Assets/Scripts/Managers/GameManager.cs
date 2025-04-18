using UnityEngine;
using MurderMystery;
using MurderMystery.Models;
using MurderMystery.Generators;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [Header("Visualisation References")]
    public RoomGenerator roomGenerator;
    public CharacterVisualiser characterVisualiser;
    public GameObject playerPrefab;

    [Header("Game Settings")]
    public bool autoGenerateOnStart = true;
    public bool debugLog = true;

    private Mystery currentMystery;
    private GameState gameState;

    void Start()
    {
        if (autoGenerateOnStart)
        {
            GenerateNewMystery();
        }
    }

    public void GenerateNewMystery()
    {
        // Generate a new mystery
        currentMystery = MysteryGenerator.CreateMystery();

        // Setup mansion layout
        var mansionGenerator = new MansionGenerator();
        var mansion = mansionGenerator.GenerateMansionLayout(currentMystery);

        // Place NPCs
        mansionGenerator.PlacePeople(currentMystery.People, currentMystery.Timeline);

        // Create game state
        gameState = new GameState(currentMystery, mansion);

        // Generate rooms in Unity
        roomGenerator.GenerateRoomsFromMystery(currentMystery);

        // Generate character visuals
        characterVisualiser.VisualiseCharacters(currentMystery);

        // Place player in starting room (usually "Foyer")
        PlacePlayerInStartingRoom();

        if (debugLog)
        {
            DebugPrintMysteryDetails();
        }
    }

    private void PlacePlayerInStartingRoom()
    {
        string startingRoom = "Foyer";
        if (!gameState.Mansion.ContainsKey(startingRoom))
        {
            startingRoom = gameState.Mansion.Keys.First();
        }

        Vector3 roomCenter = roomGenerator.GetRoomCenter(startingRoom);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null && playerPrefab != null)
        {
            player = Instantiate(playerPrefab, roomCenter, Quaternion.identity);
            player.tag = "Player";
        }
        else if (player != null)
        {
            player.transform.position = roomCenter;
        }
    }

    public void MoveToRoom(string roomName)
    {
        if (gameState != null)
        {
            gameState.MoveToRoom(roomName);

            // Move player 
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = roomGenerator.GetRoomCenter(roomName);
            }
        }
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
            Debug.Log($"Number of rooms: {currentMystery.Rooms.Count}");
            Debug.Log($"Number of characters: {currentMystery.People.Count}");
        }
    }
}