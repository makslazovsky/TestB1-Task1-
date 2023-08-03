using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Windows.Controls;

namespace TestB1_Task1_
{
    class FileImporter
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private static TextBox _outputTB;
        private static ProgressBar _progressBar;
        static DataTable DataTable1;

        public FileImporter(TextBox OutputTB, ProgressBar progressBar)
        {
            _outputTB = OutputTB;
            _progressBar = progressBar;
        }
        public static async Task CallStoredProcedure3Async(string filePath)
        {
            int bulkSize = 250;

            DataTable1 = new DataTable("TableTask1");


            DataColumn Date = new DataColumn("Date");
            DataTable1.Columns.Add(Date);
            DataColumn EngSym = new DataColumn("EngSym");
            DataTable1.Columns.Add(EngSym);
            DataColumn RusSym = new DataColumn("RusSym");
            DataTable1.Columns.Add(RusSym);
            DataColumn UEvenInt = new DataColumn("UEvenInt");
            DataTable1.Columns.Add(UEvenInt);
            DataColumn UDecimal = new DataColumn("UDecimal");
            DataTable1.Columns.Add(UDecimal);
            DataColumn Id = new DataColumn("Id");
            DataTable1.Columns.Add(Id);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connection))
                {
                    sqlBulkCopy.DestinationTableName = "dbo.TableTask1";

                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        string line;
                        int totalCount = File.ReadLines(filePath).Count();
                        _progressBar.Maximum = totalCount;
                        int currentCount = 0;
                        while ((line = reader.ReadLine()) != null)
                        {
                            // разделение строки на параметры
                            string[] parameters = line.Split(new string[] { "||" }, StringSplitOptions.None);
                            // добавление параметров в хранимую процедуру
                            DataTable1.Rows.Add(currentCount,
                                                DateTime.Parse(parameters[0]),
                                                parameters[1],
                                                parameters[2],
                                                parameters[3],
                                                decimal.Parse(parameters[4]));

                            currentCount++;

                            if (currentCount % bulkSize == 0)
                            {
                                //Запись на сервер
                                await sqlBulkCopy.WriteToServerAsync(DataTable1);
                                DataTable1.Clear();
                                _progressBar.Dispatcher.Invoke(() =>
                                {
                                    _progressBar.Value = currentCount;
                                });
                            }
                        }
                    }                  
                }
            }
        }
    }
}
