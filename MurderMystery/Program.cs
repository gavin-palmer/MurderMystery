using System;
using System.Collections.Generic;
using System.Linq;
using MurderMystery;
using MurderMystery.Data;
using MurderMystery.Generators;
using MurderMystery.Models;

namespace MysteryGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("🔍 MURDER MYSTERY GENERATOR 🔍");
            Console.WriteLine("==================================\n");

            // Generate a mystery using our new approach
            var mystery = MysteryGenerator.CreateMystery();

            // Print all the details
            mystery.PrintAll();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}