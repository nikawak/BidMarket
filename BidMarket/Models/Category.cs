using BidMarket.Services;
using System.ComponentModel.DataAnnotations;

namespace BidMarket.Models
{
    public class Category : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        public List<CategoryLot> Lots { get;set; }
    }
}
