using MurderMystery.Enums;

namespace MurderMystery.Models
{
    public class TimelineEvent
    {
        public string Time { get; set; }
        public Person Person { get; set; }
        public string Location { get; set; }
        public string Action { get; set; }
        public bool IsSecret { get; set; }
        public bool IsLie { get; set; }
        public Proof Proof { get; set; } = Proof.None;

        public bool HasProof => Proof != Proof.None;

        public string Description()
        {
            return $"{Time}: I was in {Location}: {Action}";
        }
    }
}