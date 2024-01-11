using Microsoft.AspNetCore.Identity;

namespace BidMarket.Models
{
    public class AppUser : IdentityUser<Guid>
    {
        public string? Name { get; set; }
        public decimal? Money { get; set; }
        public decimal? VirtualMoney { get; set; }
        public long? ChatIdTG { get; set; }
        public bool? IsBlocked { get; set; }
        public decimal? ManagerMark { get; set; } = 5;
        public decimal? UserMark { get; set; } = 0;
        public ICollection<Review>? SendReviews { get; set; }
        public ICollection<Review>? RecieveReviews { get; set; }
    }
}
