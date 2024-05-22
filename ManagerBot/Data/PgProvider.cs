using Npgsql;
using System.Data;
using Template.Monitoring;

namespace Template
{
    public class PgProvider
    {
        public string connectionString { get; set; }


        public PgProvider(string path) => connectionString = path;


        public EnumerableRowCollection<DataRow> ExecuteSqlQueryAsEnumerable(string sqlQuery)
        {
            return ExecuteSqlQueryAsDataTable(sqlQuery).AsEnumerable();
        }


        public DataTable ExecuteSqlQueryAsDataTable(string sqlQuery)
        {
            int attemptCount = 0;
            int maxRetries = 6;
            int commandTimeout = 600;
            int retryDelay = 500; // initial delay in milliseconds
            double delayMultiplier = 1.5; // multiplier for exponential backoff

            while (true)
            {
                try
                {
                    DataTable dataTable = new DataTable();
                    using (NpgsqlConnection pgConnection = new NpgsqlConnection(connectionString))
                    {
                        pgConnection.Open();
                        using (NpgsqlCommand pgCommand = new NpgsqlCommand(sqlQuery, pgConnection)
                        {
                            CommandType = CommandType.Text,
                            CommandTimeout = commandTimeout
                        })
                        {
                            using NpgsqlDataReader reader = pgCommand.ExecuteReader();
                            dataTable.Load(reader);
                        }

                        pgConnection.Close();
                    }

                    return dataTable;
                }
                catch (Exception ex)
                {
                    attemptCount++;
                    // Логируем исключение (рекомендуется использовать какой-нибудь логгер)
                    Console.WriteLine($"Exception on attempt {attemptCount}: {ex.Message}");

                    if (attemptCount > maxRetries)
                    {
                        throw;
                    }

                    // Экспоненциальная задержка перед повторной попыткой
                    Thread.Sleep(retryDelay);
                    retryDelay = (int)(retryDelay * delayMultiplier);
                }
            }
        }


        // OG
        //public DataTable ExecuteSqlQueryAsDataTable(string sqlQuery)
        //{
        //    int num = 0;
        //    int commandTimeout = 600;
        //    int num2 = 6;
        //    while (true)
        //    {
        //        try
        //        {
        //            DataTable dataTable = new DataTable();
        //            using (NpgsqlConnection pgConnection = new NpgsqlConnection(connectionString))
        //            {
        //                pgConnection.Open();
        //                using (NpgsqlCommand pgCommand = new NpgsqlCommand(sqlQuery, pgConnection)
        //                {
        //                    CommandType = CommandType.Text,
        //                    CommandTimeout = commandTimeout
        //                })
        //                {
        //                    using NpgsqlDataReader reader = pgCommand.ExecuteReader();
        //                    dataTable.Load(reader);
        //                }

        //                pgConnection.Close();
        //            }

        //            return dataTable;
        //        }
        //        catch (Exception ex)
        //        {
        //            num++;
        //            if (num > num2)
        //            {
        //                throw;
        //            }
        //        }

        //        Thread.Sleep(500);
        //    }
        //}
    }
}

