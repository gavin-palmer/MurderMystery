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

            var mystery = MysteryGenerator.CreateMystery();

            CheckForSolved(mystery);

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        private static void CheckForSolved(Mystery mystery)
        {
            mystery.PrintAll(false);

            Console.WriteLine("Have you solved it? Enter Y or N");
            var solved = Console.ReadLine();
            if (solved == "Y")
            {
                mystery.PrintAll(true);
            }
            else
            {
                CheckForSolved(mystery);
            }
        }
    }
}