namespace BookM.Services
{
    // These classes match the JSON structure from Ticketmaster
    public class TicketmasterResponse
    {
        public EmbeddedData _embedded { get; set; }
    }

    public class EmbeddedData
    {
        public List<TMEvent> events { get; set; }
    }

    public class TMEvent
    {
        public string name { get; set; }
        public string id { get; set; }
        public List<TMImage> images { get; set; }
        public TMDates dates { get; set; }
        public TMEmbedded _embedded { get; set; }
        public List<TMClassification> classifications { get; set; }
    }

    public class TMImage { public string url { get; set; } }
    public class TMDates { public TMStart start { get; set; } }
    public class TMStart { public DateTime dateTime { get; set; } }
    public class TMEmbedded { public List<TMVenue> venues { get; set; } }
    public class TMVenue { public string name { get; set; } public TMCity city { get; set; } }
    public class TMCity { public string name { get; set; } }
    public class TMClassification { public TMSegment segment { get; set; } }
    public class TMSegment { public string name { get; set; } }
}