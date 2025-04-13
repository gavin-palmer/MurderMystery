using System;
using System.Collections.Generic;
using MurderMystery.Data.Providers;

namespace MurderMystery.Data.Providers
{
    public class NameProvider : BaseDataProvider<string>
    {
        private readonly int nameCount = 35;
        protected override List<string> LoadItems()
        {
            var names = new List<string>();
            for (var i = 0; i < nameCount; i++)
            {
                names.Add(GenerateName());
            }
            return names;

        }

        private List<string> FirstNames => new List<string>
        {
            "Arthur", "Agatha", "Hercule", "Jane", "Dorothy", "Sherlock", "Reginald", "Mabel",
            "Edmund", "Geoffrey", "Violet", "Clarence", "Edith", "Winston", "Beatrice", "Cecil",
            "Gwendolyn", "Alfred", "Imogen", "Wilfred", "Marigold", "Bartholomew", "Millicent",
            "Montgomery", "Penelope", "Barnaby", "Clementine", "Rupert", "Euphemia", "Cornelius",
            "Jemima", "Phineas", "Prudence", "Quentin", "Wilhelmina", "Archibald", "Theodora",
            "Eleanor", "Frederick", "Henrietta", "Thaddeus", "Josephine", "Benedict", "Lavinia",
            "Bertram", "Octavia", "Alastair", "Dorothea", "Percival", "Cordelia", "Crispin",
            "Lucinda", "Sebastian", "Araminta", "Horatio", "Cecilia", "Ambrose", "Tabitha",
            "Mortimer", "Victoria", "Clifford", "Ophelia", "Lionel", "Augusta", "Theodore",
            "Harriet", "Emmett", "Genevieve", "Orville", "Florence", "Ellsworth", "Constance",
            "Jasper", "Winifred", "Ferdinand", "Hortense", "Ignatius", "Annabelle", "Humphrey",
            "Rosamund", "Nathaniel", "Daphne", "Thaddeus", "Eloise", "Ernest", "Antoinette",
            "Wendell", "Minerva", "Oswald", "Philomena", "Delbert", "Hyacinth", "Cyril",
            "Georgiana", "Silas", "Evangeline", "Augustus", "Dahlia", "Prentice", "Matilda"
        };

        private List<string> Surnames => new List<string>
        {
            "Smith", "Jones", "Williams", "Brown", "Taylor", "Davies", "Wilson", "Evans",
            "Thomas", "Roberts", "Johnson", "Lewis", "Walker", "Robinson", "Wood", "Thompson",
            "White", "Watson", "Jackson", "Wright", "Green", "Harris", "Cooper", "King",
            "Blackwood", "Montague", "Pemberton", "Sinclair", "Fitzroy", "Hawthorne", "Lancaster",
            "Thornfield", "Beaumont", "Fairfax", "Harrington", "Winchester", "Kingsley", "Wentworth",
            "Holmes", "Poirot", "Marple", "Christie", "Wimsey", "Vane", "Fletcher", "Morse",
            "Alleyn", "Campion", "Templar", "Maigret", "Wolfe", "Marlowe", "Spade", "Columbo",
            "Sterling", "Montgomery", "Rothschild", "Vanderbilt", "Rockefeller", "Astor", "Carnegie",
            "Wellington", "Huntington", "Cromwell", "Remington", "Rutherford", "Livingston", "Covington",
            "Prescott", "Worthington", "Pennington", "Whitmore", "Lockwood", "Hollingsworth", "Westbrook",
            "Sheffield", "Knightley", "Whittaker", "Ainsworth", "Chatsworth", "Longbottom", "Endicott",
            "Thackeray", "Woolsey", "Woodhouse", "Carrington", "Holloway", "Barrington", "Wadsworth",
            "Chamberlain", "Talbot", "Farnsworth", "Buckingham", "Hartley", "Chesterfield", "Everett"
        };

        private List<string> MiddleParticles => new List<string>
        {
            "von", "van", "van der", "de", "del", "della", "di", "da", "des", "du", "of", "le", "la"
        };

        private string GenerateName()
        {
            var firstName = RandomHelper.PickRandom(FirstNames);
            var lastName = RandomHelper.PickRandom(Surnames);

            if (_random.NextDouble() < 0.1)
            {
                string secondLastName = DataProviderFactory.Names.GetRandom();
                while (secondLastName == lastName || secondLastName == firstName)
                {
                    secondLastName = DataProviderFactory.Names.GetRandom();
                }
                lastName = $"{lastName}-{secondLastName}";

                if (_random.NextDouble() < 0.2)
                {
                    string thirdLastName = DataProviderFactory.Names.GetRandom();
                    while (thirdLastName == lastName.Split('-')[0] ||
                           thirdLastName == lastName.Split('-')[1] ||
                           thirdLastName == firstName)
                    {
                        thirdLastName = DataProviderFactory.Names.GetRandom();
                    }
                    lastName = $"{lastName}-{thirdLastName}";
                }
            }
            if (_random.NextDouble() < 0.02)
            {
                string middleParticle = RandomHelper.PickRandom(MiddleParticles);
                lastName = $"{middleParticle} {lastName}";
            }

            if (_random.NextDouble() < 0.05)
            {
                if (!lastName.EndsWith("son", StringComparison.OrdinalIgnoreCase))
                {
                    lastName = $"{lastName}son";
                }
            }

            return $"{firstName} {lastName}";
        }
        public List<string> GetRandomSelection(int count)
        {
            var allNames = GetAll();

            // Make sure we don't request more names than available
            count = Math.Min(count, allNames.Count);

            // Shuffle the list
            var shuffled = new List<string>(allNames);
            for (int i = shuffled.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                var temp = shuffled[i];
                shuffled[i] = shuffled[j];
                shuffled[j] = temp;
            }

            // Return the requested number of names
            return shuffled.GetRange(0, count);
        }
    }
}