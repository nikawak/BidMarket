using BidMarket.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BidMarket.Models
{
    public class Bet : BaseEntity
    {
        public Lot Lot { get; set; }
        public Guid LotId { get; set; }
        public AppUser? Byer { get; set; }
        public decimal Size { get; set; }
        public DateTime Time { get; set; }
    }
}
