using BidMarket.Models;

namespace BidMarket.Services
{
    public class CategoryLot:BaseEntity
    {
        public Guid CategoryId { get; set; }
        public Guid LotId { get; set; }
    }
}