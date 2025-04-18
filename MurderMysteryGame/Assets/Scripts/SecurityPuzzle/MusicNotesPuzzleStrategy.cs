using MurderMystery.Enums;
using MurderMystery.Interfaces;
using MurderMystery.Models;
using MurderMystery;
using System.Collections.Generic;
using System;
using System.Linq;

public class MusicNotesPuzzleStrategy : ISecurityPuzzleStrategy
{
    public string Name => "MusicNotes";

    public SecurityInfo GeneratePuzzle(Random random, TimelineContext context)
    {
        var securityInfo = new SecurityInfo
        {
            SecurityRoom = "Security Room",
            PuzzleName = Name
        };

        // Map musical notes to numbers (C=1, D=2, E=3, F=4, G=5, A=6, B=7)
        var noteMapping = new Dictionary<string, char> {
                {"C", '1'}, {"D", '2'}, {"E", '3'}, {"F", '4'},
                {"G", '5'}, {"A", '6'}, {"B", '7'}, {"C#", '8'}, {"D#", '9'}
            };

        // Select 4 random notes
        var allNotes = noteMapping.Keys.ToList();
        var selectedNotes = new List<string>();
        for (int i = 0; i < 4; i++)
        {
            selectedNotes.Add(allNotes[random.Next(allNotes.Count)]);
        }

        // Store for reference
        securityInfo.StorePuzzleData("SelectedNotes", selectedNotes);
        securityInfo.StorePuzzleData("NoteMapping", noteMapping);

        // Generate PIN from notes
        string pin = "";
        foreach (var note in selectedNotes)
        {
            pin += noteMapping[note];
        }
        securityInfo.PinCode = pin;

        // Create clues
        var sheetMusicClue = new Clue($"A sheet of music on the piano has the notes {string.Join(", ", selectedNotes)} highlighted", ClueType.Physical);
        sheetMusicClue.Location = "Music Room";
        context.AddClue(sheetMusicClue);

        var hintClue = new Clue("The owner was a pianist and believed music was the key to everything in his life", ClueType.Testimony);
        hintClue.Location = "Drawing Room";
        context.AddClue(hintClue);

        var mappingClue = new Clue("There's a curious note in the music book mapping musical notes to numbers: C=1, D=2, E=3, F=4, G=5, A=6, B=7, C#=8, D#=9", ClueType.Physical);
        mappingClue.Location = "Music Room";
        context.AddClue(mappingClue);

        return securityInfo;
    }
}