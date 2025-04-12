using System;
using System.Collections.Generic;
using MurderMystery.Models;

namespace MurderMystery.Data
{
    public class RoomProvider : BaseDataProvider<Room>
    {
        protected override List<Room> LoadItems()
        {
            return new List<Room>
            {
                new Room("Library", "Walls lined with books, comfortable reading chairs and a large desk"),
                new Room("Study", "A small room with a desk, typewriter, and bookshelves"),
                new Room("Conservatory", "A glass-walled room filled with exotic plants"),
                new Room("Ballroom", "A grand room with polished floors and chandeliers"),
                new Room("Kitchen", "A large kitchen with modern appliances and a central island"),
                new Room("Dining Room", "An elegant room with a long table that seats twelve"),
                new Room("Lounge", "A comfortable room with sofas and a fireplace"),
                new Room("Billiard Room", "A paneled room with a large billiard table and bar"),
                new Room("Servants' Quarters", "A plain room where the house staff gather"),
                new Room("Wine Cellar", "A cool underground room lined with wine racks"),
                new Room("Drawing Room", "An elegant room for entertaining guests"),
                new Room("Master Bedroom", "The largest bedroom in the mansion, lavishly furnished"),
                new Room("Guest Suite", "A comfortable bedroom for visitors"),
                new Room("Observatory", "A domed room with a telescope for stargazing"),
                new Room("Gallery", "A long room lined with family portraits and artwork")
            };
        }
    }
}
