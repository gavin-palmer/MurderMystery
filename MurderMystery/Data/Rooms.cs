using System;
using System.Collections.Generic;

namespace MurderMystery.Data
{
    public static class Rooms
    {
        public static List<string> Get()
        {
            return new List<string>() {
                "Library",
                "Study",
                "Conservatory",
                "Ballroom",
                "Kitchen",
                "Dining Room",
                "Lounge",
                "Billiard Room",
                "Servants’ Quarters",
                "Wine Cellar"
            }; 
        }
    }
}
