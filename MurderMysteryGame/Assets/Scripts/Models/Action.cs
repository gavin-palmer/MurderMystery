using System;
namespace MurderMystery.Models
{
    public class Action
    {
        public string Description { get; set; }
        public bool RequiresOtherPerson { get; set; }
        public PhysicalObject PhysicalObject { get; set; }
        public Action(string description, bool requiresOtherPerson = false, PhysicalObject physicalObject = PhysicalObject.None)
        {
            Description = description;
            RequiresOtherPerson = requiresOtherPerson;
            PhysicalObject = physicalObject;
        }

    }
    public enum PhysicalObject
    {
        None,
        Book,
        Drink,
        Letter,
        Cards,
        Magazine,
        Notebook,
        CigarettePack,
    }
}
