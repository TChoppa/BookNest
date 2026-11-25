namespace BookNest.Services
{
    public class WaitlistOptions
    {
        public int OfferDurationHours { get; set; } = 48;
        public int ExpiryCheckIntervalSeconds { get; set; } = 60;
    }
}
