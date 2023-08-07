using System.Threading.Tasks;
using TestB1_Task1_.Interfaces;

namespace TestB1_Task1_
{
    public class BL
    {
        private IProgressReporter progressReporter;
        private IDBAccessor dBAccessor;
        private FileFactory fileFactory;
        private FileCombiner fileCombiner;
        private FileImporter fileImporter;
        const int fileCount = 100;
        const int lineCount = 100000;
        const int halfIntBound = 50000000;
        const int decimalBound = 19;
        public void Init(IDBAccessor dBAccessor, IProgressReporter progressReporter) 
        {
            this.progressReporter = progressReporter;
            this.dBAccessor = dBAccessor;
            fileFactory = new FileFactory(progressReporter, halfIntBound, decimalBound);
            fileCombiner = new FileCombiner(progressReporter);
            fileImporter = new FileImporter(progressReporter, dBAccessor);
        }

        public async Task DoTask1()
        {
            await fileFactory.FileCreate(fileCount, lineCount);
        }
        public async Task DoTask2(string patternToDelete)
        {
            await fileCombiner.CombineSaveFileAsync(patternToDelete);
        }
        public async Task DoTask3(string filePath)
        {
            await fileImporter.CallStoredProcedure3Async(filePath);
        }
        public async Task DoTask41()
        {
            string result = await dBAccessor.CallculateIntSum();
            progressReporter.SetText(result);
        }

        public async Task DoTask42()
        {
            string result = await dBAccessor.CallculateDecimalMedian();
            progressReporter.SetText(result);
        }
    }
}
