using System;
using System.Collections.Generic;

namespace MurderMystery
{
    public static class RandomHelper
    {
        public static T PickRandom<T>(List<T> list)
        {
            var rng = new Random();
            return list[rng.Next(list.Count)];
        }

    }
}
