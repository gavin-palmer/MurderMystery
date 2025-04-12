using System;
namespace MurderMystery.Models
{
    public class Action
    {
        public string Description { get; set; }
        public bool RequiresOtherPerson { get; set; }

        public Action(string description, bool requiresOtherPerson = false)
        {
            Description = description;
            RequiresOtherPerson = requiresOtherPerson;
        }
    }
}
