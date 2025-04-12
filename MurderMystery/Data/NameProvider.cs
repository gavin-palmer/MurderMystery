using System;
using System.Collections.Generic;
using MurderMystery.Data.Providers;

namespace MurderMystery.Data.Providers
{
    public class NameProvider : BaseDataProvider<string>
    {
        protected override List<string> LoadItems()
        {
            return new List<string>
            {
                // Original names
                "Alice", "Bob", "Clara", "David", "Eve", "Frank",
                
                // Additional names
                "George", "Hannah", "Isaac", "Julia", "Kenneth", "Laura",
                "Michael", "Natalie", "Oliver", "Penelope", "Quentin", "Rose",
                "Samuel", "Tiffany", "Ulysses", "Victoria", "William", "Xenia",
                "Zachary", "Agatha", "Bernard", "Catherine", "Daniel", "Eleanor",
                "Frederick", "Grace", "Henry", "Isabelle", "Jonathan", "Katherine"
            };
        }

        /// <summary>
        /// Gets a random selection of names with the specified count
        /// </summary>
        public List<string> GetRandomSelection(int count)
        {
            var allNames = GetAll();

            // Make sure we don't request more names than available
            count = Math.Min(count, allNames.Count);

            // Shuffle the list
            var shuffled = new List<string>(allNames);
            for (int i = shuffled.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                var temp = shuffled[i];
                shuffled[i] = shuffled[j];
                shuffled[j] = temp;
            }

            // Return the requested number of names
            return shuffled.GetRange(0, count);
        }
    }
}