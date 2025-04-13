using System;
using System.Collections.Generic;
using MurderMystery.Enums;

namespace MurderMystery.Models
{
    public class Person
    {
        public string Name { get; set; }
        public string Job { get; set; }
        public string FunFact { get; set; }
        public List<Relationship> Relationships { get; set; }
        public bool IsVictim { get; set; }
        public string CurrentRoom { get; set; }
        public Footwear Footwear { get; set; }

        public string GenerateStatement()
        {
            return "Hello! I'm just a test at the moment and don't know anythinig :( ";
        }
    }

}
