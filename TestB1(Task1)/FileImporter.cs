using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TestB1_Task1_.Interfaces;
using TestB1_Task1_.Model;

namespace TestB1_Task1_
{
    class FileImporter
    {
        private IProgressReporter progressReporter;
        private IDBAccessor dBAccessor;

        public FileImporter(IProgressReporter progressReporter, IDBAccessor dBAccessor)
        {
            this.progressReporter = progressReporter;
            this.dBAccessor = dBAccessor;
        }
        public async Task CallStoredProcedure3Async(string filePath)
        {
            int bulkSize = 250;
            long totalCount = 0;
            
            using (StreamReader reader = new StreamReader(filePath))
            {
                char x;
                while (!reader.EndOfStream)
                {
                    x = (char)reader.Read();
                    if (x=='\n')
                    {
                        totalCount++;
                    }
                }
            }
            progressReporter.SetMaxProgress(totalCount);
            using (StreamReader reader = new StreamReader(filePath))
            {
                string fileLine;
                
                
                int currentCount = 0;
                List<Line> lines = new List<Line>();
                while ((fileLine = reader.ReadLine()) != null)
                {
                    // разделение строки на параметры
                    string[] parameters = fileLine.Split(new string[] { "||" }, StringSplitOptions.None);
                    // добавление параметров в хранимую процедуру
                    Line line = new Line
                    {
                        Date = DateTime.Parse(parameters[0]),
                        EngSym = parameters[1],
                        RusSym = parameters[2],
                        UEvenInt = parameters[3],
                        UDecimal = parameters[4]
                    };
                    lines.Add(line);
                    currentCount++;

                    if (currentCount % bulkSize == 0)
                    {
                        await dBAccessor.UploadFileLines(lines);
                        lines.Clear();
                        progressReporter.SetCurrentProgress(currentCount);
                    }
                }
                if (lines.Any())
                {
                    await dBAccessor.UploadFileLines(lines);
                    progressReporter.SetCurrentProgress(currentCount);
                }
            }                             
        }
    }
}
