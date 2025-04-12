using System;
namespace MurderMystery.Enums
{
    public enum Proof
    {
        None,           // Default: no proof exists
        Alibi,          // Another character confirms their presence
        PhysicalEvidence, // Item or environmental clue confirms presence (e.g., lipstick on a glass)
        Surveillance,   // Seen on camera, mirror, reflection, etc.
        DiaryEntry,     // Character logs their own whereabouts truthfully (or verifiably)
        WitnessStatement, // A third-party (e.g., servant) confirms their presence or actions
        AudioEvidence,  // Overheard talking, humming, etc. via intercom, phone, etc.
        ForensicTrace,  // Fingerprints, footprints, cigarette ashes, etc.
        Contradiction,  // A statement disproves another's lie indirectly (e.g., "They weren't there")
        RoomLog,        // A logbook, keycard access, or guest log entry
        TimedObject,    // Something time-based like a stopped watch, a warming kettle, or auto-played music
        LetterOrNote,   // Written material indicating presence or intention
        SceneEvidence   // Specific trace in the environment (e.g., mud in carpet, blood smear in room),

    }
}
