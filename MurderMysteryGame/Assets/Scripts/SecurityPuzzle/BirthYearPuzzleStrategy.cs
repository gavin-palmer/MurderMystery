using MurderMystery.Enums;
using MurderMystery.Interfaces;
using MurderMystery.Models;
using MurderMystery;

using System;

public class BirthYearPuzzleStrategy : ISecurityPuzzleStrategy
{
    public string Name => "BirthYear";

    public SecurityInfo GeneratePuzzle(Random random, TimelineContext context)
    {
        var securityInfo = new SecurityInfo
        {
            SecurityRoom = "Security Room",
            PuzzleName = Name
        };

        // Set birth year for the puzzle
        int currentYear = 2023; // Set to your game's year
        int ownerAge = random.Next(65, 90); // Random elderly age
        int birthYear = currentYear - ownerAge;

        // Store for later reference
        securityInfo.StorePuzzleData("OwnerBirthYear", birthYear);
        securityInfo.StorePuzzleData("OwnerAge", ownerAge);

        // Set PIN to the last 4 digits of birth year
        securityInfo.PinCode = (birthYear % 10000).ToString("0000");

        // Create gossip clues
        var ageClue = new Clue($"The owner just celebrated his {ownerAge}th birthday last week. He's quite proud of his age.", ClueType.Testimony);
        ageClue.Location = "Dining Room";
        context.AddClue(ageClue);

        var historyClue = new Clue($"The mansion owner often talks about being born in {birthYear}. He says it was a significant year in history.", ClueType.Testimony);
        historyClue.Location = "Library";
        context.AddClue(historyClue);

        var hintClue = new Clue("I overhead the security consultant tell the owner to never use his birth year as a PIN, but the old man is stubborn.", ClueType.Testimony);
        hintClue.Location = "Garden";
        context.AddClue(hintClue);

        return securityInfo;
    }
}

