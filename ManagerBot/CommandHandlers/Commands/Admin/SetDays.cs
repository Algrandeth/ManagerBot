using ManagerBot.Data;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotFramework;
using Template.Monitoring;
using static Template.Data.Structures;

namespace Template.Entities
{
    public partial class CommandHandler
    {
        private async Task AddDays()
        {
            pg.ExecuteSqlQueryAsEnumerable("delete from days");

            for (int year = DateTime.Now.Year; year <= DateTime.Now.Year + 1; year++)
            {
                for (int month = 1; month <= 12; month++)
                {
                    var daysInMonth = DateTime.DaysInMonth(year, month);

                    for (int day = 1; day < daysInMonth; day++)
                    {
                        try
                        {
                            var dt = new DateTime(year, month, day);
                            sqlQuery = $@"insert into days
                                      values ('{dt.Year}-{dt.Month}-{dt.Day}', false, '10:00:00', '20:00:00')";
                            pg.ExecuteSqlQueryAsEnumerable(sqlQuery);
                        }
                        catch (Exception ex)
                        {
                            await Logger.LogError(ex);
                        }
                    }
                }
            }
        }


        /// <summary> Setting main admin schedule </summary>
        public async Task SetDays(UpdateInfo update, CallbackQuery? callback = null, int page = 1)
        {
            var days = Database.GetDays(page: page);
            var daysButtons = new List<InlineKeyboardButton[]>
            {
                new InlineKeyboardButton[] { "Дата", "Доступ", "Открытие", "Закрытие" }
            };

            foreach (var day in days)
            {
                daysButtons.Add(new InlineKeyboardButton[]
                {
                    new(day.Date.ToString("M")) { CallbackData = $"ChangeDay_{day.Date.ToString("yyyy-MM-dd")}" },
                    new(day.IsAvailable ? "🟢" : "🔴") { CallbackData = $"ChangeDay_{day.Date.ToString("yyyy-MM-dd")}" },
                    new(day.OpenTime.ToString()) { CallbackData = $"ChangeOpenTime_{day.Date.ToString("yyyy-MM-dd")}" },
                    new(day.CloseTime.ToString()) { CallbackData = $"ChangeCloseTime_{day.Date.ToString("yyyy-MM-dd")}" },
                });
            }

            daysButtons.Add(new InlineKeyboardButton[]
            {
                new("👈🏻") { CallbackData = $"Back" }, new("👉🏻") { CallbackData = $"Next" }
            });

            var inlineKeyboard = new InlineKeyboardMarkup(daysButtons);

            var replyMsg = $"<b>Установка расписания.</b>\n\n";
              //$"<b><code>║ ДАТА ║ ДОСТУП ║ НАЧАЛО ║ КОНЕЦ ║</code></b>\n";


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
                    await bot.BotClient.EditMessageTextAsync(update.Message.Chat.Id, callback.Message.MessageId, replyMsg, parseMode: ParseMode.Html, replyMarkup: inlineKeyboard);
                }
            else
                await bot.BotClient.SendTextMessageAsync(update.Message.Chat.Id, replyMsg, parseMode: ParseMode.Html, replyMarkup: inlineKeyboard);

            callback = await bot.NewButtonClick(update);
            if (callback == null) return;

            if (callback.Data.Contains("Back"))
            {
                page--;

                await SetDays(update, callback, page);
            }

            if (callback.Data.Contains("Next"))
            {
                page++;
                await SetDays(update, callback, page);
            }

            string selectedDate = new Regex(@"\d{4}-\d{2}-\d{2}").Match(callback.Data).Value;
            if (callback.Data.Contains("ChangeDay"))
            {
                await ChangeDayAvailability(update, callback, selectedDate, page);
                return;
            }

            if (callback.Data.Contains("ChangeOpenTime"))
            {
                await ChangeDayOpenTime(update, callback, selectedDate, "open", page);
                return;
            }

            if (callback.Data.Contains("ChangeCloseTime"))
            {
                await ChangeDayOpenTime(update, callback, selectedDate, "close", page);
                return;
            }

            else
                await SetDays(update);
        }


        private async Task ChangeDayAvailability(UpdateInfo update, CallbackQuery callback, string selectedDate, int page = 1)
        {
            sqlQuery = $"update days set is_available = NOT is_available where date = '{selectedDate}'";
            pg.ExecuteSqlQueryAsEnumerable(sqlQuery);

            await SetDays(update, callback, page);
        }


        private async Task ChangeDayOpenTime(UpdateInfo update, CallbackQuery callback, string selectedDate, string type, int page = 1)
        {
            var replyMsg = $"✍🏻 <i>Пришли новое время в формате</i> <code>00:00</code>";
            await bot.BotClient.EditMessageTextAsync(update.Message.Chat.Id, callback.Message.MessageId, replyMsg, parseMode: ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(new[]
            {
                new InlineKeyboardButton[] { "Назад" }
            }));

            var newMessage = await bot.NewTextMessage(update);
            if (newMessage == null) return;
            if (newMessage == "Назад") await SetDays(update, callback, page);

            var correctUserInput = new Regex(@"^([01]\d|2[0-3]):([0-5]\d)$").Match(newMessage).Success;

            if (correctUserInput)
            {
                newMessage += ":00";
                var userInputTime = TimeSpan.Parse(newMessage);

                sqlQuery = $"select open_time, close_time from days where date = '{selectedDate}'";
                var times = pg.ExecuteSqlQueryAsEnumerable(sqlQuery).Select(a => new
                {
                    Open = a.Field<TimeSpan>("open_time"),
                    Close = a.Field<TimeSpan>("close_time")
                }).ToList();

                switch (type)
                {
                    case "open":
                        {
                            if (userInputTime > times.First().Close)
                            {
                                await bot.BotClient.SendTextMessageAsync(update.Message.Chat.Id, $"❌ <b>Некорректное время: открытие должно быть раньше закрытия</b>", parseMode: ParseMode.Html);
                                await SetDays(update, page: page);
                                return;
                            }
                            break;
                        }

                    case "close":
                        {
                            if (userInputTime < times.First().Open)
                            {
                                await bot.BotClient.SendTextMessageAsync(update.Message.Chat.Id, $"❌ <b>Некорректное время: закрытие должно быть позже открытия</b>", parseMode: ParseMode.Html);
                                await SetDays(update, page: page);
                                return;
                            }
                            break;
                        }
                }

                sqlQuery = $"update days set {type}_time = '{newMessage}' where date = '{selectedDate}'";
                pg.ExecuteSqlQueryAsEnumerable(sqlQuery);

                await SetDays(update, page: page);
            }
            else
            {
                await bot.BotClient.SendTextMessageAsync(update.Message.Chat.Id, "❌ Некорректное время. \n\nПопробуйте снова в следующем формате: <code>00:00</code>", parseMode: ParseMode.Html);
                await SetDays(update, page: page);
            }
        }
    }
}
