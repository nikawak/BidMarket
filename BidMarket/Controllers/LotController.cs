using AutoMapper;
using BidMarket.Models;
using BidMarket.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace BidMarket.Controllers
{
    [Controller]
    [Authorize]
    public class LotController : Controller
    {
        private AppDbContext _context;
        private IMapper _mapper;
        private UserManager<AppUser> _userManager;
        private ICloudService _cloud;
        private IConfiguration _configuration;
        private TelegramBot _bot;
        private MailSender _sender;

        public LotController(AppDbContext context, IMapper mapper, MailSender sender,
            UserManager<AppUser> userManager, ICloudService cloud, IConfiguration configuration, TelegramBot bot)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _cloud = cloud;
            _configuration = configuration;
            _bot = bot;
            _sender = sender;
        }
        [HttpGet]
        public async Task<ActionResult> GetLots(string filter = "", int sort = 0)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            var lots = await _context.Lots
                .Include(x => x.Seller)
                .Include(x => x.Images)
                .Include(x => x.Categories)
                .Where(x => x.Approved.HasValue && x.Approved.Value)
                .ToListAsync();

            var resLots = new List<Lot>();
            foreach(var lot in lots)
            {
                if(lot.EndTime > DateTime.Now.AddHours(3) && lot.StartTime < DateTime.Now.AddHours(3))
                {
                    resLots.Add(lot);
                }
            }

            if(sort == 0)
            {
                resLots = resLots.OrderBy(x => x.EndTime).ToList();
            }
            else
            {
                resLots = resLots.OrderByDescending(x => x.StartTime).ToList();
            }
            if(Guid.TryParse(filter, out var filterId)){
                resLots = resLots.Where(x => x.Categories.Select(x=>x.CategoryId).Contains(filterId)).ToList();
            }


            return View(resLots);
        }

        [HttpGet]
        public async Task<ActionResult> GetSelfLots()
        {

            ViewBag.Categories = await _context.Categories.ToListAsync();
            var user = await _userManager.GetUserAsync(User);
            var lots = _context.Lots
                .Include(x =>x.Seller)
                .Where(l => l.Seller.Id == user.Id)
                .Where(x => !x.Approved.HasValue || x.Approved.Value)
                .Include(x => x.Images)
                .ToList();

            var resLots = new List<Lot>();
            foreach (var lot in lots)
            {
                if (lot.EndTime > DateTime.Now.AddHours(3))
                {
                    resLots.Add(lot);
                }
            }
            resLots = resLots.OrderBy(x => x.EndTime).ToList();

            return View("GetLots", resLots);
        }
        [HttpGet]
        public async Task<ActionResult> GetLotsHistorySell()
		{
            var user = await _userManager.GetUserAsync(User);
            var lots = await _context.Lots
                .Include(x=>x.Seller)
                .Include(x=>x.Categories)
                .Include(x=>x.Winner)
                .Where(l => l.Seller.Id == user.Id)
                .Include(x => x.Images)
                .ToListAsync();

            var resLots = new List<Lot>();
            foreach (var lot in lots)
            {
                if (lot.EndTime < DateTime.Now.AddHours(3))
                {
                    resLots.Add(lot);
                }
            }
            resLots = resLots.OrderByDescending(x => x.EndTime).ToList();

            return View(resLots);
		}
        [HttpGet]
        public async Task<ActionResult> GetLotsHistoryBuy()
        {
            var user = await _userManager.GetUserAsync(User);
            var lots = await _context.Lots
                .Include(x=>x.Seller)
                .Where(l => l.Winner.Id == user.Id)
                .Where(x => x.Approved.HasValue && x.Approved.Value)
                .Include(x => x.Images)
                .ToListAsync();

            lots = lots.OrderByDescending(x => x.EndTime).ToList();

            return View(lots);
        }
        [HttpGet]
        public async Task<ActionResult> GetLot(Guid id)
        {
            var lot = await _context.Lots.Include(x=>x.Images).Include(x=>x.Winner).Include(x=>x.Bets).Include(x=>x.Seller).FirstOrDefaultAsync(x=>x.Id == id);
            dynamic res = lot == null ? BadRequest() : lot;
            return View(res);
        }
        [HttpGet]
        public async Task<ActionResult> CreateLot()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync(); 
            return View(new LotDTO() { CurrentPrice = 1, StartTime = DateTime.Now.AddHours(3), EndTime = DateTime.Now.AddHours(3) });
        }
        [HttpPost]
        public async Task<ActionResult> CreateLot(LotDTO createLot, Guid[] categories)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            if (!ModelState.IsValid) return View(createLot);
            if (createLot.StartTime > createLot.EndTime)
            {
                ModelState.AddModelError("t","Некорректное время");
                return View(createLot);
            }
            if (createLot.EndTime < DateTime.Now.AddHours(3))
            {
                ModelState.AddModelError("t4", "Некорректное время. Аукцион должен длиться дольше 3 минут");
                return View(createLot);
            }
            if (createLot.StartTime.AddMinutes(3) > createLot.EndTime)
            {
                ModelState.AddModelError("t", "Некорректное время. Аукцион должен длиться дольше 3 минут");
                return View(createLot);
            }
            if (createLot.EndTime.AddMinutes(2) < DateTime.Now.AddHours(3))
            {
                ModelState.AddModelError("t2", "Некорректное время. Аукцион должен длиться дольше 3 минут");
                return View(createLot);
            }
            if (createLot.EndTime > DateTime.Now.AddHours(3).AddMonths(1).AddDays(1))
            {
                ModelState.AddModelError("t3", "Некорректное время. Аукцион не должен длиться дольше месяца");
                return View(createLot);
            }
            var lot = _mapper.Map<Lot>(createLot);
            var listImages = new List<LotImage>();
            if (createLot.ImagesFiles != null)
            {
                foreach (var img in createLot.ImagesFiles)
                {
                    var lotImage = new LotImage();
                    var imageName = (DateTime.Now.AddHours(3).ToString() + img.FileName).Replace(".", "-");
                    var split = img.FileName.Split(".");
                    lotImage.MimeType = "image/" + split[split.Length - 1];

                    lotImage.Name = await MegaImageWrite(img, imageName);
                    listImages.Add(lotImage);
                }

                lot.Images = listImages;
                await _context.LotImages.AddRangeAsync(listImages);
            }
            
            var cats = new List<CategoryLot>();
            foreach(var cat in categories)
            {
                cats.Add(new CategoryLot() { CategoryId = cat , LotId = lot.Id}) ;
            }
            await _context.CategoryLot.AddRangeAsync(cats);
            lot.Categories = cats;
            lot.Seller = await _userManager.GetUserAsync(User);
            _context.Entry(lot.Seller).State = EntityState.Unchanged;

            lot.Approved = null;
            await _context.Lots.AddAsync(lot);
            await _context.SaveChangesAsync();
            return RedirectToAction("GetSelfLots");
        }
        public async Task<ActionResult> EditLot(Guid id)
        {
            var lot = await _context.Lots.Include(x=>x.Categories).FirstOrDefaultAsync(x=>x.Id == id);
            ViewBag.Categories = lot.Categories.Select(x => _context.Categories.Find(x.CategoryId).Name);
            var editLot = new LotEdit() { Id = id, StartTime = lot.StartTime, EndTime = lot.EndTime, CurrentPrice = lot.CurrentPrice, Description = lot.Description, Name = lot.Name };

            return View(editLot);
        }
        [HttpPost]
        public async Task<ActionResult> EditLot(LotEdit editLot)
        {
            if (!ModelState.IsValid) return View(editLot);
            if(editLot.EndTime < DateTime.Now.AddHours(3))
            {
                ModelState.AddModelError("t4", "Аукцион на этот лот уже закончился. Вы не можете его редактировать");
                return View(editLot);
            }
            if (editLot.StartTime > editLot.EndTime)
            {
                ModelState.AddModelError("t", "Некорректное время");
                return View(editLot);
            }
            if (editLot.EndTime.AddMinutes(2) < DateTime.Now.AddHours(3))
            {
                ModelState.AddModelError("t2", "Некорректное время. Аукцион должен длиться дольше 3 минут");
                return View(editLot);
            }
            if (editLot.StartTime.AddMinutes(3) > editLot.EndTime)
            {
                ModelState.AddModelError("t0", "Некорректное время. Аукцион должен длиться дольше 3 минут");
                return View(editLot);
            }
            if (editLot.EndTime > DateTime.Now.AddHours(3).AddMonths(1).AddDays(1))
            {
                ModelState.AddModelError("t3", "Некорректное время. Аукцион не должен длиться дольше месяца");
                return View(editLot);
            }
            var lot = await _context.Lots.FirstOrDefaultAsync(x => x.Id == editLot.Id);
            lot.StartTime = editLot.StartTime;
            lot.EndTime = editLot.EndTime;
            lot.Description = editLot.Description;
            lot.Name = editLot.Name;
            lot.CurrentPrice = editLot.CurrentPrice;
           

            lot.Approved = null;

            _context.Lots.Update(lot);
            await _context.SaveChangesAsync();
            return RedirectToAction("GetSelfLots", "Lot");
        }
        [HttpPost]
        public async Task<ActionResult> DeleteLot(Guid id)
		{
            var lot = await _context.Lots.Include(x=>x.Bets).Include(x=>x.Categories).Include(x=>x.Images).FirstOrDefaultAsync(x=>x.Id == id);

            _context.Bets.RemoveRange(lot.Bets);
            _context.CategoryLot.RemoveRange(lot.Categories);
            _context.LotImages.RemoveRange(lot.Images);
			_context.Lots.Remove(lot);
            await _context.SaveChangesAsync();
			return RedirectToAction("GetSelfLots", "Lot");
		}
        [HttpPost]
        public async Task<ActionResult> LoadImage(IFormFile[] images, Guid lotId)
        {
            foreach (var img in images)
            {
                var fileLink = await _cloud.UploadImageAsync(img, new Random().Next().ToString() + DateTime.Now.AddHours(3));
                var lot = await _context.Lots.Include(l => l.Images).FirstOrDefaultAsync(l => l.Id == lotId);
                lot.Images.Add(new LotImage() { Name = fileLink });
                _context.Lots.Update(lot);
                await _context.SaveChangesAsync();
            }
            return View();
        }
        [HttpGet]

        [Authorize(Roles = "Manager")]
        public async Task<ActionResult> ManagerLot()
        {
            var lot = await _context.Lots
                .OrderBy(x => x.StartTime)
                .Include(x => x.Images)
                .Include(x => x.Categories)
                .Include(x => x.Seller)
                .Where(x => !x.Approved.HasValue && x.EndTime > DateTime.Now.AddHours(3))
                .FirstOrDefaultAsync();

            if(lot != null)
            {
                ViewBag.Categories = lot?.Categories?.Select(x => _context.Categories.Find(x.CategoryId).Name);
            }
            

            return View(lot);
        }
        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult> DisapproveLot(Guid lotId, string reason)
        {
            var lot = await _context.Lots.Include(x=>x.Seller).FirstOrDefaultAsync(x=>x.Id == lotId);
            if(lot == null) return RedirectToAction("ManagerLot");
            lot.Approved = false;
            if (lot.Seller != null && lot.Seller.EmailConfirmed)
            {
                await _sender.SendDisapproveAsync(lot.Seller.Email, lot.Name, reason);
            }
            if (lot.Seller != null && lot.Seller.ChatIdTG != null)
            {
                await _bot.SendDisapproveAsync(lot.Seller.ChatIdTG, lot.Name, reason);
            }

            _context.Lots.Update(lot);
            _context.SaveChanges();
            return RedirectToAction("ManagerLot");
        }
        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult> ApproveLot(Lot lot, string reason)
        {
            var editLot = await _context.Lots
                .OrderBy(x => x.StartTime)
                .Include(x => x.Images)
                .Include(x => x.Categories)
                .Include(x => x.Seller)
                .Where(x => !x.Approved.HasValue && x.EndTime > DateTime.Now.AddHours(3))
                .FirstOrDefaultAsync();

            if (editLot != null)
            {
                ViewBag.Categories = lot?.Categories?.Select(x => _context.Categories.Find(x.CategoryId).Name);
            }
            if (!ModelState.IsValid) return View(lot);
            if (lot.StartTime > lot.EndTime)
            {
                ModelState.AddModelError("t", "Некорректное время");
                return View("ManagerLot",editLot);
            }
            if (lot.EndTime.AddMinutes(3) < DateTime.Now.AddHours(3))
            {
                ModelState.AddModelError("t2", "Некорректное время. Аукцион должен длиться дольше 3 минут");
                return View("ManagerLot", editLot);
            }
            if (lot.StartTime.AddMinutes(3) > lot.EndTime)
            {
                ModelState.AddModelError("t0", "Некорректное время. Аукцион должен длиться дольше 3 минут");
                return View("ManagerLot", editLot);
            }
            if (lot.EndTime > DateTime.Now.AddHours(3).AddMonths(1).AddDays(1))
            {
                ModelState.AddModelError("t", "Некорректное время. Аукцион не должен длиться дольше месяца");
                return View("ManagerLot", editLot);
            }

            var lotModel = await _context.Lots.FindAsync(lot.Id);
            lotModel.Approved = true;
            lotModel.CurrentPrice = lot.CurrentPrice;
            lotModel.Name = lot.Name;
            lotModel.Description = lot.Description;
            lotModel.StartTime = lot.StartTime;
            lotModel.EndTime = lot.EndTime;

            _context.Lots.Update(lotModel);
            _context.SaveChanges();
            return RedirectToAction("ManagerLot");
        }
        [HttpPost]
        public async Task<ActionResult> WinLot(Guid lotId)
        {
            var lot = await _context.Lots.AsNoTrackingWithIdentityResolution().Include(x=>x.Winner).Include(x=>x.Bets).ThenInclude(x=>x.Byer).AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(x=>x.Id == lotId);
            if (lot == null) return Ok();
            if (lot.EndTime > DateTime.Now.AddHours(3)) return Ok();
            if(lot.Winner != null)
            {
               
                return Ok(lot.Winner);
            }
            var bets = lot.Bets.OrderByDescending(x => x.Size);
            if (bets.Count() == 0) return Ok();
            var winner = bets.FirstOrDefault().Byer;

            _context.Entry(winner).State = EntityState.Detached;

            if (winner == null || lot == null) return Ok();
            
            lot.Winner = bets.FirstOrDefault().Byer;

            _context.Entry(lot.Winner).State = EntityState.Detached;

            _context.Lots.Update(lot);
            await _context.SaveChangesAsync();

            if (lot.Winner.ChatIdTG != null)
            {
                await _bot.SendWinNotification(lot.Winner.ChatIdTG, lot.Name);
            }
            if (lot.Winner.EmailConfirmed)
            {
                await _sender.SendWinAsync(lot.Winner.Email, lot.Name);
            }

            return Ok(lot.Winner);
            
        }
        [HttpPost]
        public async Task<ActionResult> ConfirmLot(Guid id)
        {
            var lot = await _context.Lots.FirstOrDefaultAsync(x=>x.Id == id);
            lot.Confirmed = true;
            _context.Lots.Update(lot);
            await _context.SaveChangesAsync();
            return RedirectToAction("GetLot", "Lot", new { id = id });
        }
        public async Task<ActionResult> TimeToEnd(Guid lotId)
        {
            var lot = _context.Lots.Find(lotId);
            if (lot == null) return Ok();
            var timeToEnd = lot.EndTime - DateTime.Now.AddHours(3);
            timeToEnd = timeToEnd < TimeSpan.Zero ? TimeSpan.Zero : timeToEnd;
            timeToEnd = new TimeSpan((int)timeToEnd.TotalHours, timeToEnd.Minutes, timeToEnd.Seconds);
            return Json(timeToEnd);
        }
        public async Task<ActionResult> SearchLots(string searchString)
        {
            var lots = await _context.Lots
                 .Include(x => x.Seller)
                .Include(x => x.Images)
                .Include(x => x.Categories)
                .Where(x => x.Approved.HasValue && x.Approved.Value)
                .Where(x => x.Name.Contains(searchString)
                || x.Description.Contains(searchString)).ToListAsync();
            

            var resLots = new List<Lot>();
            foreach (var lot in lots)
            {
                if (lot.EndTime > DateTime.Now.AddHours(3) && lot.StartTime < DateTime.Now.AddHours(3))
                {
                    resLots.Add(lot);
                }
            }

                resLots = resLots.OrderByDescending(x => x.EndTime).ToList();


            return View(resLots);
        }



        [AcceptVerbs]
        public async Task<FileResult> MegaImageRead(Guid id, int number)
        {
            var lot = await _context.Lots.Include(x=>x.Images).FirstOrDefaultAsync(x=>x.Id == id);
            if (lot==null || !lot.Images.Any() || string.IsNullOrEmpty(lot?.Images[number].Name))
            {
                return File("~/img/tusk-4.jpg", "image/jpg");
            }

            var bytes = await _cloud.DownloadImageAsync(lot?.Images[number].Name);

            return File(bytes, lot?.Images[number].MimeType);
        }
        [AcceptVerbs]
        public async Task<string> MegaImageWrite(IFormFile image, string imageName)
        {
            return await _cloud.UploadImageAsync(image, imageName);
        }
    }
}
