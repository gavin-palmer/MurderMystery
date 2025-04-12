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
                new Room("Gallery", "A long room lined with family portraits and artwork"),
                new Room("Foyer", "The grand entrance hall with marble floors and a sweeping staircase"),
                new Room("Music Room", "A refined room with a grand piano and shelves of sheet music"),
                new Room("Greenhouse", "A humid glass structure with rows of rare and exotic plants"),
                new Room("Armory", "A locked room displaying antique weapons and armor"),
                new Room("Chapel", "A quiet, candle-lit space with stained glass and wooden pews"),
                new Room("Smoking Room", "A dimly lit room with leather chairs and cigar smoke lingering"),
                new Room("Pantry", "A narrow room off the kitchen filled with preserved goods and spices"),
                new Room("Servants' Stairs", "A cramped stairwell used by the staff to move unseen"),
                new Room("Trophy Room", "A wood-paneled room filled with hunting trophies and animal heads"),
                new Room("Basement Hall", "A shadowy hallway beneath the house, lined with old storage crates")
            };
        }
    }
}
