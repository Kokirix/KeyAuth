namespace KeyAuth.Models {
    public class SubscriptionData {
        public string Subscription { get; internal set; }
        public string Key { get; internal set; }
        public string Expiry { get; internal set; }
        public long TimeLeft { get; internal set; }
    }
    public class ChatMessage {
        public string Message { get; internal set; }
        public string Author { get; internal set; }
        public string Timestamp { get; internal set; }
    }
}
// Made for KeyAuth | Completed by [Kokiri#8556]