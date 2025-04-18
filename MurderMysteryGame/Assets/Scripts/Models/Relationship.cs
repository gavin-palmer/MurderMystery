using System;
using MurderMystery.Enums;
using MurderMystery.Models;
namespace MurderMystery.Models
{
    public class Relationship
    {
        public Person Person1 { get; set; }
        public Person Person2 { get; set; }
        public RelationshipType RelationshipType { get; set; }
    }
}