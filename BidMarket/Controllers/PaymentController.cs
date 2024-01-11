
using BidMarket.Models;
using BidMarket.Services;
using CourseProject.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BidMarket.Controllers
{
    public class PaymentController : Controller
    {
        private PaymentService _paymentService;
        private UserManager<AppUser> _userManager;
        private AppDbContext _context;

        public PaymentController(PaymentService paymentService, UserManager<AppUser> userManager, AppDbContext context)
        {
            _paymentService = paymentService;
            _userManager = userManager;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index(bool callbackToLot = false, Guid? lotId = null)
        {
            var token = await _paymentService.GenerateToken();
            ViewBag.Token = token;
            ViewBag.CallbackToLot = callbackToLot;
            ViewBag.LotId = lotId;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var transactions = await _context.Transactions.ToListAsync();
            return View(transactions);
        }
        
        public async Task<ActionResult> RefundRequest(Guid tranId)
        {
            var tran = await _context.Transactions.FindAsync(tranId);
            var user = await _userManager.GetUserAsync(User);
            if (user.VirtualMoney > tran.Amount && tran.DateTime.AddDays(7) > DateTime.Now.AddHours(3))
            {
                user.VirtualMoney -= tran.Amount;
            }
            return RedirectToAction("GetAll");
        }
        [HttpPost]
        public async Task<ActionResult> Pay(Guid id)
        {
            var lot = await _context.Lots.Include(x=>x.Bets).FirstOrDefaultAsync(l => l.Id == id);
            var betSize = lot.Bets.Select(x => x.Size).OrderByDescending(x => x).FirstOrDefault();

            var user = await _userManager.GetUserAsync(User);
            if (user.VirtualMoney >= betSize)
            {
                user.VirtualMoney -= betSize;
                await _userManager.UpdateAsync(user);

                lot.Payed = true;
                _context.Lots.Update(lot);
                await _context.SaveChangesAsync();
                return RedirectToAction("GetLot","Lot", new {id = id});
            }
            else
            {
                return RedirectToAction("Index", new { callbackToLot = true, lotId = id });
            }
            
        }
        [HttpPost]
        public async Task<IActionResult> Index(Models.Transaction transactionDTO)
        {

            var request = _paymentService.CreateTransaction(transactionDTO.Amount, transactionDTO.NonceMethod);
            var transaction = _paymentService.StartTransaction(request);

            ViewBag.IsSuccess = transaction.IsSuccess();
            if (transaction.IsSuccess())
            {
                ViewBag.Result = "Транзакция одобрена";
                var user = await _userManager.GetUserAsync(User);
                user.VirtualMoney = user.VirtualMoney == null ? 0 : user.VirtualMoney;
                user.VirtualMoney += transactionDTO.Amount;
                await _userManager.UpdateAsync(user);
                transactionDTO.UserId = user.Id;
                await _context.Transactions.AddAsync(transactionDTO);
            }
            else
            {
                ViewBag.Result = "Транзакция не одобрена. Вероятно вы не выбрали карту";
            }
            ViewBag.CallBackToLot = false;
            return View();
        }
    }
}
