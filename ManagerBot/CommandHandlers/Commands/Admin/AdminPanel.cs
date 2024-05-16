using System.Text.Encodings.Web;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotFramework;

namespace Template.Entities
{
    public partial class CommandHandler
    {
        public bool AcceptJoinRequests = true;

        /// <summary> Админ панель канала Black </summary>
        public async Task AdminPanel(UpdateInfo update, CallbackQuery? callback = null)
        {
            var startInlineKeyboard = new List<KeyboardButton[]>()
            {
                new KeyboardButton[] { "Установить расписание", "Актуальные записи" }
            };

            var replyMsg = $"Панель управления<a href=\"https://i.pinimg.com/564x/c9/4d/ab/c94dab0f12a851df1edd4efb15f0b8c9.jpg\">.</a>";
            await bot.BotClient.SendTextMessageAsync(update.Message.Chat.Id, replyMsg, parseMode: ParseMode.Html, replyMarkup: new ReplyKeyboardMarkup(startInlineKeyboard) { ResizeKeyboard = true });
        }
    }
}
