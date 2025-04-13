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
            Console.WriteLine("Welcome to Murder Mystery!");
            Console.WriteLine("=========================\n");
            Console.WriteLine("Press any key to start a new game...");
            Console.ReadKey(true);

            // Start a new game
            StartNewGame();
        }

        static void StartNewGame()
        {
            // 1. Generate the mystery (murderer, victim, etc.)
            Console.WriteLine("Generating mystery...");
            var mystery = MysteryGenerator.CreateMystery();

            // 2. Generate the mansion layout
            Console.WriteLine("Creating mansion layout...");
            var mansionGenerator = new MansionGenerator();
            var mansion = mansionGenerator.GenerateMansionLayout();

            // 3. Place clues and people in the mansion
            Console.WriteLine("Placing clues and characters...");
            mansionGenerator.DistributeClues(mystery.Clues);
            mansionGenerator.PlacePeople(mystery.People, mystery.Timeline);

            // 4. Create the game state
            var gameState = new GameState(mystery, mansion);

            // 5. Create the game controller
            var gameController = new GameController(gameState);

            // 6. Start the game loop
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