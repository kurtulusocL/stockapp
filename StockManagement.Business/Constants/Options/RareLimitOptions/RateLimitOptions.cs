namespace StockManagement.Business.Constants.Options.RareLimitOptions
{
    public class RateLimitOptions
    {
        public PolicySettings Web { get; set; }
        public PolicySettings SignalR { get; set; }
    }
}
