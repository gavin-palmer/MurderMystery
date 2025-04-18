using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is a simplified controller for initial testing
public class MinimalGameController : MonoBehaviour
{
    [Header("References")]
    public SimpleRoomGenerator roomGenerator;
    public SimplePlayerController playerController;
    public GameObject npcPrefab;

    [Header("Debug Settings")]
    public bool generateTestNPCs = true;
    public int npcCount = 3;

    private List<GameObject> spawnedNPCs = new List<GameObject>();

    void Start()
    {
        // Generate rooms
        if (roomGenerator != null)
        {
            roomGenerator.CreateTestRooms();
            Debug.Log("Test rooms generated");
        }

        // Position player in the first room
        if (playerController != null)
        {
            Vector3 foyerPosition = roomGenerator.GetRoomCenter("Foyer");
            playerController.transform.position = foyerPosition;
            Debug.Log($"Player positioned at {foyerPosition}");
        }

        // Generate test NPCs if enabled
        if (generateTestNPCs && npcPrefab != null)
        {
            GenerateTestNPCs();
        }
    }

    private void GenerateTestNPCs()
    {
        string[] roomNames = { "Foyer", "Library", "Dining Room", "Study", "Kitchen" };
        string[] npcNames = { "Mrs. White", "Colonel Mustard", "Professor Plum", "Miss Scarlet", "Mrs. Peacock" };

        // Clean up any existing NPCs
        foreach (var npc in spawnedNPCs)
        {
            if (npc != null)
            {
                Destroy(npc);
            }
        }
        spawnedNPCs.Clear();

        // Create new NPCs
        for (int i = 0; i < npcCount && i < roomNames.Length && i < npcNames.Length; i++)
        {
            // Get room position
            Vector3 roomCenter = roomGenerator.GetRoomCenter(roomNames[i]);

            // Add random offset
            Vector3 offset = new Vector3(
                Random.Range(-2f, 2f),
                Random.Range(-2f, 2f),
                0
            );

            // Instantiate NPC
            GameObject npcObj = Instantiate(npcPrefab, roomCenter + offset, Quaternion.identity);
            npcObj.name = npcNames[i];

            // If NPC has a controller component, initialize it
            NPCController controller = npcObj.GetComponent<NPCController>();
            if (controller != null)
            {
                controller.npcName = npcNames[i];

                // Assign random personality
                var personalityTypes = System.Enum.GetValues(typeof(MurderMystery.Enums.PersonalityType));
                controller.personalityType = (MurderMystery.Enums.PersonalityType)personalityTypes.GetValue(Random.Range(0, personalityTypes.Length));

                // Set random color to distinguish NPCs
                SpriteRenderer renderer = npcObj.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.color = new Color(
                        Random.Range(0.5f, 1f),
                        Random.Range(0.5f, 1f),
                        Random.Range(0.5f, 1f)
                    );
                }
            }

            spawnedNPCs.Add(npcObj);
            Debug.Log($"NPC {npcNames[i]} spawned in {roomNames[i]}");
        }
    }
}