    using System;
namespace MurderMystery.Models
{
    public class TimelineEvent
    {
        public string Time { get; set; }
        public Person Person { get; set; }
        public string Location { get; set; }
        public string Action { get; set; }
        public bool IsSecret { get; set; } = false;
        public bool IsLie { get; set; } = false;
    }
}
