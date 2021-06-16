namespace Sample8Models.Common
{
    public sealed class AppSettings
    {
        public string Host { get; set; }

        public Endpoints Endpoints { get; set; }

        public int Time2Retry { get; set; }
    }

    public class Endpoints
    {
        public string Cities { get; set; }
        public string Countries { get; set; }
    }
}
