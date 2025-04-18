using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SimpleRoomGenerator : MonoBehaviour
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

    // Room data
    private Dictionary<string, RoomData> rooms = new Dictionary<string, RoomData>();

    void Start()
    {
        // Create basic test rooms
        CreateTestRooms();
    }

    public void CreateTestRooms()
    {
        // Define some test rooms
        string[] roomNames = { "Foyer", "Library", "Dining Room", "Study", "Kitchen" };

        // Create rooms in a line for simplicity
        for (int i = 0; i < roomNames.Length; i++)
        {
            CreateRoom(roomNames[i], i * (roomWidth + roomSpacing), 0);

            // Connect adjacent rooms with doors
            if (i > 0)
            {
                ConnectRooms(roomNames[i - 1], roomNames[i]);
            }
        }
    }

    public void CreateRoom(string roomName, int posX, int posY)
    {
        // Create floor tiles
        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                floorTilemap.SetTile(new Vector3Int(posX + x, posY + y, 0), floorTile);
            }
        }

        // Create wall tiles (outer perimeter)
        for (int x = -1; x <= roomWidth; x++)
        {
            wallTilemap.SetTile(new Vector3Int(posX + x, posY - 1, 0), wallTile);
            wallTilemap.SetTile(new Vector3Int(posX + x, posY + roomHeight, 0), wallTile);
        }

        for (int y = 0; y < roomHeight; y++)
        {
            wallTilemap.SetTile(new Vector3Int(posX - 1, posY + y, 0), wallTile);
            wallTilemap.SetTile(new Vector3Int(posX + roomWidth, posY + y, 0), wallTile);
        }

        // Add room label
        GameObject label = new GameObject($"Label_{roomName}");
        label.transform.position = new Vector3(posX + roomWidth / 2, posY + roomHeight + 1, 0);
        TextMesh textMesh = label.AddComponent<TextMesh>();
        textMesh.text = roomName;
        textMesh.alignment = TextAlignment.Center;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.fontSize = 40;
        textMesh.characterSize = 0.1f;

        // Store room data
        RoomData roomData = new RoomData
        {
            name = roomName,
            x = posX,
            y = posY,
            width = roomWidth,
            height = roomHeight,
            entrances = new List<EntranceData>()
        };

        rooms[roomName] = roomData;
    }

    public void ConnectRooms(string room1Name, string room2Name)
    {
        if (!rooms.ContainsKey(room1Name) || !rooms.ContainsKey(room2Name))
        {
            Debug.LogError($"Cannot connect rooms: {room1Name} or {room2Name} doesn't exist");
            return;
        }

        RoomData room1 = rooms[room1Name];
        RoomData room2 = rooms[room2Name];

        // Determine which direction the connection should be
        // For simplicity, we'll only handle left-to-right connections here

        // Room1 is to the left of Room2
        if (room1.x + room1.width < room2.x)
        {
            // Place door on the right wall of room1
            int doorX = room1.x + room1.width;
            int doorY = room1.y + room1.height / 2;

            // Remove wall tile and place door tile
            wallTilemap.SetTile(new Vector3Int(doorX, doorY, 0), doorTile);

            // Add entrance data to both rooms
            room1.entrances.Add(new EntranceData { direction = "east", connectedRoom = room2Name, x = doorX, y = doorY });
            room2.entrances.Add(new EntranceData { direction = "west", connectedRoom = room1Name, x = doorX, y = doorY });

            // Place door on the left wall of room2
            doorX = room2.x - 1;
            wallTilemap.SetTile(new Vector3Int(doorX, doorY, 0), doorTile);

            // Add door interaction component
            CreateDoorInteraction(doorX, doorY, room2Name);
            CreateDoorInteraction(room2.x - 1, doorY, room1Name);
        }
        // Room1 is to the right of Room2
        else if (room2.x + room2.width < room1.x)
        {
            // Place door on the right wall of room2
            int doorX = room2.x + room2.width;
            int doorY = room2.y + room2.height / 2;

            // Remove wall tile and place door tile
            wallTilemap.SetTile(new Vector3Int(doorX, doorY, 0), doorTile);

            // Add entrance data to both rooms
            room2.entrances.Add(new EntranceData { direction = "east", connectedRoom = room1Name, x = doorX, y = doorY });
            room1.entrances.Add(new EntranceData { direction = "west", connectedRoom = room2Name, x = doorX, y = doorY });

            // Place door on the left wall of room1
            doorX = room1.x - 1;
            wallTilemap.SetTile(new Vector3Int(doorX, doorY, 0), doorTile);

            // Add door interaction component
            CreateDoorInteraction(doorX, doorY, room1Name);
            CreateDoorInteraction(room1.x - 1, doorY, room2Name);
        }

        // Note: You would also want to handle vertical connections (north-south)
        // using a similar approach, but with the y-coordinates
    }

    private void CreateDoorInteraction(int x, int y, string targetRoom)
    {
        GameObject doorObj = new GameObject($"Door_to_{targetRoom}");
        doorObj.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0);

        // Add a box collider
        BoxCollider2D collider = doorObj.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(1f, 1f);

        // Add door interaction component
        DoorInteraction doorInteraction = doorObj.AddComponent<DoorInteraction>();
        doorInteraction.targetRoomName = targetRoom;
    }

    public Vector3 GetRoomCenter(string roomName)
    {
        if (rooms.ContainsKey(roomName))
        {
            RoomData room = rooms[roomName];
            return new Vector3(
                room.x + room.width / 2,
                room.y + room.height / 2,
                0
            );
        }

        Debug.LogError($"Room {roomName} not found");
        return Vector3.zero;
    }
}

// Data classes to store room information
public class RoomData
{
    public string name;
    public int x;
    public int y;
    public int width;
    public int height;
    public List<EntranceData> entrances;
}

public class EntranceData
{
    public string direction;
    public string connectedRoom;
    public int x;
    public int y;
}