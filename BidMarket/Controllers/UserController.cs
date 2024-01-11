using AutoMapper;
using BidMarket.Models;
using BidMarket.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace BidMarket.Controllers
{
    [Controller]

    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly AppDbContext _context;

        public UserController(MailSender sender, SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager, IMapper mapper, ILogger<AccountController> logger, AppDbContext context)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(users);
        }
        [HttpGet]
        public async Task<ActionResult> GetUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            var userRev = _context.Reviews.Include(x => x.Reciever).Include(x => x.Sender).Where(x => x.Reciever.Id == id);
            if (userRev != null && userRev.Count() > 0)
                user.RecieveReviews = userRev.ToList();

            var lots = await _context.Lots
                .Include(x => x.Seller)
                .Include(x => x.Images)
                .Include(x => x.Categories)
                .Where(x => x.Approved.HasValue && x.Approved.Value)
                .ToListAsync();

            var resLots = new List<Lot>();
            foreach (var lot in lots)
            {
                if (lot.EndTime > DateTime.Now.AddHours(3) && lot.StartTime < DateTime.Now.AddHours(3))
                {
                    resLots.Add(lot);
                }
            }

            var curId = (await _userManager.GetUserAsync(User)).Id;
            if (curId != user.Id)
            {
                var first = await _context.Lots.Include(x => x.Seller).Include(x => x.Winner)
                    .AnyAsync(x => x.Seller.Id == user.Id && x.Winner.Id == curId);
                var second = await _context.Reviews.Include(x => x.Sender).Include(x => x.Reciever)
                    .AnyAsync(x => x.Reciever.Id == user.Id && x.Sender.Id == curId);
                ViewBag.CanWriteReview = first && !second;
            }
            else ViewBag.CanWriteReview = false;

            return View((user, resLots));
        }
        [HttpGet]
        public async Task<ActionResult> GetSelfUser()
        {
            var user = await _userManager.GetUserAsync(User);
            var userRev = _context.Reviews.Include(x=>x.Reciever).Include(x=>x.Sender).Where(x => x.Reciever.Id == user.Id);
            user.RecieveReviews = userRev.ToList();

            return View(user);
        }
        [HttpPut]
        public async Task<ActionResult> ChangeRoles(Guid id, string[] roles)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return BadRequest();

            var prevRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, prevRoles);
            await _userManager.AddToRolesAsync(user, roles);
            return Ok(user);
        }
        [HttpPut]
        public async Task<ActionResult> BlockUser(Guid id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return BadRequest();
            user.IsBlocked = true;
            await _userManager.UpdateAsync(user);

            return Ok(user);
        }
        [HttpDelete]
        public async Task<ActionResult> DeleteUser(Guid id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return BadRequest();
            await _userManager.DeleteAsync(user);

            return Ok(user);
        }

    }
}

