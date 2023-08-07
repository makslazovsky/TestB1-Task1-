using Microsoft.Win32;
using System.ComponentModel;
using System.Configuration;
using System.Windows;
using TestB1_Task1_.ViewModel;

namespace TestB1_Task1_
{
    public partial class MainWindow : Window
    {
        private BL bl;
        public MainWindow()
        {
            InitializeComponent();
            DBAccessor accessor = new DBAccessor(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            bl = new BL();
            ProgressViewModel progressViewModel = new ProgressViewModel(bl);
            DataContext = progressViewModel;
            bl.Init(accessor, progressViewModel);
            Closing += MainWindow_Closing;
        }
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            DataContext = null;
        }



        private async void CombineFileBtn_Click(object sender, RoutedEventArgs e)
        {
            await bl.DoTask2(CombineTB.Text);
        }

        private async void ImportFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                await bl.DoTask3(openFileDialog.FileName);
            }
        }

        private async void SumIntBtn_Click(object sender, RoutedEventArgs e)
        {
            await bl.DoTask41();
        }

        private async void MedianDeciamlBtn_Click(object sender, RoutedEventArgs e)
        {
            await bl.DoTask42();
        }
    }
}
