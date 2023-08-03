using Microsoft.Win32;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;
using TestB1_Task1_.ViewModel;

namespace TestB1_Task1_
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ProgressViewModel();
            Closing += MainWindow_Closing;
            FileFactory fileFactory = new FileFactory(OutputText, progressBar);
            FileCombiner fileCombiner = new FileCombiner(OutputText, progressBar);
            FileImporter fileImporter = new FileImporter(OutputText, progressBar);
            StoredProcedures storedProcedures = new StoredProcedures(OutputText, progressBar);
        }
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            DataContext = null;
        }

        private async void CreateFileBtn_Click(object sender, RoutedEventArgs e)
        {
            await FileFactory.FileCreate();
        }

        private void CombineFileBtn_Click(object sender, RoutedEventArgs e)
        {
             FileCombiner.CombineSaveFileAsync(CombineTB.Text);
        }

        private async void ImportFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                await FileImporter.CallStoredProcedure3Async(openFileDialog.FileName);
            }
        }

        private async void SumIntBtn_Click(object sender, RoutedEventArgs e)
        {
            await StoredProcedures.CallStoredProcedureAsync("CalculateIntegerSum", SqlDbType.BigInt);
        }

        private async void MedianDeciamlBtn_Click(object sender, RoutedEventArgs e)
        {
            await StoredProcedures.CallStoredProcedureAsync("CalculateDecimalMedian", SqlDbType.Float);
        }
    }
}
