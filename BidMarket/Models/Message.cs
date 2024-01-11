using System.ComponentModel.DataAnnotations.Schema;

namespace BidMarket.Models
{
    public class Message : BaseEntity
    {
        public AppUser? Sender { get; set; }
        public AppUser? Reciever { get; set; }
        [NotMapped]
        public Guid? RecieverId { get; set; }
        public string? Text { get; set; }
        public DateTime? DateTime { get; set; }
    }
}
