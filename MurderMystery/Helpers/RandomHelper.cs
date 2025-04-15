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
        public static List<T> PickMultipleRandom<T>(IList<T> items, int count)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items), "Cannot pick from a null list");

            if (count <= 0)
                return new List<T>();

            if (count >= items.Count)
                return new List<T>(items); 

            var availableItems = new List<T>(items);
            var result = new List<T>(count);

            for (int i = 0; i < count; i++)
            {
                int index = rng.Next(availableItems.Count);
                result.Add(availableItems[index]);
                availableItems.RemoveAt(index); 
            }

            return result;
        }
        public static T GetRandomEnumValue<T>() where T : Enum
        {
            Array values = Enum.GetValues(typeof(T));
            Random random = new Random();
            return (T)values.GetValue(random.Next(values.Length));
        }

    }
}
