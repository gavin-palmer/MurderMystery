using System.Collections.Generic;
using UnityEngine;

public class RoomData
{
    public string Name { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Height { get; set; }
    public int Width { get; set; }
    public List<EntranceData> Entrances { get; set; }
}
public class EntranceData { }