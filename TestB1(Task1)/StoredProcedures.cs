using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Windows.Controls;

namespace TestB1_Task1_
{
    class StoredProcedures
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private static TextBox _outputTB;
        private static ProgressBar _progressBar;
        public StoredProcedures(TextBox OutputTB, ProgressBar progressBar)
        {
            _outputTB = OutputTB;
            _progressBar = progressBar;
        }
        public static async Task CallStoredProcedureAsync(string comandName, SqlDbType sqlDbType)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                // создание хранимой процедуры
                using (SqlCommand command = new SqlCommand(comandName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@output",
                        SqlDbType = sqlDbType,
                        Direction = ParameterDirection.Output
                    });
                    // выполнение хранимой процедуры
                    await command.ExecuteNonQueryAsync();
                    _outputTB.Text = command.Parameters["@output"].Value.ToString();
                }
            }
        }
    }
}
