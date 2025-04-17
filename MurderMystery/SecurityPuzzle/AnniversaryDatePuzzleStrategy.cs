using MurderMystery.Enums;
using MurderMystery.Interfaces;
using MurderMystery.Models;
using MurderMystery;
using System;

public class AnniversaryDatePuzzleStrategy : ISecurityPuzzleStrategy
{
    public string Name => "AnniversaryDate";

    public SecurityInfo GeneratePuzzle(Random random, TimelineContext context)
    {
        var securityInfo = new SecurityInfo
        {
            SecurityRoom = "Security Room",
            PuzzleName = Name
        };

        // Randomly generate a month and day for the anniversary
        int month = random.Next(1, 13);
        int day = random.Next(1, 29); // Avoiding edge cases with month lengths

        // Store for reference
        securityInfo.StorePuzzleData("AnniversaryMonth", month);
        securityInfo.StorePuzzleData("AnniversaryDay", day);

        // Format as MMDD for the pin
        string pin = month.ToString("00") + day.ToString("00");
        securityInfo.PinCode = pin;

        // Create clues
        var anniversaryClue = new Clue($"The owner and his wife celebrated their anniversary on {month}/{day} every year", ClueType.Testimony);
        anniversaryClue.Location = "Dining Room";
        context.AddClue(anniversaryClue);

        var photoClue = new Clue($"There's a photo of the owner and his wife with '{month}/{day}' engraved on the frame", ClueType.Physical);
        photoClue.Location = "Master Bedroom";
        context.AddClue(photoClue);

        var hintClue = new Clue("The owner was notoriously sentimental and used important dates as his passwords", ClueType.Testimony);
        hintClue.Location = "Garden";
        context.AddClue(hintClue);

        return securityInfo;
    }
}