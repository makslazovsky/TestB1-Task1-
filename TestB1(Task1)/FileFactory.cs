using System;
using System.IO;
using System.Threading.Tasks;
using TestB1_Task1_.Interfaces;

namespace TestB1_Task1_
{
    class FileFactory
    {
        private readonly Random random = new Random();
        const string charsEN = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string charsRU = "абвгдеёжзийклмнопрстуфхцчшщьыъэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        private IProgressReporter progressReporter;
        private int halfIntBound;
        private int decimalBound;
        public FileFactory(IProgressReporter progressReporter, int halfIntBound, int decimalBound)
        {
            this.progressReporter = progressReporter;
            this.halfIntBound = halfIntBound;
            this.decimalBound = decimalBound;
        }


        public async Task FileCreate(int fileCount, int lineCount)
        {
            string baseDirectory = Directory.GetCurrentDirectory();
            string outputPath = Path.Combine(baseDirectory, "Output");

            progressReporter.SetMaxProgress(fileCount);
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
                        progressReporter.SetCurrentProgress(fileIndex);
                       
                    }
                }
                progressReporter.SetText("Файлы успешно созданы");
            }
            catch (Exception ex)
            {
                progressReporter.SetText($"Ошибка при создании файлов: {ex.Message}");
            }
        }

        private string GenerateRandomLine()
        {
            DateTime randomDate = GenerateRandomDate();
            string randomLatin = GenerateRandomEngString(10);
            string randomRussian = GenerateRandomRusString(10);
            int randomNumber = GenerateRandomPositiveEvenNumber();
            decimal randomDecimal = GenerateRandomDecimal();

            return $"{randomDate:dd.MM.yyyy}||{randomLatin}||{randomRussian}||{randomNumber}||{randomDecimal:F8}";
        }

        private DateTime GenerateRandomDate()
        {
            DateTime startDate = DateTime.Now.AddYears(-5);
            int totalDays = (DateTime.Today - startDate).Days;

            return startDate.AddDays(random.Next(totalDays));
        }

        private string GenerateRandomEngString(int length)
        {
            char[] randomChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                randomChars[i] = charsEN[random.Next(charsEN.Length)];
            }
            return new string(randomChars);
        }

        private string GenerateRandomRusString(int length)
        {
            char[] randomChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                randomChars[i] = charsRU[random.Next(charsRU.Length)];
            }

            return new string(randomChars);
        }
        private int GenerateRandomPositiveEvenNumber()
        {
            int number = random.Next(1, halfIntBound) * 2;
            return number;
        }

        private decimal GenerateRandomDecimal()
        {
            double number = random.NextDouble() * decimalBound + 1;
            return (decimal)number;
        }
    }
}
