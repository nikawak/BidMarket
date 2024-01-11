using BidMarket.Models;
using System.Drawing;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace BidMarket.Services
{
    public class TelegramBot
    {
        private TelegramBotClient _client;
        public TelegramBot()
        {
            _client = new TelegramBotClient("6882196641:AAGazd3y0-tW_n5O9SkuRYqQdxjWfvIbhtY");
        }

        public async Task SendWinNotification(long? chatId, string lotName)
        {
            if(chatId == null) return;
            await _client.SendTextMessageAsync(chatId, "Поздравляем, вы выйграли лот: " + lotName);
        }
        public async Task SendDisapproveAsync(long? chatId, string lotName, string reason)
        {
            if(chatId == null) return;
            await _client.SendTextMessageAsync(chatId, "Ваш лот '" + lotName + "' был отклонён по причине: " + reason);
        }

    }
}
