using ManagerBot.Data;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotFramework;
using Template.Additional;
using Template.Data;

namespace Template.Entities
{
    public partial class CommandHandler
    {
        /// <summary> Bot users count </summary>
        public async Task GetAvailableSchedule(UpdateInfo update, CallbackQuery? callback = null, int page = 1)
        {
            if (!pg.ExecuteSqlQueryAsEnumerable("select 1 from days where is_available = true").Any())
            {
                await bot.BotClient.SendTextMessageAsync(update.Message.Chat.Id, $"<b>К сожалению, доступных дат нет!</b>", parseMode: ParseMode.Html);
                return;
            }

            var days = Database.GetDays(page: page, isAvailable: true);
            var daysButtons = new List<InlineKeyboardButton[]>();

            if (days.Any() == false)
            {
                page--;
                await GetAvailableSchedule(update, callback, page);
                return;
            }

            for (int i = 0; i < days.Count + 1; i += 2)
            {
                try
                {
                    if (i + 1 < days.Count)
                        daysButtons.Add(new InlineKeyboardButton[]
                        {
                            new(days[i].Date.ToString("M")) { CallbackData = $"SignUp_{days[i].Date.ToString("yyyy-MM-dd")}" },
                            new(days[i + 1].Date.ToString("M")) { CallbackData = $"SignUp_{days[i + 1].Date.ToString("yyyy-MM-dd")}" },
                        });
                    else
                        daysButtons.Add(new InlineKeyboardButton[]
                        {
                            new(days[i].Date.ToString("M")) { CallbackData = $"SignUp_{days[i].Date.ToString("yyyy-MM-dd")}" },
                        });
                }
                catch (Exception) { }
            }

            daysButtons.Add(new InlineKeyboardButton[]
            {
                new("👈🏻") { CallbackData = $"Back" }, new("👉🏻") { CallbackData = $"Next" }
            });

            var inlineKeyboard = new InlineKeyboardMarkup(daysButtons);

            var replyMsg = $"<b>Доступные даты для записи 👇🏻</b>\n\n";

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
                await GetAvailableSchedule(update, callback, page);
            }

            if (callback.Data.Contains("Next"))
            {
                page++;
                await GetAvailableSchedule(update, callback, page);
            }

            if (callback.Data.Contains("SignUp"))
            {
                string selectedDate = new Regex(@"\d{4}-\d{2}-\d{2}").Match(callback.Data).Value;
                await SignUp(update, callback, selectedDate);
            }
        }


        private async Task SignUp(UpdateInfo update, CallbackQuery? callback, string selectedDate)
        {
            #region Запрос номера, если отсутствует юзернейм
            var replyMsg = "";

            bool phoneRequested = false;
            string? phoneNumber = null;
            if (update.Message.Chat.Username == null)
            {
                var user = Database.GetUser(update.Message.Chat.Id);
                if (user!.Phone == null)
                {
                    if (callback != null)
                        await bot.BotClient.DeleteMessageAsync(update.Message.Chat.Id, callback.Message.MessageId);

                    replyMsg = $"<b>Пожалуйста, предоставьте ваш номер телефона для контакта с вами, нажав на кнопку ниже.\n\n" +
                               $"Если вы не хотите оставлять номер - вы можете установить <code><i>Имя пользователя</i></code> в <a href=\"tg://settings/edit_profile/\">настройках</a>.\n\nПосле этого нажмите /sign_up снова.</b>";

                    var messageToDelete = (await bot.BotClient.SendTextMessageAsync(update.Message.Chat.Id, replyMsg, parseMode: ParseMode.Html, replyMarkup: new ReplyKeyboardMarkup(new List<KeyboardButton>
                    {
                        new("Дать контакт") { RequestContact = true }
                    }))).MessageId;

                    var userPhone = await bot.NewFullMessage(update);
                    if (userPhone == null) return;
                    if (userPhone.Contact == null)
                    {
                        await bot.BotClient.SendTextMessageAsync(update.Message.Chat.Id, $"Вы не нажали на кнопку <code>Дать контакт</code>, попробуйте заново.", parseMode: ParseMode.Html);
                        await SignUp(update, null, selectedDate);
                        return;
                    }

                    await Database.EditUser(update.Message.Chat.Id, phone: userPhone.Contact!.PhoneNumber);

                    await bot.BotClient.DeleteMessageAsync(update.Message.Chat.Id, userPhone.MessageId);
                    await bot.BotClient.DeleteMessageAsync(update.Message.Chat.Id, messageToDelete);

                    phoneNumber = userPhone.Contact!.PhoneNumber;
                    phoneRequested = true;
                }
                else
                    phoneNumber = user.Phone;
            }

            #endregion

            var signData = new Structures.Sign()
            {
                Username = update.Message.Chat.Username ?? phoneNumber,
                UserID = update.Message.Chat.Id,
                Date = DateTime.Parse(selectedDate)
            };

            #region Выбор длительности занятия
            replyMsg = $"<b>Ваш контакт для связи: <code>{(update.Message.Chat.Username != null ? $"@{signData.Username}" : $"{phoneNumber}")}</code></b>\n" +
                       $"<b>Выбранная дата: <code>{DateTime.Parse(selectedDate).ToString("D")}</code></b>\n\n" +
                       "<b>Выберите интересующую вас длительность занятия 👇🏻</b>";
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new InlineKeyboardButton[]
                {
                    new("60 минут") { CallbackData = "60" },
                    new("90 минут") { CallbackData = "90" }
                },
                new InlineKeyboardButton[]
                {
                    new("👈🏻 Назад") { CallbackData = "Back"}
                }
            });

            if (phoneRequested == false)
                await bot.BotClient.EditMessageTextAsync(update.Message.Chat.Id, callback.Message.MessageId, replyMsg, parseMode: ParseMode.Html, replyMarkup: inlineKeyboard);
            else
                await bot.BotClient.SendTextMessageAsync(update.Message.Chat.Id, replyMsg, parseMode: ParseMode.Html, replyMarkup: inlineKeyboard);
            #endregion

            #region Выбор времени занятия

            #region OG
            //callback = await bot.NewButtonClick(update);
            //if (callback == null) return;
            //if (callback.Data == "Back")
            //    await GetAvailableSchedule(update, callback);

            //signData.TimeSpan = callback.Data == "1" ? 1 : 2;

            //var thisDateSigns = Database.GetSigns(date: selectedDate);
            //var thisDateData = Database.GetDays(date: selectedDate).First();

            //var availableHours = new List<TimeSpan?>();
            //for (int hour = thisDateData.OpenTime.Hours; hour < thisDateData.CloseTime.Hours + 1; hour++)
            //{
            //    if (thisDateSigns.Any(a => a.Time.Hours == hour))
            //        continue;

            //    availableHours.Add(new TimeSpan(hour, 0, 0));
            //}

            //var hoursButtons = new List<InlineKeyboardButton[]>();
            //for (int i = 0; i < availableHours.Count + 1; i += 2)
            //{
            //    try
            //    {
            //        if (i + 1 < availableHours.Count)
            //            hoursButtons.Add(new InlineKeyboardButton[] { $"{DateTime.Parse(availableHours[i].ToString()):t}", $"{DateTime.Parse(availableHours[i + 1].ToString()):t}" });
            //        else
            //            hoursButtons.Add(new InlineKeyboardButton[] { $"{DateTime.Parse(availableHours[i].ToString()):t}" });
            //    }
            //    catch (Exception) { }
            //}
            //hoursButtons.Add(new InlineKeyboardButton[] { new InlineKeyboardButton("👈🏻 Назад") { CallbackData = "Back" } });

            //replyMsg = $"<b>Ваш контакт для связи: <code>{(update.Message.Chat.Username != null ? $"@{signData.Username}" : $"{phoneNumber}")}</code></b>\n" +
            //           $"<b>Выбранная дата: <code>{DateTime.Parse(selectedDate).ToString("D")}</code></b>\n" +
            //            $"<b>Выбранный Длительность: <code>{(signData.TimeSpan == 1 ? "60 минут" : "90 минут")}</code></b>\n\n" +
            //            $"<b>Выберите интересующее вас время 👇🏻</b>";
            //await bot.BotClient.EditMessageTextAsync(update.Message.Chat.Id, callback.Message.MessageId, replyMsg, ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(hoursButtons));
            #endregion

            #region DEV
            callback = await bot.NewButtonClick(update);
            if (callback == null) return;
            if (callback.Data == "Back")
                await GetAvailableSchedule(update, callback);

            signData.TimeSpan = Convert.ToInt32(callback.Data);

            var thisDateSigns = Database.GetSigns(date: selectedDate);
            var thisDateData = Database.GetDays(date: selectedDate).First();

            var availableHours = new List<TimeSpan?>();
            for (int hour = thisDateData.OpenTime.Hours; hour < thisDateData.CloseTime.Hours + 1; hour++)
            {
                availableHours.Add(new TimeSpan(hour, 0, 0));
                availableHours.Add(new TimeSpan(hour, 30, 0));
            }

            var hoursToDelete = new List<TimeSpan?>();

            var hourBeforeOpen = availableHours.Find(a => a.Value < thisDateData.OpenTime);
            var hourAfterClose = availableHours.Find(a => a.Value > thisDateData.CloseTime);

            if (hourBeforeOpen != null)
                hoursToDelete.Add(hourBeforeOpen);
            if (hourAfterClose != null)
                hoursToDelete.Add(hourAfterClose);

            // Удаление некорректного времени, которое превысило бы время закрытия
            for (int i = 0; i < signData.TimeSpan / 30; i++)
            {
                if (i == 0)
                    hoursToDelete.Add(thisDateData.CloseTime);
                else
                    hoursToDelete.Add(thisDateData.CloseTime.Add(new TimeSpan(0, -(30 * i), 0)));
            }

            //Удаление доступного времени для записи в зависимости от длительности занятия
            foreach (var currentSign in thisDateSigns)
            {
                for (int i = 0; i < currentSign.TimeSpan / 30; i++)
                {
                    if (i == 0)
                        hoursToDelete.Add(currentSign.Time);
                    else
                        hoursToDelete.Add(currentSign.Time.Add(new TimeSpan(0, 30 * i, 0)));
                }
            }

            foreach (var sign in thisDateSigns)
            {
                var signTime = sign.Time;
                var availableHoursBeforeSignTime = availableHours.Where(a => a.Value < signTime).ToList();

                foreach (var hour in availableHours)
                {
                    var restTimeBeforeSign = signTime - hour.Value;
                    if (restTimeBeforeSign.TotalMinutes <= 0)
                        break;
                    if (restTimeBeforeSign.TotalMinutes < signData.TimeSpan)
                        hoursToDelete.Add(hour);
                }
            }

            // Удаление некорректного времени
            foreach (var hour in hoursToDelete)
                availableHours.Remove(hour);

            var hoursButtons = new List<InlineKeyboardButton[]>();
            for (int i = 0; i < availableHours.Count + 1; i += 2)
            {
                try
                {
                    if (i + 1 < availableHours.Count)
                        hoursButtons.Add(new InlineKeyboardButton[]
                        {
                            $"{DateTime.Parse(availableHours[i].ToString()):t}",
                            $"{DateTime.Parse(availableHours[i + 1].ToString()):t}"
                        });
                    else
                        hoursButtons.Add(new InlineKeyboardButton[]
                        {
                            $"{DateTime.Parse(availableHours[i].ToString()):t}"
                        });
                }
                catch (Exception) { }
            }


            hoursButtons.Add(new InlineKeyboardButton[] { new InlineKeyboardButton("👈🏻 Назад") { CallbackData = "Back" } });

            replyMsg = $"<b>Ваш контакт для связи: <code>{(update.Message.Chat.Username != null ? $"@{signData.Username}" : $"{phoneNumber}")}</code></b>\n" +
                       $"<b>Выбранная дата: <code>{DateTime.Parse(selectedDate).ToString("D")}</code></b>\n" +
                       $"<b>Выбранная длительность: <code>{signData.TimeSpan} минут</code></b>\n" +
                       $"<b>Выберите интересующее вас время 👇🏻</b>";
            await bot.BotClient.EditMessageTextAsync(update.Message.Chat.Id, callback.Message.MessageId, replyMsg, ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(hoursButtons));
            #endregion
            #endregion

            #region Подтверждение
            callback = await bot.NewButtonClick(update);
            if (callback == null) return;
            if (callback.Data == "Back")
                await SignUp(update, callback, selectedDate);

            signData.Time = TimeSpan.Parse(callback.Data);

            replyMsg = $"<b>Ваш контакт для связи: <code>{(update.Message.Chat.Username != null ? $"@{signData.Username}" : $"{phoneNumber}")}</code></b>\n" +
                       $"<b>Выбранная дата: <code>{DateTime.Parse(selectedDate).ToString("D")}</code></b>\n" +
                        $"<b>Выбранное время: <code>{DateTime.Parse(callback.Data):t}</code></b>\n\n" +
                        $"<b>Выбранная длительность: <code>{signData.TimeSpan} минут</code></b>\n" +
                        $"<b>Все верно? Подтверждаем? 👇🏻</b>";
            inlineKeyboard = new InlineKeyboardMarkup(new InlineKeyboardButton[]
            {
                new("❌") { CallbackData = "0" },
                new("✅") { CallbackData = "1", }
            });

            await bot.BotClient.EditMessageTextAsync(update.Message.Chat.Id, callback.Message.MessageId, replyMsg, ParseMode.Html, replyMarkup: inlineKeyboard);
            #endregion

            #region Вставка в Postgres
            callback = await bot.NewButtonClick(update);
            if (callback == null) return;
            if (callback.Data == "0")
                await bot.BotClient.EditMessageTextAsync(update.Message.Chat.Id, callback.Message.MessageId, $"<b>Запись отменена!\n\nЕсли передумаете - /sign_up</b>", ParseMode.Html);
            if (callback.Data == "1")
            {
                sqlQuery = @$"insert into signs (username, 
                                                 timespan, 
                                                 time, 
                                                 date, 
                                                 is_active, 
                                                 user_id,
                                                 id)
                              values ('{signData.Username}', 
                                      {signData.TimeSpan}, 
                                      '{signData.Time}', 
                                      '{signData.Date:yyyy-MM-dd}',
                                      true,
                                      {signData.UserID},
                                      '{Guid.NewGuid().ToString().Replace("-", "")}')";
                pg.ExecuteSqlQueryAsEnumerable(sqlQuery);

                await bot.BotClient.EditMessageTextAsync(update.Message.Chat.Id, callback.Message.MessageId, $"<b>✅ Вы успешно записались! С вами свяжутся.</b>", parseMode: ParseMode.Html);

                var notifyAdminsMessage = $"<b>Новая запись! ✅</b>\n\n" +
                    $"<b>Контакт: <code>{(update.Message.Chat.Username != null ? $"@{signData.Username}" : $"{phoneNumber}")}</code></b>\n" +
                    $"<b>Дата: <code>{signData.Date:D}</code></b>\n" +
                    $"<b>Время: <code>{DateTime.Parse(signData.Time.ToString()):t}</code></b>\n" +
                    $"<b>Длительность: <code>{signData.TimeSpan} минут</code></b>\n";
                await Tools.NotifyAdmins(bot.BotClient, notifyAdminsMessage);
            }
            #endregion
        }
    }
}
