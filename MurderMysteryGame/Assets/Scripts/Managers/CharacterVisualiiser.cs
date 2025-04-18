using UnityEngine;
using System.Collections.Generic;
using MurderMystery.Models;
using MurderMystery.Enums;

public class CharacterVisualiser : MonoBehaviour
{
    [Header("Character Prefabs")]
    public GameObject characterPrefab;

    [Header("Character Visuals")]
    public Sprite defaultCharacterSprite;
    public Sprite victimSprite;
    public Color[] personalityColors;

    private Dictionary<string, GameObject> characterObjects = new Dictionary<string, GameObject>();
    private RoomGenerator roomGenerator;

    private void Awake()
    {
        roomGenerator = FindObjectOfType<RoomGenerator>();
    }

    public void VisualiseCharacters(Mystery mystery)
    {
        ClearExistingCharacters();

        foreach (var person in mystery.People)
        {
            if (person != mystery.Victim)
            {
                CreateCharacterVisual(person);
            }
        }
    }

    private void CreateCharacterVisual(Person person)
    {
        // Get position in the character's current room
        Vector3 roomCenter = roomGenerator.GetRoomCenter(person.CurrentRoom);

        // Add some random offset
        Vector3 offset = new Vector3(
            Random.Range(-roomGenerator.roomWidth / 4f, roomGenerator.roomWidth / 4f),
            Random.Range(-roomGenerator.roomHeight / 4f, roomGenerator.roomHeight / 4f),
            0
        );

        // Create the character object
        GameObject characterObj = Instantiate(characterPrefab, roomCenter + offset, Quaternion.identity);
        characterObj.name = "Character_" + person.Name;

        // Set up visuals
        SpriteRenderer spriteRenderer = characterObj.GetComponent<SpriteRenderer>();
        if (spriteRenderer)
        {
            spriteRenderer.sprite = defaultCharacterSprite;

            // Set color based on personality
            int personalityIndex = (int)person.PersonalityType;
            if (personalityIndex >= 0 && personalityIndex < personalityColors.Length)
            {
                spriteRenderer.color = personalityColors[personalityIndex];
            }
        }

        // Add character controller component
        NPCController npcController = characterObj.GetComponent<NPCController>();
        if (npcController)
        {
            npcController.Initialize(person);
        }

        // Add nameplate
        GameObject nameplate = new GameObject("Nameplate");
        nameplate.transform.SetParent(characterObj.transform);
        nameplate.transform.localPosition = new Vector3(0, 0.8f, 0);

        TextMesh nameplateText = nameplate.AddComponent<TextMesh>();
        nameplateText.text = person.Name;
        nameplateText.fontSize = 40;
        nameplateText.characterSize = 0.05f;
        nameplateText.alignment = TextAlignment.Center;
        nameplateText.anchor = TextAnchor.MiddleCenter;
        nameplateText.color = Color.black;

        // Make text face camera
        nameplate.AddComponent<Billboard>();

        // Store reference to character
        characterObjects[person.Name] = characterObj;
    }

    public void MoveCharacterToRoom(string characterName, string roomName)
    {
        if (characterObjects.ContainsKey(characterName))
        {
            GameObject characterObj = characterObjects[characterName];

            Vector3 roomCenter = roomGenerator.GetRoomCenter(roomName);
            Vector3 offset = new Vector3(
                Random.Range(-roomGenerator.roomWidth / 4f, roomGenerator.roomWidth / 4f),
                Random.Range(-roomGenerator.roomHeight / 4f, roomGenerator.roomHeight / 4f),
                0
            );

            characterObj.transform.position = roomCenter + offset;
        }
    }

    private void ClearExistingCharacters()
    {
        foreach (var charObj in characterObjects.Values)
        {
            Destroy(charObj);
        }

        characterObjects.Clear();
    }
}