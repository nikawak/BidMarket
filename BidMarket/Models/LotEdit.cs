using System.ComponentModel.DataAnnotations;

namespace BidMarket.Models
{
    public class LotEdit
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Нельзя оставлять имя пустым")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Нельзя оставлять описание пустым")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Нельзя оставлять время начала пустым")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Нельзя оставлять время конца пустым")]
        public DateTime EndTime { get; set; }
        [Required(ErrorMessage = "Нельзя оставлять начальную цену пустой")]
        public decimal CurrentPrice { get; set; }
    }
}
