﻿using System.Data;
using Template;
using Template.Data;

namespace ManagerBot.Data
{
    public static class Database
    {
        private static readonly PgProvider pg = new(Bot.DatabaseConnectionString);

        public static List<Structures.Day> GetDays(bool? isAvailable = null, string? date = null, int page = 1, int limit = 10)
        {
            if (page < 1)
                page = 1;

            var sql = @$"select date, 
                          is_available, 
                          open_time, 
                          close_time 
                   from days 
                   where 
                   {(date != null ? $"date = '{date}'" : "date >= now()")}
                   {(isAvailable != null ? $"and is_available = {isAvailable}" : null)}
                   order by date
                   limit {limit}
                   offset {(page - 1) * limit}";

            var days = pg.ExecuteSqlQueryAsEnumerable(sql).Select(a => new Structures.Day
            {
                Date = a.Field<DateTime>("date"),
                OpenTime = a.Field<TimeSpan>("open_time"),
                CloseTime = a.Field<TimeSpan>("close_time"),
                IsAvailable = a.Field<bool>("is_available")
            }).ToList();
            return days;
        }


        public static List<Structures.Sign> GetSigns(bool? is_active = true, string? date = null, int page = 1, int limit = 10, long? user_id = null, string? signID = null)
        {
            if (page < 1)
                page = 1;

            var sql = @$"select username, 
                                timespan, 
                                time, 
                                date,
                                is_active,
                                user_id,
                                id
                         from signs 
                         where {(date != null ? $"date = '{date}'" : "date >= now()")}
                         and is_active = {is_active}
                         {(user_id != null ? $"and user_id = {user_id}" : "")}
                         {(signID != null ? $"and id = '{signID}'" : "")}
                         order by date
                         limit {limit}
                         offset {(page - 1) * limit}";

            var signs = pg.ExecuteSqlQueryAsEnumerable(sql).Select(a => new Structures.Sign
            {
                Date = a.Field<DateTime>("date"),
                Username = a.Field<string>("username"),
                Time = a.Field<TimeSpan>("time"),
                TimeSpan = a.Field<int>("timespan"),
                IsActive = a.Field<bool>("is_active"),
                UserID = a.Field<long>("user_id"),
                ID = a.Field<string>("id")
            }).ToList();
            return signs;
        }
    }
}
