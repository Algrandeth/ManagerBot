using ManagerBot.Data;
using System.Data;
using System.Text.RegularExpressions;
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
        public async Task AdminSchedule(UpdateInfo update, CallbackQuery? callback = null, int page = 1)
        {
            if (!pg.ExecuteSqlQueryAsEnumerable($"select 1 from signs where is_active = true").Any())
            {
                if (callback != null)
                    await bot.BotClient.EditMessageTextAsync(update.Message.Chat.Id, callback.Message.MessageId, $"<b>Записи отсутствуют! ❌</b>", parseMode: ParseMode.Html);
                else
                    await bot.BotClient.SendTextMessageAsync(update.Message.Chat.Id, $"<b>Записи отсутствуют! ❌</b>", parseMode: ParseMode.Html);
                return;
            }

            var signs = Database.GetSigns(page: page, is_active: true);
            var signsButtons = new List<InlineKeyboardButton[]>();

            if (signs.Any() == false)
            {
                page--;
                await AdminSchedule(update, callback, page);
                return;
            }

            for (int i = 0; i < signs.Count; i++)
            {
                signsButtons.Add(new InlineKeyboardButton[]
                {
                    new($"{signs[i].Date.ToString("M")} {DateTime.Parse(signs[i].Time.ToString()):t}") { CallbackData = $"Get_{signs[i].ID}" },
                    new('@' + signs[i].Username) { Url = $"https://t.me/{signs[i].Username}" },
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

                await AdminSchedule(update, callback, page);
            }

            if (callback.Data.Contains("Next"))
            {
                page++;
                await AdminSchedule(update, callback, page);
            }

            if (callback.Data.Contains("Get"))
            {
                string signID = new Regex(@"_(.*)").Match(callback.Data).Groups[1].Value;
                await GetSign_Admin(update, signID, callback);
            }
        }


        private async Task GetSign_Admin(UpdateInfo update, string signID, CallbackQuery callback)
        {
            var sign = Database.GetSigns(signID: signID).First();

            var replyMsg = $"<b>Пользователь: @{sign.Username}</b>\n" +
                           $"<b>Дата: <code>{sign.Date:D}</code></b>\n" +
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

            replyMsg = $"<b>Вы уверены что хотите отменить запись на <code>{sign.Date.ToString("dd MMMM")} {DateTime.Parse(sign.Time.ToString()):t}?</code></b>\n\n<i>Пользователю будет отправлено уведомление.</i>";
            await bot.BotClient.EditMessageTextAsync(update.Message.Chat.Id, callback.Message.MessageId, replyMsg, parseMode: ParseMode.Html, replyMarkup: inlineKeyboard);

            callback = await bot.NewButtonClick(update);
            if (callback == null) return;

            if (callback.Data == "1")
            {
                sqlQuery = $"update signs set is_active = false where id = '{signID}'";
                pg.ExecuteSqlQueryAsEnumerable(sqlQuery);

                try
                {
                    await bot.BotClient.SendTextMessageAsync(sign.UserID, $"<b>❌ Ваша запись на <code>{sign.Date.ToString("dd MMMM")} {DateTime.Parse(sign.Time.ToString()):t}</code> была отменена администратором!</b>", parseMode: ParseMode.Html);
                }
                catch (Exception) { }
                    await AdminSchedule(update, callback);
            }

            else
            {
                await AdminSchedule(update, callback);
            }
        }
    }
}
