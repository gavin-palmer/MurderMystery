using System;
using System.Collections.Generic;

namespace MurderMystery.Data
{
    public static class Weapons
    {
        public static List<string> Get()
        {
            return new List<string>() { "Knife", "Rope", "Poison" };
        }
    }
}
