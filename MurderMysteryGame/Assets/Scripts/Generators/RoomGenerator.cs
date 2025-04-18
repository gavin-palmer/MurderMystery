using UnityEngine;
using UnityEngine.Tilemaps;
using MurderMystery.Models;
using System.Collections.Generic;

public class RoomGenerator : MonoBehaviour
{
    [Header("Room Settings")]
    public int roomWidth = 20;
    public int roomHeight = 15;
    public int roomSpacing = 5;

    [Header("Tilemap References")]
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;
    public Tilemap doorTilemap;

    [Header("Tile References")]
    public TileBase floorTile;
    public TileBase wallTile;
    public TileBase doorTile;

    [Header("Room Colors")]
    public Color libraryColor = new Color(0.8f, 0.7f, 0.5f);
    public Color kitchenColor = new Color(0.7f, 0.9f, 0.9f);
    public Color loungeColor = new Color(0.8f, 0.6f, 0.6f);
    public Color defaultRoomColor = Color.white;

    private Dictionary<string, RoomData> generatedRooms = new Dictionary<string, RoomData>();

    public void GenerateRoomsFromMystery(Mystery mystery)
    {
        ClearTilemaps();
        generatedRooms.Clear();

        int roomCount = mystery.Rooms.Count;
        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(roomCount));

        // Position rooms in a grid
        for (int i = 0; i < roomCount; i++)
        {
            Room room = mystery.Rooms[i];
            int row = i / gridSize;
            int col = i % gridSize;

            Vector2Int position = new Vector2Int(
                col * (roomWidth + roomSpacing),
                row * (roomHeight + roomSpacing)
            );

            CreateRoomVisual(room.Name, position);
        }

        // Connect rooms based on their logical connections
        foreach (var room in mystery.Rooms)
        {
            foreach (var connection in room.Connections)
            {
                ConnectRooms(room.Name, connection.Value);
            }
        }
    }

    private void CreateRoomVisual(string roomName, Vector2Int position)
    {
        // Create floor tiles
        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                floorTilemap.SetTile(new Vector3Int(position.x + x, position.y + y, 0), floorTile);
            }
        }

        // Create walls
        for (int x = -1; x <= roomWidth; x++)
        {
            wallTilemap.SetTile(new Vector3Int(position.x + x, position.y - 1, 0), wallTile);
            wallTilemap.SetTile(new Vector3Int(position.x + x, position.y + roomHeight, 0), wallTile);
        }

        for (int y = 0; y < roomHeight; y++)
        {
            wallTilemap.SetTile(new Vector3Int(position.x - 1, position.y + y, 0), wallTile);
            wallTilemap.SetTile(new Vector3Int(position.x + roomWidth, position.y + y, 0), wallTile);
        }

        // Set room color based on room type
        Color roomColor = GetRoomColor(roomName);
        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                floorTilemap.SetColor(new Vector3Int(position.x + x, position.y + y, 0), roomColor);
            }
        }

        // Add room label
        GameObject labelObj = new GameObject($"Label_{roomName}");
        labelObj.transform.position = new Vector3(position.x + roomWidth / 2, position.y + roomHeight + 1, 0);
        TextMesh textMesh = labelObj.AddComponent<TextMesh>();
        textMesh.text = roomName;
        textMesh.alignment = TextAlignment.Center;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.fontSize = 40;
        textMesh.characterSize = 0.1f;

        // Store room data
        RoomData roomData = new RoomData
        {
            Name = roomName,
            X = position.x,
            Y = position.y,
            Width = roomWidth,
            Height = roomHeight,
            Entrances = new List<EntranceData>()
        };

        generatedRooms[roomName] = roomData;
    }

    private void ConnectRooms(string room1Name, string room2Name)
    {
        if (!generatedRooms.ContainsKey(room1Name) || !generatedRooms.ContainsKey(room2Name))
            return;

        RoomData room1 = generatedRooms[room1Name];
        RoomData room2 = generatedRooms[room2Name];

        // Determine positions for the door
        int doorX, doorY;
        string direction;

        // Room 1 is to the left of Room 2
        if (room1.X + room1.Width < room2.X)
        {
            doorX = room1.X + room1.Width;
            doorY = room1.Y + room1.Height / 2;
            direction = "east";

            // Remove wall and place door
            wallTilemap.SetTile(new Vector3Int(doorX, doorY, 0), doorTile);

            // Add door on Room 2's side
            wallTilemap.SetTile(new Vector3Int(room2.X - 1, doorY, 0), doorTile);

            CreateDoorInteraction(doorX, doorY, room2Name);
            CreateDoorInteraction(room2.X - 1, doorY, room1Name);
        }
        // Room 1 is to the right of Room 2
        else if (room2.X + room2.Width < room1.X)
        {
            doorX = room2.X + room2.Width;
            doorY = room2.Y + room2.Height / 2;
            direction = "west";

            // Remove wall and place door
            wallTilemap.SetTile(new Vector3Int(doorX, doorY, 0), doorTile);

            // Add door on Room 1's side
            wallTilemap.SetTile(new Vector3Int(room1.X - 1, doorY, 0), doorTile);

            CreateDoorInteraction(doorX, doorY, room1Name);
            CreateDoorInteraction(room1.X - 1, doorY, room2Name);
        }
        // Room 1 is below Room 2
        else if (room1.X + room1.Width < room2.Y)
        {
            doorX = room1.X + room1.Height / 2;
            doorY = room1.Y + room1.Height;
            direction = "north";

            // Remove wall and place door
            wallTilemap.SetTile(new Vector3Int(doorX, doorY, 0), doorTile);

            // Add door on Room 2's side
            wallTilemap.SetTile(new Vector3Int(doorX, room2.Y - 1, 0), doorTile);

            CreateDoorInteraction(doorX, doorY, room2Name);
            CreateDoorInteraction(doorX, room2.Y - 1, room1Name);
        }
        // Room 1 is above Room 2
        else if (room2.Y + room2.Height < room1.Y)
        {
            doorX = room2.X + room2.Width / 2;
            doorY = room2.Y + room2.Height;
            direction = "south";

            // Remove wall and place door
            wallTilemap.SetTile(new Vector3Int(doorX, doorY, 0), doorTile);

            // Add door on Room 1's side
            wallTilemap.SetTile(new Vector3Int(doorX, room1.Y - 1, 0), doorTile);

            CreateDoorInteraction(doorX, doorY, room1Name);
            CreateDoorInteraction(doorX, room1.Y - 1, room2Name);
        }
    }

    private void CreateDoorInteraction(int x, int y, string targetRoom)
    {
        GameObject doorObj = new GameObject($"Door_to_{targetRoom}");
        doorObj.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0);

        // Add door collider
        BoxCollider2D collider = doorObj.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(1f, 1f);

        // Add door interaction component
        DoorInteraction doorInteraction = doorObj.AddComponent<DoorInteraction>();
        doorInteraction.targetRoomName = targetRoom;
    }

    private Color GetRoomColor(string roomName)
    {
        roomName = roomName.ToLower();

        if (roomName.Contains("library")) return libraryColor;
        if (roomName.Contains("study")) return new Color(0.7f, 0.5f, 0.4f);
        if (roomName.Contains("kitchen")) return kitchenColor;
        if (roomName.Contains("dining")) return new Color(0.9f, 0.7f, 0.5f);
        if (roomName.Contains("lounge") || roomName.Contains("living")) return loungeColor;
        if (roomName.Contains("bedroom")) return new Color(0.6f, 0.7f, 0.9f);
        if (roomName.Contains("bathroom")) return new Color(0.8f, 0.9f, 1.0f);
        if (roomName.Contains("hall") || roomName.Contains("foyer")) return new Color(0.85f, 0.85f, 0.7f);
        if (roomName.Contains("garden")) return new Color(0.6f, 0.8f, 0.6f);

        return defaultRoomColor;
    }

    private void ClearTilemaps()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        doorTilemap.ClearAllTiles();

        // Clear labels
        GameObject[] labels = GameObject.FindGameObjectsWithTag("RoomLabel");
        foreach (var label in labels)
        {
            Destroy(label);
        }
    }

    public Vector3 GetRoomCenter(string roomName)
    {
        if (generatedRooms.ContainsKey(roomName))
        {
            RoomData room = generatedRooms[roomName];
            return new Vector3(
                room.X + room.Width / 2,
                room.Y + room.Height / 2,
                0
            );
        }

        return Vector3.zero;
    }
}