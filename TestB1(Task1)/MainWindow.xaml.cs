using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace TestB1_Task1_
{
    public partial class MainWindow : Window
    {
        string connectionString;
        SqlDataAdapter adapter;
        DataTable phonesTable;
        public MainWindow()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataContext = new ProgressViewModel();
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            DataContext = null;
        }

        private static readonly Random random = new Random();

        #region FileFactory
        public async Task FileCreate()
        {
            const int fileCount = 100;
            const int lineCount = 100000;

            string baseDirectory = Directory.GetCurrentDirectory();
            string outputPath = Path.Combine(baseDirectory, "Output");
            Directory.CreateDirectory(outputPath);

            try
            {
                for (int fileIndex = 1; fileIndex <= fileCount; fileIndex++)
                {
                    string filePath = Path.Combine(outputPath, $"File{fileIndex}.txt");

                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        for (int lineIndex = 1; lineIndex <= lineCount; lineIndex++)
                        {
                            string line = GenerateRandomLine();
                            await writer.WriteLineAsync(line);
                        }
                    }
                }
                OutputText.Text = $"Файлы успешно созданы";
            }
            catch (Exception ex)
            {
                OutputText.Text = $"Ошибка при создании файлов: {ex.Message}";
            }
        }

        private static string GenerateRandomLine()
        {
            DateTime randomDate = GenerateRandomDate();
            string randomLatin = GenerateRandomEngString(10);
            string randomRussian = GenerateRandomRusString(10);
            int randomNumber = GenerateRandomPositiveEvenNumber();
            decimal randomDecimal = GenerateRandomDecimal();

            return $"{randomDate:dd.MM.yyyy}||{randomLatin}||{randomRussian}||{randomNumber}||{randomDecimal:F8}";
        }

        private static DateTime GenerateRandomDate()
        {
            DateTime startDate = DateTime.Now.AddYears(-5);
            int totalDays = (DateTime.Today - startDate).Days;

            return startDate.AddDays(random.Next(totalDays));
        }

        private static string GenerateRandomEngString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

            char[] randomChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                randomChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(randomChars);
        }

        private static string GenerateRandomRusString(int length)
        {
            const string chars = "абвгдеёжзийклмнопрстуфхцчшщьыъэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";

            char[] randomChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                randomChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(randomChars);
        }
        private static int GenerateRandomPositiveEvenNumber()
        {
            int number = random.Next(1, 50000000) * 2;
            return number;
        }

        private static decimal GenerateRandomDecimal()
        {
            double number = random.NextDouble() * 19 + 1;
            return (decimal)number;
        }

        #endregion

        #region FileCombiner
        private async Task CombineFilesAsync(string outputPath, string[] filesToMerge, string patternToDelete)
        {
            int currentCount = 0;
            progressBar.Maximum = filesToMerge.GetLength(0);
            int deletedLinesCount = 0;
            try
            {
                using (var outputStream = new StreamWriter(outputPath))
                {
                    foreach (var filePath in filesToMerge)
                    {
                        using (var inputStream = new StreamReader(filePath))
                        {
                            string line;
                            while ((line = await inputStream.ReadLineAsync()) != null)
                            {
                                if (Regex.IsMatch(line, patternToDelete))
                                {
                                    deletedLinesCount++;
                                    continue;
                                }
                                await outputStream.WriteLineAsync(line);
                            }
                        }
                        currentCount++;
                        progressBar.Dispatcher.Invoke(() =>
                        {
                            progressBar.Value = currentCount;
                        });
                    }
                }
                OutputText.Text = $"Удалено {deletedLinesCount} линий ";
                OutputText.Text += "\nФайлы объеденены";
            }
            catch (Exception ex)
            {
                OutputText.Text = $"Ошибка при объединении файлов: {ex.Message}";
            }
        }

        private async void CombineSaveFile()
        {
            try
            {
                string outputDirectory = "Output"; // Указываем папку Output в базовой директории

                string[] files = Directory.GetFiles(outputDirectory); // Получаем список файлов из папки Output

                SaveFileDialog saveFileDialog = new SaveFileDialog();

                if (saveFileDialog.ShowDialog() == true)
                {
                    string outputPath = saveFileDialog.FileName;
                    await CombineFilesAsync(outputPath, files, CombineTB.Text);

                }
            }
            catch (Exception ex)
            {
                OutputText.Text = $"Ошибка при объединении файлов: {ex.Message}";
            }
                
        }
        #endregion

        #region FileImporter

        public class ProgressViewModel : INotifyPropertyChanged
        {
            private int progress;
            public int Progress
            {
                get { return progress; }
                set
                {
                    progress = value;
                    NotifyPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public async Task CallStoredProcedure3Async(string filePath)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                // создание хранимой процедуры
                using (SqlCommand command = new SqlCommand("sp_InsertRecord", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    // чтение текстового файла
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        string line;
                        int totalCount = File.ReadLines(filePath).Count();
                        progressBar.Maximum = totalCount;
                        int currentCount = 0;
                        while ((line = reader.ReadLine()) != null)
                        {
                                // разделение строки на параметры
                                string[] parameters = line.Split(new string[] { "||" }, StringSplitOptions.None);

                                // добавление параметров в хранимую процедуру
                                command.Parameters.AddWithValue("@Date", DateTime.Parse(parameters[0]));
                                command.Parameters.AddWithValue("@EngSym", parameters[1]);
                                command.Parameters.AddWithValue("@RusSym", parameters[2]);
                                command.Parameters.AddWithValue("@UEvenInt", parameters[3]);
                                command.Parameters.AddWithValue("@UDecimal", decimal.Parse(parameters[4]));

                                // выполнение хранимой процедуры
                                await command.ExecuteNonQueryAsync();

                                // очистка параметров для следующей строки
                                command.Parameters.Clear();

                                currentCount++;
                                progressBar.Dispatcher.Invoke(() =>
                                {
                                    progressBar.Value = currentCount;
                                });
                        }
                    }
                }
            }
        }

        #endregion

        #region StoredProcedures

        public async Task CallStoredProcedureAsync(string comandName, SqlDbType sqlDbType)
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
                    OutputText.Text = command.Parameters["@output"].Value.ToString();
                }
            }
        }


        #endregion
        private async void CreateFileBtn_Click(object sender, RoutedEventArgs e)
        {
            await FileCreate();
        }

        private void CombineFileBtn_Click(object sender, RoutedEventArgs e)
        {
             CombineSaveFile();
        }

        private async void ImportFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                await CallStoredProcedure3Async(openFileDialog.FileName);
            }
        }

        private async void SumIntBtn_Click(object sender, RoutedEventArgs e)
        {
            await CallStoredProcedureAsync("CalculateIntegerSum", SqlDbType.BigInt);
        }

        private async void MedianDeciamlBtn_Click(object sender, RoutedEventArgs e)
        {
            await CallStoredProcedureAsync("CalculateDecimalMedian", SqlDbType.Float);
        }
    }
}
