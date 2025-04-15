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
            Console.WriteLine("Welcome to Gavins Murder Mystery!");
            Console.WriteLine("=========================\n");
            Console.WriteLine("Press any key to start a new game...");
            Console.ReadKey(true);

            // Start a new game
            StartNewGame();

        }

        static void StartNewGame()
        {
            Console.WriteLine("Generating mystery...");
            var mystery = MysteryGenerator.CreateMystery();

            Console.WriteLine("Creating mansion layout...");
            var mansionGenerator = new MansionGenerator();
            var mansion = mansionGenerator.GenerateMansionLayout(mystery);

            Console.WriteLine("Placing clues and characters...");
            mansionGenerator.PlacePeople(mystery.People, mystery.Timeline);

            var gameState = new GameState(mystery, mansion);

            var gameController = new GameController(gameState);

            gameController.StartGame();
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