using System;
using System.Collections.Generic;
using System.Linq;
using MurderMystery.Data;
using MurderMystery.Enums;
using MurderMystery.Helpers;
using MurderMystery.Models;
using MurderMystery.Interfaces;
using MurderMystery.SecurityPuzzle;

namespace MurderMystery.Generators
{
    public class SecuritySystemGenerator
    {
        private readonly TimelineContext _context;
        private readonly Random _random;
        private readonly SecurityPuzzleManager _puzzleManager;

        public SecuritySystemGenerator(TimelineContext context, Random random)
        {
            _context = context;
            _random = random;
            _puzzleManager = new SecurityPuzzleManager(random);
        }

        public void SetupSecuritySystem()
        {
            var securityInfo = _puzzleManager.GenerateRandomPuzzle(_context);

            _context.SecurityRoom = securityInfo.SecurityRoom;
            _context.SecurityPinCode = securityInfo.PinCode;
            _context.SecurityInfo = securityInfo;

            var pinHintClue = new Clue($"Security room pin code: {_context.SecurityPinCode}", ClueType.Security);
            pinHintClue.Location = RandomHelper.PickRandom(DataProviderFactory.Rooms.GetAll().Select(r => r.Name).ToList());
            _context.AddClue(pinHintClue);

            SetupSecurityCameras();
        }


        public void SetupSecuritySystem(string puzzleName)
        {
            var securityInfo = _puzzleManager.GeneratePuzzleByName(puzzleName, _context);

            _context.SecurityRoom = securityInfo.SecurityRoom;
            _context.SecurityPinCode = securityInfo.PinCode;
            _context.SecurityInfo = securityInfo;

            var pinHintClue = new Clue($"Security room pin code: {_context.SecurityPinCode}", ClueType.Security);
            pinHintClue.Location = RandomHelper.PickRandom(DataProviderFactory.Rooms.GetAll().Select(r => r.Name).ToList());
            _context.AddClue(pinHintClue);

            SetupSecurityCameras();
        }

        public void RegisterCustomPuzzle(ISecurityPuzzleStrategy puzzle)
        {
            _puzzleManager.RegisterPuzzle(puzzle);
        }

        private void SetupSecurityCameras()
        {
            var availableRooms = DataProviderFactory.Rooms.GetAll()
                .Where(r => r.Name != _context.Room && r.Name != _context.SecurityRoom)
                .Select(r => r.Name)
                .ToList();

            int cameraCount = Math.Min(3, availableRooms.Count);
            _context.RoomsWithCameras = RandomHelper.PickMultipleRandom(availableRooms, cameraCount);

            if (_context.SecurityInfo != null)
            {
                _context.SecurityInfo.RoomsWithCameras = _context.RoomsWithCameras;
            }

            foreach (var cameraRoom in _context.RoomsWithCameras)
            {
                var cameraClue = new Clue($"Security camera installed in {cameraRoom}", ClueType.Security);
                cameraClue.Location = _context.SecurityRoom;
                _context.AddClue(cameraClue);
            }
        }
    }
}