using System;
using System.Collections.Generic;
using System.Linq;
using MurderMystery.Interfaces;
using MurderMystery.Models;

namespace MurderMystery.SecurityPuzzle
{
    public class SecurityPuzzleManager
    {
        private readonly List<ISecurityPuzzleStrategy> _availablePuzzles;
        private readonly Random _random;

        public SecurityPuzzleManager(Random random)
        {
            _random = random;
            _availablePuzzles = new List<ISecurityPuzzleStrategy>();

            RegisterDefaultPuzzles();
        }

        private void RegisterDefaultPuzzles()
        {
            _availablePuzzles.Add(new BirthYearPuzzleStrategy());
            _availablePuzzles.Add(new SafeCombinationPuzzleStrategy());
            _availablePuzzles.Add(new AnniversaryDatePuzzleStrategy());
            _availablePuzzles.Add(new HistoricalReferencePuzzleStrategy());
            _availablePuzzles.Add(new MusicNotesPuzzleStrategy());
        }

        public void RegisterPuzzle(ISecurityPuzzleStrategy puzzle)
        {
            if (!_availablePuzzles.Any(p => p.Name == puzzle.Name))
            {
                _availablePuzzles.Add(puzzle);
            }
        }
        public ISecurityPuzzleStrategy GetPuzzleByName(string name)
        {
            return _availablePuzzles.FirstOrDefault(p => p.Name == name);
        }

        public ISecurityPuzzleStrategy GetRandomPuzzle()
        {
            if (_availablePuzzles.Count == 0)
                throw new InvalidOperationException("No puzzle strategies are registered");

            int index = _random.Next(_availablePuzzles.Count);
            return _availablePuzzles[index];
        }

        public SecurityInfo GenerateRandomPuzzle(TimelineContext context)
        {
            var strategy = GetRandomPuzzle();
            return strategy.GeneratePuzzle(_random, context);
        }

        public SecurityInfo GeneratePuzzleByName(string name, TimelineContext context)
        {
            var strategy = GetPuzzleByName(name);
            if (strategy == null)
                throw new ArgumentException($"No puzzle strategy found with name '{name}'");

            return strategy.GeneratePuzzle(_random, context);
        }
    }
}