using BidMarket.Models;
using BidMarket.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Web;

namespace BidMarket.Controllers
{

    [Authorize]
    public class ChatController : Controller
    {
		private UserManager<AppUser> _userManager;
        private AppDbContext _context;

        public ChatController(UserManager<AppUser> userManager, AppDbContext context)
		{
			_userManager = userManager;
            _context = context;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var curUser = await _userManager.GetUserAsync(User);
            var users1 = await _context.Messages
                .Include(x => x.Sender).Include(x => x.Reciever)
                .Where(x=>x.Sender.Id == curUser.Id)
                .ToListAsync();

            var users2 = await _context.Messages
                .Include(x => x.Sender).Include(x => x.Reciever)
                .Where(x => x.Reciever.Id == curUser.Id)
                .ToListAsync();

            var messages = users1.Concat(users2).ToHashSet().OrderByDescending(x=>x.DateTime).ToArray();

            var distinctMes = new List<List<Message>>();
            var distinctUs = new List<Guid>();

            foreach (var mes in messages)
            {
                var senId = mes.Sender.Id;
                var recId = mes.Reciever.Id;

                if (senId == curUser.Id && !distinctUs.Contains(recId))
                {
                    distinctUs.Add(recId);
                }
                else if(senId != curUser.Id && !distinctUs.Contains(senId))
                {
                    distinctUs.Add(senId);
                }
            }
            foreach(var usId in distinctUs)
            {
                var m = messages.Where(x => x.Sender.Id == usId || x.Reciever.Id == usId).ToList();
                distinctMes.Add(m);
            }
            var latestMessages = new List<Message>();
            foreach(var mes in distinctMes)
            {
                var latestMes = mes.OrderByDescending(x => x.DateTime).FirstOrDefault();
                latestMessages.Add(latestMes);
            }

            return View(latestMessages);
		}
        [HttpPost]
        public async Task<IActionResult> SendMessage(Message message)
        {
            var curUser = await _userManager.GetUserAsync(User);
            message.Sender = curUser;
            message.DateTime = DateTime.Now.AddHours(3);
            message.Reciever = await _userManager.FindByIdAsync(message.RecieverId.ToString());

            _context.Entry(message.Sender).State = EntityState.Unchanged;
            _context.Entry(message.Reciever).State = EntityState.Unchanged;
            await _context.AddAsync(message);
            await _context.SaveChangesAsync();

            return RedirectToAction("Chat", message.Reciever.Id);
        }
        [HttpGet]
        public async Task<IActionResult> Chat(Guid id)
        {
            var curUser = await _userManager.GetUserAsync(User);
			var partnerUser = await _userManager.FindByIdAsync(id.ToString());

            var myMessages = await _context.Messages
                .Include(x => x.Sender).Include(x => x.Reciever)
                .Where(x => x.Sender.Id == curUser.Id && x.Reciever.Id == id)
                .ToListAsync();
            var hisMessages = await _context.Messages
               .Include(x => x.Sender).Include(x => x.Reciever)
               .Where(x => x.Sender.Id == id && x.Reciever.Id == curUser.Id)
               .ToListAsync();
            ViewBag.user = partnerUser;

            return View(myMessages.Concat(hisMessages).OrderByDescending(x=>x.DateTime).ToArray());
        }
    }
}
