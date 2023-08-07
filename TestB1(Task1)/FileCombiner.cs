using Microsoft.Win32;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TestB1_Task1_.Interfaces;

namespace TestB1_Task1_
{
    class FileCombiner
    {
        private IProgressReporter progressReporter;

        public FileCombiner(IProgressReporter progressReporter)
        {
            this.progressReporter = progressReporter;
        }
        public async Task CombineSaveFileAsync(string TBtext)
        {
            try
            {
                string outputDirectory = "Output"; // Указываем папку Output в базовой директории

                string[] files = Directory.GetFiles(outputDirectory); // Получаем список файлов из папки Output

                SaveFileDialog saveFileDialog = new SaveFileDialog();

                if (saveFileDialog.ShowDialog() == true)
                {
                    string outputPath = saveFileDialog.FileName;
                    await CombineFilesAsync(outputPath, files, TBtext);

                }
            }
            catch (Exception ex)
            {
                progressReporter.SetText($"Ошибка при объединении файлов: {ex.Message}");
            }

        }

        private async Task CombineFilesAsync(string outputPath, string[] filesToMerge, string patternToDelete)
        {
            int currentCount = 0;
            progressReporter.SetMaxProgress(filesToMerge.Length);
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
                        progressReporter.SetCurrentProgress(currentCount);
                    }
                }
                progressReporter.SetText($"Удалено {deletedLinesCount} линий ");
                progressReporter.AppendText("Файлы объеденены");
            }
            catch (Exception ex)
            {
                progressReporter.SetText($"Ошибка при объединении файлов: {ex.Message}");
            }
        }

       
    }
}
