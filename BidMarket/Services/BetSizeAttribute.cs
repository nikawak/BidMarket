using AutoMapper.Configuration;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Telegram.Bot.Types;

namespace BidMarket.Services
{
    public class BetSizeAttribute : ValidationAttribute
    {
        public BetSizeAttribute(Guid lotId)
            => LotId = lotId;

        public Guid LotId { get; }

        public string GetErrorMessage(decimal value) =>
            $"Ставка должна быть больше предыдущей ({value})";

        protected override ValidationResult? IsValid(
            object? value, System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            var _context = (AppDbContext)validationContext
                         .GetService(typeof(AppDbContext));

            var lot = _context.Lots.Include(x => x.Bets).FirstOrDefault(x=>x.Id == LotId);
            var maxSize = lot.Bets.Max(x => x.Size);

            if ((decimal)value < maxSize) return new ValidationResult(GetErrorMessage(maxSize));
            return ValidationResult.Success;
        }
    }
}
