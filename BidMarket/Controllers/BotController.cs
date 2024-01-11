using BidMarket.Models;
using BidMarket.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace BidMarket.Controllers
{
    [Controller]

    [Authorize]
    public class BotController : Controller
    {
        private UserManager<AppUser> _userManager;
        private TelegramBotClient _client;
        private long _chatId;
        public BotController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _client = new TelegramBotClient("6882196641:AAGazd3y0-tW_n5O9SkuRYqQdxjWfvIbhtY");
            _client.StartReceiving(UpdateHandler, ErrorHandler);
        }
        private async Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            await Task.CompletedTask;
        }
        private async Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken token)
        {
            if (update.Message == null || update.Message.Text == null) return;
            var mes = update.Message.Text.ToLower();
            if (mes.Equals("/start") || mes.Equals("/start start"))
            {
                var id = update.Message.Chat.Id;
                await client.SendTextMessageAsync(id, $"Для подписки на рассылку введите этот код {id} на сайте 'Уведомления => ввести код'");
            }
        }
        [HttpGet]
        public async Task<ActionResult> OpenBot()
        {
            return Redirect("https://t.me/bid_app_bot?start=start");
        }
        [HttpGet]
        public async Task<ActionResult> InsertChatId()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> InsertChatId(string chatId)
        {
            chatId = chatId.Trim();
            try
            {
                await _client.SendTextMessageAsync(chatId, $"Вы успешно подписались на рассылку уведомлеий при победе в аукционе");
                var user = await _userManager.GetUserAsync(User);
                user.ChatIdTG = Convert.ToInt64(chatId);
                await _userManager.UpdateAsync(user);

                ViewBag.Result = "Вы успешно подписались";
                ViewBag.Success = true;

                return View();
            }
            catch
            {
                ViewBag.Result = "Неправильный код";
                ViewBag.Success = false;

                return View();
            }

        }
    }
}
