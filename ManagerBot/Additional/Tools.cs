﻿using ManagerBot.Data;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotFramework;
using Template.Data;
using Template.Monitoring;

namespace Template.Additional
{
    public static class Tools
    {
        private static readonly PgProvider pg = new(Bot.DatabaseConnectionString);
        private static string sqlQuery = "";


        public static async Task NotifyAdmins(ITelegramBotClient botClient, string text)
        {
            foreach (var admin in Config.Config.Admins)
                await botClient.SendTextMessageAsync(admin, text, parseMode: ParseMode.Html);
        }


        public static async Task DeleteDeadUsers(ITelegramBotClient botClient, UpdateInfo update)
        {   
            var users = pg.ExecuteSqlQueryAsEnumerable("select user_id, username from users").Select(a => new User
            {
                Id = a.Field<long>("user_id"),
                Username = a.Field<string?>("username")
            }).ToList();

            var deletedUsers = 0;

            await botClient.EditMessageTextAsync(update.Message.Chat.Id, update.Message.MessageId, "Запущена очистка. \n\n*По готовности будет уведомление*.", parseMode: ParseMode.Markdown);

            _ = Task.Run(async () =>
            {
                foreach (var user in users)
                {
                    try
                    {
                        await botClient.SendChatActionAsync(user.Id, ChatAction.Typing);
                    }
                    catch (Exception)
                    {
                        await DisableUserInDB(user);
                        deletedUsers++;
                        Thread.Sleep(50);
                        continue;
                    }
                }

                await botClient.SendTextMessageAsync(update.Message.Chat.Id, $"*Очистка юзеров успешно завершена* \n\nУдалено юзеров: *{deletedUsers}*", parseMode: ParseMode.Markdown);
            });
        }


        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }


        public static async Task AddUserToDB(User user)
        {
            sqlQuery = $@"select 1
                          from users
                          where user_id = {user.Id}";
            if (!pg.ExecuteSqlQueryAsEnumerable(sqlQuery).Any())
            {
                sqlQuery = $@"insert into users
                                          (user_id,
                                           username,
                                           created_at)
                                   values ({user.Id},
                                           {(user.Username != null ? $"'{user.Username}'" : "null")},
                                            {DateTime.Now.ToTimeStamp()})";
                pg.ExecuteSqlQueryAsEnumerable(sqlQuery);

                await Logger.LogMessage($"Добавлен пользователь {user.Id} {(user.Username != null ? $"@{user.Username}" : null)}");
            }
            else
            {
                var userData = Database.GetUser(user.Id);
                if (userData.Active == false)
                {

                    sqlQuery = $@"update users set active = true where user_id = {user.Id}";
                    pg.ExecuteSqlQueryAsEnumerable(sqlQuery);

                    await Logger.LogMessage($"Добавлен пользователь {user.Id} {(user.Username != null ? $"@{user.Username}" : "")}");
                }
            }
        }


        public static async Task DisableUserInDB(User user)
        {
            sqlQuery = $@"select 1
                              from users
                              where user_id = {user.Id}";
            if (pg.ExecuteSqlQueryAsEnumerable(sqlQuery).Any())
            {
                sqlQuery = $@"update users set active = false where user_id = {user.Id}";
                pg.ExecuteSqlQueryAsEnumerable(sqlQuery);

                await Logger.LogMessage($"Удален пользователь {user.Id} {(user.Username != null ? $"@{user.Username}" : "")}");
            }
        }
    }
}
