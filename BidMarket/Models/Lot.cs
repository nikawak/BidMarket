using BidMarket.Services;

namespace BidMarket.Models
{
    public class Lot : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<LotImage>? Images { get; set; }
        public decimal CurrentPrice { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public AppUser? Seller { get; set; }
        public bool? Approved { get; set; }
        public AppUser? Winner { get; set; }
        public bool? Payed { get; set; }
        public bool? Confirmed { get; set; }
        public List<Bet>? Bets { get; set; }
        public List<CategoryLot>? Categories { get; set; }
    }
}
