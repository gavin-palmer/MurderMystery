using System;
namespace MurderMystery.Enums
{
    public enum SecretType
    {
        // Financial
        Debt,
        Embezzlement,
        Gambling,
        HiddenWealth,
        Inheritance,
        Insurance,

        // Relationship
        Affair,
        Blackmail,
        ForbiddenLove,
        HiddenRelative,
        SecretMarriage,
        UnwantedSuitor,

        // Past
        FakeIdentity,
        MilitaryDisgrace,
        PreviousCrime,
        RunawayHistory,
        Betrayal,

        // Knowledge
        DarkSecret,            // Knows someone else's secret
        WitnessedCrime,        // Saw something they shouldn't have
        IncriminatingDocument, // Has evidence of wrongdoing
        FamilySecret,          // Knows a scandalous family history

        // Vices
        Addiction,
        IllegalHobby,
        Smuggling,

        // Other
        FalseCredentials,     // Education/qualification/military rank is fake
        IllHealth,            // Hiding a serious illness
        DoubleLife,           // Has another identity elsewhere
        Spying                // Working for someone else secretly
    }
}