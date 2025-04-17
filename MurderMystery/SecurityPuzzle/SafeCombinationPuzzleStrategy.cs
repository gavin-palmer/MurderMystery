using MurderMystery.Enums;
using MurderMystery.Interfaces;
using MurderMystery.Models;
using MurderMystery;
using System.Collections.Generic;

using System;

/// <summary>
/// Safe Combination Puzzle: Each digit of the PIN is hidden in a different location
/// </summary>
public class SafeCombinationPuzzleStrategy : ISecurityPuzzleStrategy
{
    public string Name => "SafeCombination";

    public SecurityInfo GeneratePuzzle(Random random, TimelineContext context)
    {
        var securityInfo = new SecurityInfo
        {
            SecurityRoom = "Security Room",
            PuzzleName = Name
        };

        // Create a random 4-digit pin
        string pin = random.Next(1000, 10000).ToString("0000");
        securityInfo.PinCode = pin;

        // Store the digit locations for reference
        var digitLocations = new List<string> { "Study", "Drawing Room", "Hallway", "Lounge" };
        securityInfo.StorePuzzleData("DigitLocations", digitLocations);

        // Create clues for each digit
        var digit1Clue = new Clue($"The owner's favorite book is on page {pin[0]} of his personal journal", ClueType.Physical);
        digit1Clue.Location = digitLocations[0];
        context.AddClue(digit1Clue);

        var digit2Clue = new Clue($"There are {pin[1]} roses in the painting above the fireplace", ClueType.Physical);
        digit2Clue.Location = digitLocations[1];
        context.AddClue(digit2Clue);

        var digit3Clue = new Clue($"The antique clock in the hall is always set to {pin[3]} o'clock", ClueType.Physical);
        digit3Clue.Location = digitLocations[2];
        context.AddClue(digit3Clue);

        var digit4Clue = new Clue($"The owner always says his lucky number is {pin[3]}", ClueType.Testimony);
        digit4Clue.Location = digitLocations[3];
        context.AddClue(digit4Clue);

        // Create hint that these are connected
        var hintClue = new Clue("The security PIN is made up of the owner's favorite things, in order from his study to the lounge", ClueType.Testimony);
        hintClue.Location = "Library";
        context.AddClue(hintClue);

        return securityInfo;
    }
}