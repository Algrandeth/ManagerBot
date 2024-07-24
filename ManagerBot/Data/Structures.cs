using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Template.Data
{
    public class Structures
    {

        public class Day
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


        public class User
        {
            public string Username { get; set; }
            public long ID { get; set; }
            public DateTime CreatedAt { get; set; }
            public string Phone { get; set; }
            public bool Active { get; set; }
        }
    }
}
