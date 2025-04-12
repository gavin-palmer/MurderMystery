using System;
using System.Collections.Generic;
using MurderMystery.Models;

namespace MurderMystery.Data
{
    public class WeaponProvider : BaseDataProvider<Weapon>
    {
        protected override List<Weapon> LoadItems()
        {
            return new List<Weapon>
            {
                new Weapon("Knife", "A sharp kitchen knife with a wooden handle"),
                new Weapon("Rope", "A length of sturdy rope, perfect for strangulation"),
                new Weapon("Poison", "A vial of deadly poison that leaves few traces"),
                new Weapon("Candlestick", "A heavy brass candlestick, an effective blunt instrument"),
                new Weapon("Revolver", "An elegant revolver with pearl grip, recently fired"),
                new Weapon("Lead Pipe", "A section of lead pipe, quite heavy and deadly"),
                new Weapon("Wrench", "A large wrench from the garage, covered in grease"),
                new Weapon("Dagger", "An ornate dagger with jewels in the hilt"),
                new Weapon("Letter Opener", "A silver letter opener, sharp enough to be lethal")
            };
        }
    }
}
