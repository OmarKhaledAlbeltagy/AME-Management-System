namespace AMEKSA.Entities
{
    public class EventSpeaker
    {
        public int Id { get; set; }

        public string SpeakerName { get; set; }

        public int EventId { get; set; }

        public Event Event { get; set; }
    }
}
