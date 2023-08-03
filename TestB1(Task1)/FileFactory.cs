using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TestB1_Task1_
{
    class FileFactory
    {
        private static readonly Random random = new Random();
        const int fileCount = 100;
        const int lineCount = 100000;
        const int halfIntBound = 50000000;
        const int decimalBound = 19;
        const string charsEN = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string charsRU = "абвгдеёжзийклмнопрстуфхцчшщьыъэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        private static TextBox _outputTB;
        private static ProgressBar _progressBar;

        public FileFactory(TextBox OutputTB, ProgressBar progressBar)
        {
            _outputTB = OutputTB;
            _progressBar = progressBar;
        }


        public static async Task FileCreate()
        {
            string baseDirectory = Directory.GetCurrentDirectory();
            string outputPath = Path.Combine(baseDirectory, "Output");
            _progressBar.Maximum = fileCount;
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
                        _progressBar.Dispatcher.Invoke(() =>
                        {
                            _progressBar.Value = fileIndex;
                        });
                    }
                }
                _outputTB.Text = "Файлы успешно созданы";
            }
            catch (Exception ex)
            {
                _outputTB.Text = $"Ошибка при создании файлов: {ex.Message}";
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
            char[] randomChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                randomChars[i] = charsEN[random.Next(charsEN.Length)];
            }
            return new string(randomChars);
        }

        private static string GenerateRandomRusString(int length)
        {
            char[] randomChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                randomChars[i] = charsRU[random.Next(charsRU.Length)];
            }

            return new string(randomChars);
        }
        private static int GenerateRandomPositiveEvenNumber()
        {
            int number = random.Next(1, halfIntBound) * 2;
            return number;
        }

        private static decimal GenerateRandomDecimal()
        {
            double number = random.NextDouble() * decimalBound + 1;
            return (decimal)number;
        }
    }
}
