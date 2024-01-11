using System.ComponentModel.DataAnnotations;

namespace BidMarket.Models
{
    public class Transaction : BaseEntity
    {
        public Guid UserId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public DateTime DateTime {get;set;}
        [Required]
        public string NonceMethod { get; set; }
    }
}
