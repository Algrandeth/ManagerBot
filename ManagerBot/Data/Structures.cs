using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Template.Data
{
    public class Structures
    {

        public struct Day
        {
            public bool IsAvailable { get; set; }
            public TimeSpan OpenTime { get; set; }
            public TimeSpan CloseTime { get; set; }
            public DateTime Date { get; set; }
        }


        public class Sign
        {
            public int TimeSpan { get; set; }
            public string Username { get; set; }
            public TimeSpan Time { get; set; }
            public DateTime Date { get; set; }
            public bool IsActive { get; set; }
            public long UserID { get; set; }
            public string ID { get; set; }
        }
    }
}
