using System;
using System.Collections.Generic;

namespace MurderMystery.Models
{
    public class Person
    {
        public string Name { get; set; }
        public string Job { get; set; }
        public string FunFact { get; set; }
        public List<Relationship> Relationships { get; set; }
    }
}
