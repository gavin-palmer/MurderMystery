using System;
using System.Collections.Generic;
using System.Linq;
using MurderMystery.Models;
using MurderMystery.Enums;
using MurderMystery.Data;

namespace MurderMystery.Generators
{
    public static class SecretGenerator
    {
        private static Random _random = new Random();

        private static readonly Dictionary<SecretType, List<string>> SecretTemplates = new Dictionary<SecretType, List<string>>
        {
            { SecretType.Debt, new List<string> {
                "is deeply in debt to {0}",
                "owes a large sum of money to {0}",
                "has been borrowing money from {0} to cover gambling losses",
                "secretly pawned family heirlooms to repay {0}"
            }},
            { SecretType.Embezzlement, new List<string> {
                "has been skimming money from {0}'s accounts",
                "embezzled funds from {0}",
                "has been falsifying financial records for {0}",
                "diverted {0}'s inheritance into a secret account"
            }},
            { SecretType.Gambling, new List<string> {
                "has a severe gambling addiction unknown to {0}",
                "lost a family heirloom in a card game with {0}",
                "owes gambling debts to {0}",
                "has been gambling away money intended for {0}"
            }},
            { SecretType.HiddenWealth, new List<string> {
                "has a fortune hidden from {0}",
                "inherited a large sum of money but told {0} they were disinherited",
                "keeps a secret safe deposit box unknown to {0}",
                "sold valuable items and hid the money from {0}"
            }},
            { SecretType.Inheritance, new List<string> {
                "is the secret beneficiary in {0}'s will",
                "changed {0}'s will without their knowledge",
                "forged documents to claim {0}'s inheritance",
                "is plotting to contest {0}'s will after their death"
            }},
            { SecretType.Insurance, new List<string> {
                "has taken out a substantial life insurance policy on {0}",
                "is the beneficiary of {0}'s insurance policy",
                "filed a fraudulent insurance claim after {0}'s 'accident'",
                "forged {0}'s signature on insurance documents"
            }},
            
            { SecretType.Affair, new List<string> {
                "is having an affair with {0}",
                "had a one-night stand with {0} at the last garden party",
                "has been secretly meeting {0} for months",
                "exchanged passionate letters with {0}"
            }},
            { SecretType.Blackmail, new List<string> {
                "is blackmailing {0} over a past indiscretion",
                "received blackmail threats from {0}",
                "has evidence that could ruin {0}'s reputation",
                "paid {0} to keep quiet about a scandal"
            }},
            { SecretType.ForbiddenLove, new List<string> {
                "is secretly in love with {0}",
                "kept a locket with {0}'s picture for years",
                "follows {0}'s career with unusual interest",
                "writes unsent love letters to {0}"
            }},
            { SecretType.HiddenRelative, new List<string> {
                "is actually {0}'s long-lost sibling",
                "is {0}'s parent, but neither of them knows",
                "discovered that {0} is their illegitimate child",
                "has hidden family ties to {0}"
            }},
            { SecretType.SecretMarriage, new List<string> {
                "was briefly married to {0} years ago",
                "has a marriage certificate with {0} hidden among their possessions",
                "eloped with {0} but had it annulled",
                "is still legally married to {0} despite the 'divorce'"
            }},
            { SecretType.UnwantedSuitor, new List<string> {
                "has been sending anonymous gifts to {0}",
                "follows {0} more closely than appropriate",
                "kept a diary detailing every interaction with {0}",
                "has a concerning collection of items belonging to {0}"
            }},
            
            { SecretType.FakeIdentity, new List<string> {
                "is living under a false identity unknown to {0}",
                "has fraudulent credentials unknown to {0}",
                "is not who they claim to be, and {0} suspects",
                "has been hiding their true identity from {0}"
            }},
            { SecretType.MilitaryDisgrace, new List<string> {
                "was dishonorably discharged but told {0} it was honorable",
                "abandoned their post during battle, witnessed by {0}",
                "took credit for {0}'s heroic actions during service",
                "has been lying about their military record to {0}"
            }},
            { SecretType.PreviousCrime, new List<string> {
                "was involved in a robbery with {0} years ago",
                "helped {0} cover up a crime",
                "has evidence of {0}'s involvement in an unsolved crime",
                "served time in prison - something {0} could reveal"
            }},
            { SecretType.RunawayHistory, new List<string> {
                "ran away from home with {0} in their youth",
                "helped {0} disappear and start a new life",
                "knows {0}'s true origins and why they fled",
                "threatened to expose {0}'s past"
            }},
            { SecretType.Betrayal, new List<string> {
                "betrayed {0}'s confidence by revealing private information",
                "sold {0} out to a competitor",
                "sabotaged {0}'s project to advance their own career",
                "testified against {0} in a private inquiry"
            }},
            
            { SecretType.DarkSecret, new List<string> {
                "knows about {0}'s involvement in a scandal",
                "discovered {0}'s secret past",
                "overheard {0} confessing to something terrible",
                "has been investigating {0}'s background"
            }},
            { SecretType.WitnessedCrime, new List<string> {
                "witnessed {0} committing a crime",
                "saw {0} at the scene of an accident",
                "knows {0} wasn't where they claimed to be",
                "has evidence contradicting {0}'s alibi"
            }},
            { SecretType.IncriminatingDocument, new List<string> {
                "possesses documents that could ruin {0}",
                "found {0}'s incriminating diary",
                "keeps letters proving {0}'s indiscretion",
                "has photographs showing {0} somewhere compromising"
            }},
            { SecretType.FamilySecret, new List<string> {
                "knows the truth about {0}'s family history",
                "discovered {0}'s ancestor's crimes",
                "found evidence disproving {0}'s noble lineage",
                "uncovered that {0}'s inheritance is based on fraud"
            }},
            
            { SecretType.Addiction, new List<string> {
                "shares a substance habit with {0}",
                "supplies {0} with prohibited substances",
                "knows about {0}'s struggle with addiction",
                "attends the same recovery meetings as {0}"
            }},
            { SecretType.IllegalHobby, new List<string> {
                "participates in illegal gaming with {0}",
                "shares an underground hobby with {0}",
                "collects prohibited items with help from {0}",
                "engages in questionable activities alongside {0}"
            }},
            { SecretType.Smuggling, new List<string> {
                "smuggles goods with the help of {0}",
                "moves illegal items through {0}'s property",
                "uses {0}'s connections to import contraband",
                "operates a smuggling ring that {0} discovered"
            }},
            
            { SecretType.FalseCredentials, new List<string> {
                "knows {0}'s credentials are fraudulent",
                "helped {0} forge documents",
                "threatens to expose {0}'s false qualifications",
                "has been investigating {0}'s background"
            }},
            { SecretType.IllHealth, new List<string> {
                "knows about {0}'s terminal illness",
                "has been covering up {0}'s declining health",
                "secretly administers medication to {0}",
                "altered {0}'s medical test results"
            }},
            { SecretType.DoubleLife, new List<string> {
                "discovered {0} has another family elsewhere",
                "follows {0} and knows about their second identity",
                "has proof of {0}'s double life",
                "threatens to tell others about {0}'s secret career"
            }},
            { SecretType.Spying, new List<string> {
                "has been spying on {0} for someone else",
                "reports {0}'s movements to an unknown party",
                "photographs {0}'s documents when unobserved",
                "was hired to investigate {0}"
            }}
        };

        public static string GenerateSecret(Person person, Person about, SecretType secretType)
        {
            var templates = SecretTemplates[secretType];

            string template = templates[_random.Next(templates.Count)];

            return string.Format(template, about.Name);
        }

        public static string GenerateRandomSecret(Person person, Person about)
        {
            var secretTypes = Enum.GetValues(typeof(SecretType)).Cast<SecretType>().ToArray();
            SecretType randomType = secretTypes[_random.Next(secretTypes.Length)];

            return GenerateSecret(person, about, randomType);
        }

        public static string GenerateMotiveSecret(Person murderer, Person victim, string motive)
        {
            Dictionary<string, List<SecretType>> motiveSecretMap = new Dictionary<string, List<SecretType>>(StringComparer.OrdinalIgnoreCase)
            {
                { "Revenge", new List<SecretType> { SecretType.Betrayal, SecretType.Blackmail, SecretType.PreviousCrime } },
                { "Money", new List<SecretType> { SecretType.Debt, SecretType.Embezzlement, SecretType.Inheritance, SecretType.Insurance } },
                { "Jealousy", new List<SecretType> { SecretType.Affair, SecretType.ForbiddenLove } },
                { "Knowledge", new List<SecretType> { SecretType.DarkSecret, SecretType.WitnessedCrime, SecretType.IncriminatingDocument } },
                { "Power", new List<SecretType> { SecretType.Blackmail, SecretType.FakeIdentity, SecretType.DoubleLife } },
                { "Fear", new List<SecretType> { SecretType.Blackmail, SecretType.WitnessedCrime, SecretType.DarkSecret } },
                { "Accident", new List<SecretType> { SecretType.WitnessedCrime, SecretType.Addiction, SecretType.IllHealth } }
            };

            List<SecretType> appropriateSecrets;
            if (motiveSecretMap.ContainsKey(motive))
            {
                appropriateSecrets = motiveSecretMap[motive];
            }
            else
            {
                appropriateSecrets = new List<SecretType> {
                    SecretType.Blackmail, SecretType.Betrayal, SecretType.PreviousCrime,
                    SecretType.DarkSecret, SecretType.Affair
                };
            }

            SecretType secretType = appropriateSecrets[_random.Next(appropriateSecrets.Count)];

            return GenerateSecret(murderer, victim, secretType);
        }

        public static void AssignSecrets(Mystery mystery)
        {
            string motiveSecret = GenerateMotiveSecret(mystery.Murderer, mystery.Victim, mystery.Motive);
            mystery.Murderer.Secrets.Add(motiveSecret);

            // Then give each character 0-2 random secrets
            foreach (var person in mystery.People)
            {
                // Skip the victim (dead people tell no tales)
                if (person == mystery.Victim) continue;

                // Determine how many secrets this person has (0-2)
                int secretCount = _random.Next(3); // 0, 1, or 2

                for (int i = 0; i < secretCount; i++)
                {
                    // Pick a random person who isn't themselves
                    var about = RandomHelper.PickRandom(
                        mystery.People.Where(p => p != person).ToList()
                    );

                    // Generate a random secret about that person
                    string secret = GenerateRandomSecret(person, about);

                    // Add it to their secrets
                    person.Secrets.Add(secret);
                }
            }

            // Add some clues related to secrets
            GenerateSecretClues(mystery);
        }

        // Generate clues related to character secrets
        private static void GenerateSecretClues(Mystery mystery)
        {
            // Loop through people with secrets
            foreach (var person in mystery.People.Where(p => p.Secrets.Any()))
            {
                // For each secret, there's a 30% chance of generating a clue
                foreach (var secret in person.Secrets)
                {
                    if (_random.Next(100) < 30)
                    {
                        // Create a clue based on the secret
                        var clue = new Clue
                        {
                            Description = GenerateClueFromSecret(person, secret),
                            Type = ClueType.Personal,
                            Location = GetAppropriateLocation(mystery, person, secret)
                        };

                        // Add to the mystery's clues list
                        mystery.Clues.Add(clue);
                    }
                }
            }
        }

        // Generate a clue description based on a secret
        private static string GenerateClueFromSecret(Person person, string secret)
        {
            string[] clueTemplates = {
                "A crumpled note that reads: \"{0}\"",
                "A page torn from a diary: \"{0}\"",
                "An overheard whisper about how {0}",
                "A letter addressed to {1} that mentions \"{0}\"",
                "A photograph that suggests {0}",
                "A newspaper clipping related to {1} that implies {0}",
                "A key labeled with initials that could connect to the fact that {0}",
                "A receipt showing a transaction that supports that {0}",
                "A ticket stub that corroborates {0}"
            };

            string template = clueTemplates[_random.Next(clueTemplates.Length)];

            // Format appropriately based on template
            if (template.Contains("{1}"))
            {
                return string.Format(template, secret, person.Name);
            }
            else
            {
                return string.Format(template, secret);
            }
        }

        // Choose an appropriate location for the clue
        private static string GetAppropriateLocation(Mystery mystery, Person person, string secret)
        {
            // 50% chance to be in the person's last known location
            if (_random.Next(100) < 50)
            {
                return person.CurrentRoom;
            }

            // 20% chance to be in a location mentioned in the secret
            // (This is simplistic - in reality you'd want to parse the secret text)
            if (_random.Next(100) < 20)
            {
                // Try to extract a room name from the secret
                foreach (var roomName in DataProviderFactory.Rooms.GetAll().Select(r => r.Name))
                {
                    if (secret.Contains(roomName))
                    {
                        return roomName;
                    }
                }
            }

            // Otherwise pick a random room
            return RandomHelper.PickRandom(DataProviderFactory.Rooms.GetAll()).Name;
        }
    }
}