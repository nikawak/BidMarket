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
    public class ReviewController : Controller
    {
        private AppDbContext _context;
        private IMapper _mapper;
        private UserManager<AppUser> _userManager;
        public ReviewController(AppDbContext context, IMapper mapper, UserManager<AppUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }
        [HttpPost]
        public async Task<ActionResult> WriteReview(Review review)
        {
            if(review.Comment == null || review.Comment == "")
            {
                ModelState.AddModelError("31", "Напишите комментарий");
                return View(review);
            }
            review.Sender = await _userManager.GetUserAsync(User);
            review.Reciever = await _userManager.FindByIdAsync(review.RecieverId.ToString());
            _context.Entry(review.Sender).State = EntityState.Unchanged;
            _context.Entry(review.Reciever).State = EntityState.Unchanged;
            review.Id = Guid.NewGuid();

            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
            return RedirectToAction("GetUser", "User", new {id = review.RecieverId});
        }
        [HttpGet]
        public async Task<ActionResult> WriteReview(Guid id)
        {
            var review = new Review() { RecieverId = id };
            return View(review);
        }

        [HttpGet]
        public async Task<ActionResult> GetUserReviews(Guid userId)
        {
            var reviews = await _context.Reviews.Include(x=>x.Reciever).Where(r=>r.Reciever.Id == userId).ToListAsync();
            return Ok(reviews);
        }
    }
}
