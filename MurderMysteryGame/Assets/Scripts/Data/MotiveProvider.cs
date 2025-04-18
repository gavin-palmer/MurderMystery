using System;
using System.Collections.Generic;
using MurderMystery.Models;

namespace MurderMystery.Data.Providers
{
    public class MotiveProvider : BaseDataProvider<Motive>
    {
        protected override List<Motive> LoadItems()
        {
            return new List<Motive>
            {
                new Motive(
                    "Jealousy",
                    "Deep jealousy over a relationship or achievement",
                    new string[] {
                        "A diary suggests {0} was jealous of {1}",
                        "Love letters indicate {0} was jealous of {1}'s relationship",
                        "Photos show {0} looking enviously at {1}"
                    }
                ),
                new Motive(
                    "Revenge",
                    "A desire to pay back a perceived wrong",
                    new string[] {
                        "A letter shows {0} felt deeply wronged by {1}",
                        "Notes describe how {0} planned to get even with {1}",
                        "A journal entry reveals {0} blamed {1} for past misfortunes"
                    }
                ),
                new Motive(
                    "Greed",
                    "The desire for money or possessions",
                    new string[] {
                        "Financial records show {0} would profit from {1}'s death",
                        "Bank statements reveal {0} was in debt to {1}",
                        "A will indicates {0} would inherit {1}'s fortune"
                    }
                ),
                new Motive(
                    "Blackmail",
                    "Being threatened with exposure of a secret",
                    new string[] {
                        "A threatening note suggests {1} was blackmailing {0}",
                        "A hidden letter reveals {1} knew a dark secret about {0}",
                        "A diary entry shows {0} feared what {1} might reveal"
                    }
                ),
                new Motive(
                    "Love Triangle",
                    "Romantic complications between three people",
                    new string[] {
                        "Love letters show both {0} and {1} were involved with the same person",
                        "A diary reveals {0} and {1} were romantic rivals",
                        "Photos suggest a complicated relationship between {0}, {1} and another person"
                    }
                ),
                new Motive(
                    "Secret Knowledge",
                    "Knowing something that poses a danger to someone",
                    new string[] {
                        "A cryptic note suggests {1} discovered something dangerous about {0}",
                        "A journal entry indicates {1} was researching {0}'s past",
                        "A torn document shows {1} had evidence against {0}"
                    }
                )
            };
        }
    }
}