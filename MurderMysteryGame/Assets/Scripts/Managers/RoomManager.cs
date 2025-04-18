using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using MurderMystery.Models;
using MurderMystery.Enums;
namespace MurderMystery.Managers
{
    public class RoomManager : MonoBehaviour
    {
        [Header("Room Generation Settings")]
        public int roomWidth = 20;
        public int roomHeight = 15;
        public float tileSize = 1f;

        [Header("Tile References")]
        public Tile floorTile;
        public Tile wallTile;
        public Tile doorTile;

        [Header("Tile Variants")]
        public List<RoomTileSet> roomTileSets;

        [Header("References")]
        public Tilemap floorTilemap;
        public Tilemap wallTilemap;
        public Tilemap propsTilemap;

        // Dictionary to store room game objects
        private Dictionary<string, GameObject> roomGameObjects = new Dictionary<string, GameObject>();
        private Dictionary<string, Vector2> roomPositions = new Dictionary<string, Vector2>();

        // Current active room
        private string currentRoomName;

        // Generate room layouts from the mansion data
        public void GenerateRooms(Dictionary<string, Room> mansion)
        {
            ClearTilemaps();
            roomGameObjects.Clear();
            roomPositions.Clear();

            // We'll position rooms on a grid
            int gridX = 0;
            int gridY = 0;
            int maxRoomsPerRow = 5;

            foreach (var kvp in mansion)
            {
                string roomName = kvp.Key;
                Room room = kvp.Value;

                // Create position for this room
                Vector2 position = new Vector2(
                    gridX * (roomWidth + 5) * tileSize,
                    gridY * (roomHeight + 5) * tileSize
                );

                // Store room position for teleporting player
                roomPositions[roomName] = position;

                // Create room GameObject container
                GameObject roomObj = new GameObject(roomName);
                roomObj.transform.position = position;

                // Generate room layout based on type
                GenerateRoomLayout(roomName, room, position);

                // Add doors based on connections
                foreach (var connection in room.Connections)
                {
                    AddDoor(position, connection.Key, connection.Value);
                }

                // Store reference to room game object
                roomGameObjects[roomName] = roomObj;

                // Advance grid position
                gridX++;
                if (gridX >= maxRoomsPerRow)
                {
                    gridX = 0;
                    gridY++;
                }
            }

            // Start with all rooms active for now
            // In a full implementation, you'd only show the current room
        }

        private void GenerateRoomLayout(string roomName, Room room, Vector2 position)
        {
            // Select appropriate tileset based on room type
            RoomTileSet tileSet = GetTileSetForRoom(roomName);

            // Get room bounds
            int startX = Mathf.FloorToInt(position.x);
            int startY = Mathf.FloorToInt(position.y);
            int endX = startX + roomWidth;
            int endY = startY + roomHeight;

            // Create floor
            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    floorTilemap.SetTile(new Vector3Int(x, y, 0), tileSet.floorTile);
                }
            }

            // Create walls
            for (int x = startX - 1; x <= endX; x++)
            {
                wallTilemap.SetTile(new Vector3Int(x, startY - 1, 0), tileSet.wallTile);
                wallTilemap.SetTile(new Vector3Int(x, endY, 0), tileSet.wallTile);
            }

            for (int y = startY; y < endY; y++)
            {
                wallTilemap.SetTile(new Vector3Int(startX - 1, y, 0), tileSet.wallTile);
                wallTilemap.SetTile(new Vector3Int(endX, y, 0), tileSet.wallTile);
            }

            // Add room label
            GameObject labelObj = new GameObject("Label");
            labelObj.transform.position = new Vector3(startX + roomWidth / 2, endY + 1, 0);
            TextMesh textMesh = labelObj.AddComponent<TextMesh>();
            textMesh.text = roomName;
            textMesh.fontSize = 40;
            textMesh.alignment = TextAlignment.Center;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.color = Color.black;
            textMesh.characterSize = 0.1f;

            // Add props based on room type
            AddRoomProps(roomName, tileSet, new Vector2(startX, startY), new Vector2(endX, endY));
        }

        private void AddDoor(Vector2 roomPosition, Direction direction, string connectedRoom)
        {
            // Calculate door position based on direction
            Vector3Int doorPosition = new Vector3Int();
            int startX = Mathf.FloorToInt(roomPosition.x);
            int startY = Mathf.FloorToInt(roomPosition.y);
            int endX = startX + roomWidth;
            int endY = startY + roomHeight;

            switch (direction)
            {
                case Direction.North:
                    doorPosition = new Vector3Int(startX + roomWidth / 2, endY, 0);
                    break;
                case Direction.South:
                    doorPosition = new Vector3Int(startX + roomWidth / 2, startY - 1, 0);
                    break;
                case Direction.East:
                    doorPosition = new Vector3Int(endX, startY + roomHeight / 2, 0);
                    break;
                case Direction.West:
                    doorPosition = new Vector3Int(startX - 1, startY + roomHeight / 2, 0);
                    break;
            }

            // Replace wall with door
            wallTilemap.SetTile(doorPosition, doorTile);

            // Add door collider and interaction
            GameObject doorObj = new GameObject($"Door_{connectedRoom}");
            doorObj.transform.position = new Vector3(doorPosition.x + 0.5f, doorPosition.y + 0.5f, 0);
            BoxCollider2D collider = doorObj.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(1f, 1f);

            // Add door interaction component
            DoorInteraction doorInteraction = doorObj.AddComponent<DoorInteraction>();
            doorInteraction.targetRoomName = connectedRoom;
        }

        private void AddRoomProps(string roomName, RoomTileSet tileSet, Vector2 start, Vector2 end)
        {
            // Add props based on room type
            // This is a simplified version - you'd want more variety based on room type

            // For example, add a table in the center of the room
            if (tileSet.propTiles.Count > 0)
            {
                int centerX = Mathf.FloorToInt(start.x + (end.x - start.x) / 2);
                int centerY = Mathf.FloorToInt(start.y + (end.y - start.y) / 2);

                propsTilemap.SetTile(new Vector3Int(centerX, centerY, 0), tileSet.propTiles[0]);
            }

            // In a complete implementation, you would add specific props based on the room type
            // For example, bookshelves in library, dining table in dining room, etc.
        }

        private RoomTileSet GetTileSetForRoom(string roomName)
        {
            // Check if we have a specific tileset for this room
            foreach (var tileSet in roomTileSets)
            {
                if (tileSet.roomType.ToLower() == roomName.ToLower())
                {
                    return tileSet;
                }
            }

            // Return default tileset
            return roomTileSets.Count > 0 ? roomTileSets[0] : CreateDefaultTileSet();
        }

        private RoomTileSet CreateDefaultTileSet()
        {
            RoomTileSet defaultSet = new RoomTileSet();
            defaultSet.roomType = "Default";
            defaultSet.floorTile = floorTile;
            defaultSet.wallTile = wallTile;

            return defaultSet;
        }

        public void ActivateRoom(string roomName)
        {
            currentRoomName = roomName;

            // In a complete implementation, you might only show the current room
            // and hide other rooms for performance reasons

            // For now, we'll keep all rooms visible
        }

        public Vector2 GetRoomPosition(string roomName)
        {
            if (roomPositions.ContainsKey(roomName))
            {
                // Return center of room
                Vector2 position = roomPositions[roomName];
                return new Vector2(
                    position.x + roomWidth * tileSize / 2,
                    position.y + roomHeight * tileSize / 2
                );
            }

            // Default to origin if room not found
            return Vector2.zero;
        }

        private void ClearTilemaps()
        {
            floorTilemap.ClearAllTiles();
            wallTilemap.ClearAllTiles();
            propsTilemap.ClearAllTiles();
        }
    }

    [System.Serializable]
    public class RoomTileSet
    {
        public string roomType;
        public Tile floorTile;
        public Tile wallTile;
        public List<Tile> propTiles;
    }
}