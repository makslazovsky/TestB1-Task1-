using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using TestB1_Task1_.Interfaces;
using TestB1_Task1_.Model;

namespace TestB1_Task1_
{
    class DBAccessor : IDBAccessor
    {
        private string connectionString;
        const string spDecimalName = "CalculateDecimalMedian";
        const string spIntName = "CalculateIntegerSum";
        public DBAccessor(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public Task<string> CallculateDecimalMedian()
        {
            return CallStoredProcedureAsync(spDecimalName, SqlDbType.Float);
        }

        public Task<string> CallculateIntSum()
        {
            return CallStoredProcedureAsync(spIntName, SqlDbType.BigInt);
        }

        public async Task UploadFileLines(List<Line> lines)
        {
            DataTable dataTable = new DataTable("TableTask1");
            //Создание разметки для таблицы
            DataColumn Id = new DataColumn("Id");
            dataTable.Columns.Add(Id);
            DataColumn Date = new DataColumn("Date");
            dataTable.Columns.Add(Date);
            DataColumn EngSym = new DataColumn("EngSym");
            dataTable.Columns.Add(EngSym);
            DataColumn RusSym = new DataColumn("RusSym");
            dataTable.Columns.Add(RusSym);
            DataColumn UEvenInt = new DataColumn("UEvenInt");
            dataTable.Columns.Add(UEvenInt);
            DataColumn UDecimal = new DataColumn("UDecimal");
            dataTable.Columns.Add(UDecimal);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                foreach (Line line in lines)
                {
                    // добавление параметров в таблицу
                    dataTable.Rows.Add(0,
                                        line.Date,
                                        line.EngSym,
                                        line.RusSym,
                                        line.UEvenInt,
                                        line.UDecimal);
                }
                connection.Open();
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connection))
                {
                    sqlBulkCopy.DestinationTableName = "dbo.TableTask1";
                    await sqlBulkCopy.WriteToServerAsync(dataTable);
                }
            }
        }
        private async Task<string> CallStoredProcedureAsync(string comandName, SqlDbType sqlDbType)
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
                    return command.Parameters["@output"].Value.ToString();
                }
            }
        }
    }
}
