using System.ComponentModel.DataAnnotations;

namespace BidMarket.Models
{
    public class LotDTO : BaseEntity
    {
        [Required(ErrorMessage = "Нельзя оставлять имя пустым")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Нельзя оставлять описание пустым")]
        public string? Description { get; set; }
        public List<IFormFile>? ImagesFiles { get; set; }

        [Required(ErrorMessage = "Нельзя начальную цену пустой")]
        [Range(1, 100000, ErrorMessage = "Некорректная цена")]
        public decimal CurrentPrice { get; set; }

        [Required(ErrorMessage = "Нельзя оставлять время начала пустым")]
        
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Нельзя оставлять время конца пустым")]
        public DateTime EndTime { get; set; }
        public AppUser? Seller { get; set; }
        public bool Approved { get; set; }
        public AppUser? Winner { get; set; }
        public List<Bet>? Bets { get; set; }
    }
}
