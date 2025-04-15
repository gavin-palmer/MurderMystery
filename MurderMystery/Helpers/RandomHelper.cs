using System;
using System.Collections.Generic;

namespace MurderMystery
{
    public static class RandomHelper
    {
        private static readonly Random rng = new Random();

        public static T PickRandom<T>(List<T> list)
        {
            return list[rng.Next(list.Count)];
        }

        public static T GetRandomEnumValue<T>() where T : Enum
        {
            Array values = Enum.GetValues(typeof(T));
            Random random = new Random();
            return (T)values.GetValue(random.Next(values.Length));
        }

    }
}
