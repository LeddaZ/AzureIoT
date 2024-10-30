namespace WebAPI
{
    public class Message
    {
        public required string Id { get; set; }
        public required string MessageID { get; set; }
        public required string DeviceId { get; set; }
        public double Value { get; set; }
        public DateTime DataRicezione { get; set; }
        public bool Received { get; set; }
    }

}
