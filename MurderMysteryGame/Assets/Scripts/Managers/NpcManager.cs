using System.Collections.Generic;
using UnityEngine;
using MurderMystery.Models;

namespace MurderMystery.Managers
{
    public class NPCManager : MonoBehaviour
    {
        [Header("NPC Settings")]
        public GameObject npcPrefab;

        [Header("NPC Sprites")]
        public List<NPCVisualData> npcVisuals;

        // Dictionary to store NPC game objects
        private Dictionary<string, GameObject> npcGameObjects = new Dictionary<string, GameObject>();

        // Reference to room manager
        private RoomManager roomManager;

        void Awake()
        {
            roomManager = GetComponent<RoomManager>() ?? FindObjectOfType<RoomManager>();
        }

        public void SetupNPCs(List<Person> people)
        {
            ClearNPCs();

            // Generate NPCs for non-victim people
            foreach (var person in people)
            {
                if (!person.IsVictim)
                {
                    CreateNPC(person);
                }
            }
        }

        private void CreateNPC(Person person)
        {
            // Get room position
            Vector2 roomPos = roomManager.GetRoomPosition(person.CurrentRoom);

            // Add some random offset within the room
            Vector2 offset = new Vector2(
                Random.Range(-5f, 5f),
                Random.Range(-3f, 3f)
            );

            // Instantiate NPC prefab
            GameObject npcObj = Instantiate(npcPrefab, roomPos + offset, Quaternion.identity);
            npcObj.name = "NPC_" + person.Name;

            // Add NPC controller component
            NPCController controller = npcObj.GetComponent<NPCController>() ?? npcObj.AddComponent<NPCController>();
            controller.Initialize(person);

            // Assign visual appearance based on personality
            AssignNPCVisuals(controller, person);

            // Store reference to NPC game object
            npcGameObjects[person.Name] = npcObj;
        }

        private void AssignNPCVisuals(NPCController controller, Person person)
        {
            // Find appropriate visual based on personality
            NPCVisualData visualData = null;

            // Look for a visual that matches this personality
            foreach (var visual in npcVisuals)
            {
                if (visual.personalityType == person.PersonalityType)
                {
                    visualData = visual;
                    break;
                }
            }

            // If no matching personality, use random visual
            if (visualData == null && npcVisuals.Count > 0)
            {
                visualData = npcVisuals[Random.Range(0, npcVisuals.Count)];
            }

            // Apply visual data if found
            if (visualData != null)
            {
                controller.SetVisuals(visualData);
            }
        }

        private void ClearNPCs()
        {
            // Destroy all existing NPC game objects
            foreach (var npcObj in npcGameObjects.Values)
            {
                if (npcObj != null)
                {
                    Destroy(npcObj);
                }
            }

            npcGameObjects.Clear();
        }

        // Move an NPC to a specific room
        public void MoveNPCToRoom(string npcName, string roomName)
        {
            if (npcGameObjects.ContainsKey(npcName))
            {
                Vector2 roomPosition = roomManager.GetRoomPosition(roomName);

                // Add some random offset
                Vector2 offset = new Vector2(
                    Random.Range(-5f, 5f),
                    Random.Range(-3f, 3f)
                );

                npcGameObjects[npcName].transform.position = roomPosition + offset;
            }
        }
    }

    [System.Serializable]
    public class NPCVisualData
    {
        public PersonalityType personalityType;
        public Sprite baseSprite;
        public RuntimeAnimatorController animatorController;
        public Color tintColor = Color.white;
    }
}