using Microsoft.EntityFrameworkCore.Metadata;

namespace BidMarket.Models
{
    public class LotImage : BaseEntity
    {
        public string Name { get; set; }
        public string? MimeType { get; set; }
    }
}
