using MurderMystery.Enums;
using MurderMystery.Interfaces;
using MurderMystery.Models;
using MurderMystery;
using System.Collections.Generic;
using System;
using System.Linq;

public class HistoricalReferencePuzzleStrategy : ISecurityPuzzleStrategy
{
    public string Name => "HistoricalReference";

    public SecurityInfo GeneratePuzzle(Random random, TimelineContext context)
    {
        var securityInfo = new SecurityInfo
        {
            SecurityRoom = "Security Room",
            PuzzleName = Name
        };

        // Historical references with years
        var historicalEvents = new Dictionary<string, string> {
                {"1815", "Battle of Waterloo"},
                {"1865", "End of Civil War"},
                {"1912", "Sinking of the Titanic"},
                {"1929", "Stock Market Crash"},
                {"1945", "End of World War II"},
                {"1969", "Moon Landing"}
            };

        // Randomly select an event
        var eventIndex = random.Next(historicalEvents.Count);
        var selectedEvent = historicalEvents.ElementAt(eventIndex);
        string pin = selectedEvent.Key;
        securityInfo.PinCode = pin;

        // Store for reference
        securityInfo.StorePuzzleData("HistoricalEvent", selectedEvent.Value);
        securityInfo.StorePuzzleData("EventYear", selectedEvent.Key);

        // Create clues
        var bookClue = new Clue($"The owner's history book is open to a chapter about the {selectedEvent.Value}", ClueType.Physical);
        bookClue.Location = "Library";
        context.AddClue(bookClue);

        var conversationClue = new Clue($"The owner frequently mentioned how fascinated he was by the {selectedEvent.Value}", ClueType.Testimony);
        conversationClue.Location = "Lounge";
        context.AddClue(conversationClue);

        var hintClue = new Clue("The owner was passionate about history and used significant dates for his personal codes", ClueType.Testimony);
        hintClue.Location = "Study";
        context.AddClue(hintClue);

        return securityInfo;
    }
}