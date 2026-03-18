namespace BookNest.DTO
{
    public class DashboradDTO
    {
        public string Name { get; set; }
        public string RoleName { get; set; }
        public int CartCount { get; set; }
        public string  OrderStatus { get; set; }
        public int PendingOrders { get; set; }
        public int ApprovedRecords { get; set; }
        public int ClearedRecords { get; set; } 
        public int ClubHostCount { get; set; }  
        public int CLubHostOngoingCount { get; set; } 
         public int CLubHostNotYetStartedCount { get; set; } 
         public int CLubHostExpiredCount { get; set; }
    }
}
