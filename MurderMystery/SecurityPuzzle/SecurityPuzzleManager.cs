using System;
using System.Collections.Generic;
using System.Linq;
using MurderMystery.Interfaces;
using MurderMystery.Models;

namespace MurderMystery.SecurityPuzzle
{
    /// <summary>
    /// Manager class for security puzzles that selects and executes puzzle strategies
    /// </summary>
    public class SecurityPuzzleManager
    {
        private readonly List<ISecurityPuzzleStrategy> _availablePuzzles;
        private readonly Random _random;

        public SecurityPuzzleManager(Random random)
        {
            _random = random;
            _availablePuzzles = new List<ISecurityPuzzleStrategy>();

            // Register default puzzle strategies
            RegisterDefaultPuzzles();
        }

        /// <summary>
        /// Register all default puzzle types
        /// </summary>
        private void RegisterDefaultPuzzles()
        {
            _availablePuzzles.Add(new BirthYearPuzzleStrategy());
            _availablePuzzles.Add(new SafeCombinationPuzzleStrategy());
            _availablePuzzles.Add(new AnniversaryDatePuzzleStrategy());
            _availablePuzzles.Add(new HistoricalReferencePuzzleStrategy());
            _availablePuzzles.Add(new MusicNotesPuzzleStrategy());
        }

        /// <summary>
        /// Register a custom puzzle strategy
        /// </summary>
        public void RegisterPuzzle(ISecurityPuzzleStrategy puzzle)
        {
            if (!_availablePuzzles.Any(p => p.Name == puzzle.Name))
            {
                _availablePuzzles.Add(puzzle);
            }
        }

        /// <summary>
        /// Get a specific puzzle strategy by name
        /// </summary>
        public ISecurityPuzzleStrategy GetPuzzleByName(string name)
        {
            return _availablePuzzles.FirstOrDefault(p => p.Name == name);
        }

        /// <summary>
        /// Get a random puzzle strategy
        /// </summary>
        public ISecurityPuzzleStrategy GetRandomPuzzle()
        {
            if (_availablePuzzles.Count == 0)
                throw new InvalidOperationException("No puzzle strategies are registered");

            int index = _random.Next(_availablePuzzles.Count);
            return _availablePuzzles[index];
        }

        /// <summary>
        /// Generate a random puzzle
        /// </summary>
        public SecurityInfo GenerateRandomPuzzle(TimelineContext context)
        {
            var strategy = GetRandomPuzzle();
            return strategy.GeneratePuzzle(_random, context);
        }

        /// <summary>
        /// Generate a specific puzzle by name
        /// </summary>
        public SecurityInfo GeneratePuzzleByName(string name, TimelineContext context)
        {
            var strategy = GetPuzzleByName(name);
            if (strategy == null)
                throw new ArgumentException($"No puzzle strategy found with name '{name}'");

            return strategy.GeneratePuzzle(_random, context);
        }
    }
}