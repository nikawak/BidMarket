using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BidMarket.Models
{
    public class Review : BaseEntity
    {
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public AppUser? Sender { get; set; }
        [NotMapped]
        public Guid? SenderId { get; set; }

        [NotMapped]
        public Guid? RecieverId { get; set; }
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public AppUser? Reciever { get; set; }
        public CommentStars CommentStars { get; set; }
        public string Comment { get; set; }
    }
}
