using Telegram.Bot.Types;

namespace Template.Data
{
    public static class CommandsStore
    {
        public static List<BotCommand> AdminCommandsList = new()
        {
            new BotCommand() { Command = "/start", Description = "Панель управления" },
            new BotCommand() { Command = "/stats", Description = "Статистика пользователей в боте" },
            new BotCommand() { Command = "/download_users", Description = "Скачать базу пользователей" },
            new BotCommand() { Command = "/mailing", Description = "Рассылка" },
            new BotCommand() { Command = "/set_days", Description = "Установить время" },
            new BotCommand() { Command = "/my_schedule", Description = "Актуальные записи" },

        };


        public static List<BotCommand> UserCommandsList = new()
        {
            new BotCommand() { Command = "/start", Description = "Запуск бота" },
            new BotCommand() { Command = "/sign_up", Description = "Запись на занятие" },
            new BotCommand() { Command = "/my_schedule", Description = "Мои записи" }
        };


        public static List<string> CommandList = new()
        {
            "Актуальные записи", 
            "Рассылка",
            "Статистика",
            "Установить расписание",
            "Запись на занятие",
            "Мои записи"
        };


        public static List<string> MetaCommandList = new()
        {

        };
        

        public static void InitCommandList()
        {
            List<string> adminCommands = AdminCommandsList.Select(a => a.Command).ToList();
            List<string> usersCommands = UserCommandsList.Select(a => a.Command).ToList();

            CommandList.AddRange(adminCommands);
            CommandList.AddRange(usersCommands);
        }
    }
}
