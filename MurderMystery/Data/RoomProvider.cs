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
                new Room("Garden", "A meticulously maintained garden with vibrant flowerbeds, sculpted hedges, and a stone path winding through. A small ornamental pond reflects the sky, while trellises covered in climbing roses create secluded nooks perfect for private conversations. The fragrance of herbs mingles with floral scents, and carefully placed benches offer spots for quiet contemplation. In one corner stands a stone gazebo partially hidden by mature trees."),
                new Room("Drawing Room", "An elegant room for entertaining guests"),
                new Room("Master Bedroom", "The largest bedroom in the mansion, lavishly furnished"),
                new Room("Guest Suite", "A comfortable bedroom for visitors"),
                new Room("Observatory", "A domed room with a telescope for stargazing"),
                new Room("Gallery", "A long room lined with family portraits and artwork"),
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
        public List<Room> LoadNonePlayerRooms()
        {
            return new List<Room>
            {
                new Room("Foyer", "The grand entrance hall with marble floors and a sweeping staircase"),
                new Room ("Security Room", "A compact, utilitarian room dominated by a wall of surveillance monitors displaying various areas of the mansion. Computer equipment hums steadily beneath a desk cluttered with coffee mugs and technical manuals. A swivel chair sits before the main control panel, which features dozens of labeled buttons and switches. Filing cabinets line one wall, containing security logs and personnel records, while a key card access system restricts entry through the reinforced door. A small safe is embedded in the corner wall, and a bulletin board displays staff schedules and security protocols. The room has no windows, and the blue-tinged light from the monitors gives the space an eerie, artificial atmosphere."),
            };
        }
    }

}
