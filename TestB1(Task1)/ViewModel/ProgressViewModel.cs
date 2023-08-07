using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TestB1_Task1_.Interfaces;

namespace TestB1_Task1_.ViewModel
{
    public class ProgressViewModel : INotifyPropertyChanged, IProgressReporter
    {
        public ICommand FileCreateCommand { get; }

        public string patternText;

        private BL bl;
        private string text;
        private long progress;
        private long progressPercent;
        private long progressMax;
        
        public ProgressViewModel(BL bl) 
        {
            this.bl = bl;
            FileCreateCommand = new FileCreateCommand(bl);
        }
        
        public long Progress
        {
            get { return progress; }
            set
            {
                progress = value;
                NotifyPropertyChanged("Progress");
                if (progressMax == 0)
                {
                    ProgressPercent = 0;
                }
                else
                {
                    ProgressPercent = (int)(((double)progress / progressMax) * 100);
                }
            }
        }

        public long ProgressPercent
        {
            get { return progressPercent; }
            set
            {
                progressPercent = value;
                NotifyPropertyChanged("ProgressPercent");
            }
        }

        public long ProgressMax
        {
            get { return progressMax; }
            set
            {
                progressMax = value;
                NotifyPropertyChanged("ProgressMax");
            }
        }

        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                NotifyPropertyChanged("Text");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetMaxProgress(long maxProgress)
        {
            ProgressPercent = 0;
            Progress = 0;
            ProgressMax = maxProgress;
        }

        public void SetCurrentProgress(long currentProgress)
        {
            Progress = currentProgress;
        }

        public void SetText(string text)
        {
           Text = text;
        }

        public void AppendText(string text)
        {
           Text += text;
        }
    }
}
