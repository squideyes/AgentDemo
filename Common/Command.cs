using System.Collections.Generic;

namespace AgentDemo.Common
{
    public class Command
    {
        public CommandKind Kind { get; set; }
        public string TraderId { get; set; }
        public int AccountId { get; set; }
        public Dictionary<string, string> Settings { get; set; }

        public string GetTopicName() => $"{TraderId}";

        public string GetSubscriptionName() => $"{TraderId}-{AccountId:0000}";

        public override string ToString() => 
            $"{Kind.ToString().ToUpper()} {GetSubscriptionName()}";
    }
}
