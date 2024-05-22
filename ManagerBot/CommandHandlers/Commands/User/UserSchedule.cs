using ManagerBot.Data;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotFramework;
using Template.Additional;
using static Template.Data.Structures;

namespace Template.Entities
{
    public partial class CommandHandler
    {
        public async Task UserSchedule(UpdateInfo update, CallbackQuery? callback = null, int page = 1)
        {
            var signs = Database.GetSigns(page: page, is_active: true, user_id: update.Message.Chat.Id);
            if (signs.Any() == false)
            {
                if (callback != null)
                    await bot.BotClient.EditMessageTextAsync(update.Message.Chat.Id, callback.Message.MessageId, $"<b>У вас нет ни одной записи! ❌\n\n<b>Записаться</b> - /sign_up</b>", parseMode: ParseMode.Html);
                else
                    await bot.BotClient.SendTextMessageAsync(update.Message.Chat.Id, $"<b>У вас нет ни одной записи! ❌\n\n<b>Записаться</b> - /sign_up</b>", parseMode: ParseMode.Html);
                return;
            }

            var signsButtons = new List<InlineKeyboardButton[]>();
            for (int i = 0; i < signs.Count; i++)
            {
                signsButtons.Add(new InlineKeyboardButton[]
                {
                    new($"{signs[i].Date.ToString("M")} {DateTime.Parse(signs[i].Time.ToString()):t}") { CallbackData = $"Get_{signs[i].ID}" },
                });
            }

            signsButtons.Add(new InlineKeyboardButton[]
            {
                new("👈🏻") { CallbackData = $"Back" }, new("👉🏻") { CallbackData = $"Next" }
            });

            var inlineKeyboard = new InlineKeyboardMarkup(signsButtons);

            var replyMsg = $"<b>Ваши актуальные записи 👇🏻</b>\n\n";

            if (callback != null)
                if (page <= 0)
                {
                    page = 1;
                    try
                    {
                        await bot.BotClient.EditMessageTextAsync(update.Message.Chat.Id, callback.Message.MessageId, replyMsg, parseMode: ParseMode.Html, replyMarkup: inlineKeyboard);
                    }
                    catch (Exception)
                    {
                        await bot.BotClient.DeleteMessageAsync(update.Message.Chat.Id, callback.Message.MessageId);
                        await bot.BotClient.SendTextMessageAsync(update.Message.Chat.Id, replyMsg, parseMode: ParseMode.Html, replyMarkup: inlineKeyboard);
                    }
                }
                else
                {
                    try
                    {
                        await bot.BotClient.EditMessageTextAsync(update.Message.Chat.Id, callback.Message.MessageId, replyMsg, parseMode: ParseMode.Html, replyMarkup: inlineKeyboard);
                    }
                    catch (Exception)
                    {
                        await bot.BotClient.DeleteMessageAsync(update.Message.Chat.Id, callback.Message.MessageId);
                        await bot.BotClient.SendTextMessageAsync(update.Message.Chat.Id, replyMsg, parseMode: ParseMode.Html, replyMarkup: inlineKeyboard);
                    }
                }
            else
                await bot.BotClient.SendTextMessageAsync(update.Message.Chat.Id, replyMsg, parseMode: ParseMode.Html, replyMarkup: inlineKeyboard);

            callback = await bot.NewButtonClick(update);
            if (callback == null) return;

            if (callback.Data.Contains("Back"))
            {
                page--;

                await UserSchedule(update, callback, page);
            }

            if (callback.Data.Contains("Next"))
            {
                page++;
                await UserSchedule(update, callback, page);
            }

            if (callback.Data.Contains("Get"))
            {
                string signID = new Regex(@"_(.*)").Match(callback.Data).Groups[1].Value;
                await GetSign(update, signID, callback);
            }
        }


        private async Task GetSign(UpdateInfo update, string signID, CallbackQuery callback)
        {
            var sign = Database.GetSigns(signID: signID).First();

            var replyMsg = $"<b>Дата: <code>{sign.Date:D}</code></b>\n" +
                           $"<b>Время: <code>{DateTime.Parse(sign.Time.ToString()):t}</code></b>\n" +
                           $"<b>Длительность: <code>{(sign.TimeSpan == 1 ? "60 минут" : "90 минут")}</code></b>";

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[] { new InlineKeyboardButton("Отменить запись ❌") { CallbackData = signID } }
            });

            await bot.BotClient.EditMessageTextAsync(update.Message.Chat.Id, callback.Message.MessageId, replyMsg, parseMode: ParseMode.Html, replyMarkup: inlineKeyboard);

            callback = await bot.NewButtonClick(update);
            if (callback == null) return;

            inlineKeyboard = new InlineKeyboardMarkup(new InlineKeyboardButton[]
            {
                new("✅") { CallbackData = "1" },
                new("❌") { CallbackData = "0" },
            });

            replyMsg = $"<b>Вы уверены что хотите отменить запись на <code>{sign.Date.ToString("dd MMMM")} {DateTime.Parse(sign.Time.ToString()):t}?</code></b>";
            await bot.BotClient.EditMessageTextAsync(update.Message.Chat.Id, callback.Message.MessageId, replyMsg, parseMode: ParseMode.Html, replyMarkup: inlineKeyboard);

            callback = await bot.NewButtonClick(update);
            if (callback == null) return;

            if (callback.Data == "1")
            {
                sqlQuery = $"update signs set is_active = false where id = '{signID}'";
                pg.ExecuteSqlQueryAsEnumerable(sqlQuery);

                await Tools.NotifyAdmins(bot.BotClient, $"❌ <b>Пользователь @{sign.Username} отменил свою запись на <code>{sign.Date.ToString("dd MMMM")} {DateTime.Parse(sign.Time.ToString()):t}!</code></b>");

                await UserSchedule(update, callback);
            }

            else
            {
                await UserSchedule(update, callback);
            }
        }
    }
}
