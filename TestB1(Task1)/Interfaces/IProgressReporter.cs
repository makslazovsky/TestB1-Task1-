namespace TestB1_Task1_.Interfaces
{
    public interface IProgressReporter
    {
        void SetMaxProgress(long maxProgress);
        void SetCurrentProgress(long currentProgress);
        void SetText(string text);
        void AppendText(string text);
    }
}
