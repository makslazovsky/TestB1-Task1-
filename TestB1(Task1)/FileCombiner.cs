using Microsoft.Win32;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TestB1_Task1_
{
    class FileCombiner
    {
        private static TextBox _outputTB;
        private static ProgressBar _progressBar;

        public FileCombiner(TextBox OutputTB, ProgressBar progressBar)
        {
            _outputTB = OutputTB;
            _progressBar = progressBar;
        }

        private static async Task CombineFilesAsync(string outputPath, string[] filesToMerge, string patternToDelete)
        {
            int currentCount = 0;
            _progressBar.Maximum = filesToMerge.GetLength(0);
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
                        _progressBar.Dispatcher.Invoke(() =>
                        {
                            _progressBar.Value = currentCount;
                        });
                    }
                }
                _outputTB.Text = $"Удалено {deletedLinesCount} линий ";
                _outputTB.Text += "\nФайлы объеденены";
            }
            catch (Exception ex)
            {
                _outputTB.Text = $"Ошибка при объединении файлов: {ex.Message}";
            }
        }

        public static async void CombineSaveFileAsync(string TBtext)
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
                _outputTB.Text = $"Ошибка при объединении файлов: {ex.Message}";
            }

        }
    }
}
