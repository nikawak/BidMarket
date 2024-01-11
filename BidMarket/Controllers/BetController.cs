using AutoMapper;
using BidMarket.Models;
using BidMarket.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BidMarket.Controllers
{
    
    [Controller]

    [Authorize]
    public class BetController : Controller
    {
        private AppDbContext _context;
        private UserManager<AppUser> _userManager;
        private IMapper _mapper;

        public BetController(AppDbContext context, UserManager<AppUser> userManager, IMapper mapper) 
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult> GetAllBets(Guid lotId)
        {
            var bets = await _context.Bets.Include(x => x.Byer).Where(x => x.LotId == lotId).OrderByDescending(x=>x.Time).ToListAsync();
            return Json(bets);
        }
        [HttpPost]
        public async Task<ActionResult> MakeBet(Bet createBet)
        {
            if (createBet == null) return Ok();
            var byer = await _userManager.GetUserAsync(User);
            var lot = await _context.Lots.FindAsync(createBet.LotId);

            if (createBet.Size > lot.CurrentPrice && byer.VirtualMoney > createBet.Size )
            {
                ViewBag.BetSizeError = false;

                createBet.Byer = byer;
                lot.CurrentPrice = createBet.Size;
                createBet.Time = DateTime.Now.AddHours(3);

                createBet.Lot = lot;

                _context.Entry(createBet.Byer).State = EntityState.Unchanged;
                await _context.Bets.AddAsync(createBet);
                _context.Lots.Update(lot);
                await _context.SaveChangesAsync();
            }
            else ViewBag.BetSizeError = true;
            return Ok();
        }
    }
}
