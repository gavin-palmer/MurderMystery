using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MurderMystery.Data;
using MurderMystery.Enums;
using MurderMystery.Models;

namespace MurderMystery.Generators
{
    public static class PeopleGenerator
    {
        private static Random _random = new Random();

        public static List<Person> GeneratePeople()
        {
            int characterCount = _random.Next(5, 9);
            var names = DataProviderFactory.Names.GetRandomSelection(characterCount);

            var people = names.Select(n => new Person { Name = n }).ToList();

            SetOccupations(people);
            SetFootWear(people);
            SetTitles(people);
            return people;
        }

        private static void SetOccupations(List<Person> people)
        {
            var allOccupations = Enum.GetValues(typeof(Occupation))
                             .Cast<Occupation>()
                             .ToList();
            var shuffledOccupations = allOccupations.OrderBy(_ => Guid.NewGuid()).ToList();

            if (people.Count > shuffledOccupations.Count)
            {
                throw new InvalidOperationException(
                    $"Not enough unique occupations ({shuffledOccupations.Count}) for all people ({people.Count})");
            }

            for (int i = 0; i < people.Count; i++)
            {
                people[i].Occupation = shuffledOccupations[i];
            }
        }
        private static void SetFootWear(List<Person> people)
        {
            foreach (var person in people)
            {
                person.Footwear = GetAppropriateFootwear(person.Occupation);
            }
        }

        private static void SetTitles(List<Person> people)
        {
            foreach (var person in people)
            {
                person.Name = ApplyTitleToName(person.Name, person.Occupation);
            }
        }
        private static Footwear GetAppropriateFootwear(Occupation occupation)
        {
            switch (occupation)
            {
                // Formal service staff
                case Occupation.Butler:
                    return Footwear.OxfordShoe;
                case Occupation.Housekeeper:
                    return Footwear.BalletFlat;
                case Occupation.Chef:
                    return Footwear.ChefClog;

                // Outdoor workers
                case Occupation.Gardener:
                    return _random.Next(2) == 0 ? Footwear.Wellington : Footwear.WorkBoot;
                case Occupation.BotanicalExplorer:
                    return Footwear.HikingBoot;
                case Occupation.BigGameHunter:
                    return Footwear.HuntingBoot;

                // Military/Sports
                case Occupation.RetiredColonel:
                    return Footwear.MilitaryBoot;
                case Occupation.CricketCaptain:
                    return Footwear.CricketSpike;
                case Occupation.RacingDriver:
                    return Footwear.SportShoe;

                // Upper class
                case Occupation.Socialite:
                    return Footwear.HighHeel;
                case Occupation.WealthyIndustrialist:
                    return Footwear.BrogueShoe;
                case Occupation.OperaSinger:
                    return _random.Next(2) == 0 ? Footwear.PatentLeather : Footwear.BalletFlat;

                // Professional
                case Occupation.FamilyDoctor:
                    return Footwear.WalkingShoe;
                case Occupation.Librarian:
                    return Footwear.KittenHeel;
                case Occupation.FamilyLawyer:
                    return Footwear.WingtipShoe;
                case Occupation.ArtDealer:
                    return Footwear.LoaferShoe;
                case Occupation.PrivateInvestigator:
                    return Footwear.DressShoe;
                case Occupation.UniversityProfessor:
                    return Footwear.OxfordShoe;

                // Entertainment
                case Occupation.FamousAuthor:
                    return Footwear.Slipper;
                case Occupation.StageActress:
                    return Footwear.HighHeel;
                case Occupation.JazzMusician:
                    return Footwear.Sneaker;
                case Occupation.FilmDirector:
                    return Footwear.LoaferShoe;

                // Specialized
                case Occupation.Antiquarian:
                    return Footwear.DressShoe;
                case Occupation.ForeignDiplomat:
                    return Footwear.DressShoe;
                case Occupation.CardSharp:
                    return Footwear.LoaferShoe;
                case Occupation.MediumSpiritualist:
                    return Footwear.KittenHeel;
                case Occupation.Archaeologist:
                    return Footwear.HikingBoot;
                case Occupation.ChessGrandmaster:
                    return Footwear.WingtipShoe;

                // Fashion & Arts
                case Occupation.FashionDesigner:
                    return _random.Next(2) == 0 ? Footwear.HighHeel : Footwear.LaceupBoot;
                case Occupation.BallroomDancer:
                    return Footwear.DancingShoe;
                case Occupation.SocietyPhotographer:
                    return Footwear.Pump;

                // Nautical
                case Occupation.YachtCaptain:
                    return Footwear.DeckShoe;

                // Scientific
                case Occupation.PoisonExpert:
                    return Footwear.WalkingShoe;

                // Animal-related
                case Occupation.HorseTrainer:
                    return Footwear.RidingBoot;

                // Criminal
                case Occupation.JewelThief:
                    return Footwear.Unknown;

                // Default - random appropriate footwear
                default:
                    Footwear[] commonFootwear = {
                Footwear.DressShoe, Footwear.WalkingShoe, Footwear.OxfordShoe,
                Footwear.Sneaker, Footwear.LoaferShoe, Footwear.HighHeel
            };
                    return commonFootwear[_random.Next(commonFootwear.Length)];
            }
        }
        public static string ApplyTitleToName(string name, Occupation occupation)
        {
            // Get first and last name
            string[] nameParts = name.Split(' ');
            string firstName = nameParts[0];
            string lastName = string.Join(" ", nameParts.Skip(1)); // In case of multiple-barrelled surnames

            switch (occupation)
            {
                // Military ranks
                case Occupation.RetiredColonel:
                    return $"Col. {firstName} {lastName} (Ret.)";

                // Medical professionals
                case Occupation.FamilyDoctor:
                    return $"Dr. {firstName} {lastName}";
                case Occupation.PoisonExpert:
                    return $"Dr. {firstName} {lastName}";

                // Law professionals
                case Occupation.FamilyLawyer:
                    return _random.Next(3) == 0 ? $"Judge {firstName} {lastName}" : $"{firstName} {lastName}, Esq.";

                // Academic titles
                case Occupation.UniversityProfessor:
                    return _random.Next(3) == 0 ? $"Prof. {firstName} {lastName}" : $"Dr. {firstName} {lastName}";
                case Occupation.Archaeologist:
                    return $"Dr. {firstName} {lastName}";
                case Occupation.ChessGrandmaster:
                    return $"Grandmaster {firstName} {lastName}";

                // Religious/Spiritual
                case Occupation.MediumSpiritualist:
                    return $"Madame {lastName}";

                // Nobility/Honorary titles (randomly assigned, more rare)
                case Occupation.WealthyIndustrialist:
                case Occupation.ForeignDiplomat:
                case Occupation.BigGameHunter:
                case Occupation.Socialite:
                    // 15% chance of nobility title
                    if (_random.Next(100) < 15)
                    {
                        string[] maleTitles = { "Sir", "Lord", "Baron", "The Hon." };
                        string[] femaleTitles = { "Lady", "Dame", "Baroness", "The Hon." };

                        // Assuming first names ending with 'a' or other common female name endings are female
                        bool isFemale = firstName.EndsWith("a") || firstName.EndsWith("e") ||
                                       firstName.EndsWith("y") || firstName.EndsWith("ie") ||
                                       firstName.EndsWith("ine");

                        if (isFemale)
                        {
                            string title = femaleTitles[_random.Next(femaleTitles.Length)];
                            return title == "Lady" || title == "Baroness" ?
                                   $"{title} {lastName}" : $"{title} {firstName} {lastName}";
                        }
                        else
                        {
                            string title = maleTitles[_random.Next(maleTitles.Length)];
                            return title == "Sir" ?
                                   $"{title} {firstName} {lastName}" : $"{title} {lastName}";
                        }
                    }
                    return name;

                // Military officers
                case Occupation.CricketCaptain:
                    // 10% chance of military background
                    if (_random.Next(100) < 10)
                    {
                        string[] militaryRanks = { "Major", "Captain", "Lieutenant" };
                        return $"{militaryRanks[_random.Next(militaryRanks.Length)]} {firstName} {lastName}";
                    }
                    return $"Captain {firstName} {lastName}";

                // Nautical
                case Occupation.YachtCaptain:
                    return $"Captain {firstName} {lastName}";

                // Service staff - formal address
                case Occupation.Butler:
                    return lastName; // Butlers are traditionally addressed by last name only

                // Arts and performance titles
                case Occupation.OperaSinger:
                    // 20% chance of being referred to as "Maestro/Maestra" or "Prima Donna"
                    if (_random.Next(100) < 20)
                    {
                        bool isFemale = firstName.EndsWith("a") || firstName.EndsWith("e") ||
                                       firstName.EndsWith("y") || firstName.EndsWith("ie");

                        return isFemale ? $"Prima Donna {lastName}" : $"Maestro {lastName}";
                    }
                    return name;

                case Occupation.FamousAuthor:
                    // 10% chance of having a pen name
                    if (_random.Next(100) < 10)
                    {
                        return $"{firstName} '{lastName}' Smith"; // Random pen name example
                    }
                    return name;

                // Default - no title changes
                default:
                    return name;
            }
        }
    }
}
