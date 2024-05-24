using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotFramework;
using Template.Additional;

namespace Template.Entities
{
    public partial class CommandHandler
    {
        public static PgProvider pg = new(Bot.DatabaseConnectionString);
        public static string sqlQuery = "";

        public CommandHandler(Bot nb) => bot = nb;

        public readonly Bot bot;


        /// <summary> Start command handler </summary>
        public async Task Start(UpdateInfo update)
        {
            var replyMsg = "👋 <b>Привет! Здесь вы можете записаться на занятие по роликам 🛼</b>\n\n";

            await bot.BotClient.SendTextMessageAsync(update.Message.Chat.Id, replyMsg, parseMode: ParseMode.Html, replyMarkup: new ReplyKeyboardMarkup(new List<KeyboardButton[]>()
            {
                new KeyboardButton[] { "Запись на занятие" },
                new KeyboardButton[] { "Мои записи" }
            })
            { ResizeKeyboard = true });
        }
    }
}
