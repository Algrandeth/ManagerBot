using System.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotFramework;
using Template.Additional;

namespace Template.Entities
{
    public partial class CommandHandler
    {
        /// <summary> Bot users count </summary>
        public async Task UserStatistics(UpdateInfo update)
        {
            var totalUsersCount = pg.ExecuteSqlQueryAsEnumerable("select count(user_id) as count from users").First().Field<long>("count");
            var actualSignsCount = pg.ExecuteSqlQueryAsEnumerable("select count(id) as count from signs where is_active = true").First().Field<long>("count");
            var notActualSignsCount = pg.ExecuteSqlQueryAsEnumerable("select count(id) as count from signs where is_active = false").First().Field<long>("count");
            var totalSignCount = pg.ExecuteSqlQueryAsEnumerable("select count(id) as count from signs").First().Field<long>("count");


            var replyMsg = $"<b>Пользователей в боте:</b> <code>{totalUsersCount}</code>\n\n" +
                           $"<b>Актуальных записей:</b> <code>{actualSignsCount}</code>\n" +
                           $"<b>Закрытых записей:</b> <code>{notActualSignsCount}</code>\n" +
                           $"<b>Всего записей:</b> <code>{totalSignCount}</code>\n";

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                    new InlineKeyboardButton[] { "Удалить мертвых юзеров" },
            });

            await bot.BotClient.SendTextMessageAsync(update.Message.Chat.Id, replyMsg, parseMode: ParseMode.Html, replyMarkup: inlineKeyboard);

            var nextButton = await bot.NewButtonClick(update);
            if (nextButton == null) return;
            if (nextButton.Data == "Удалить мертвых юзеров")
            {
                await Tools.DeleteDeadUsers(bot.BotClient, update);
            }
        }
    }
}
