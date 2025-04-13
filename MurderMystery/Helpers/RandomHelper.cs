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

        public static T GetRandomEnumValue<T>() where T : Enum
        {
            Array values = Enum.GetValues(typeof(T));
            Random random = new Random();
            return (T)values.GetValue(random.Next(values.Length));
        }

    }
}
