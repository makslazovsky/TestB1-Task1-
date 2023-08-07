using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using TestB1_Task1_.Model;

namespace TestB1_Task1_.Interfaces
{
    public interface IDBAccessor
    {
        Task UploadFileLines(List<Line> lines);
        Task<string> CallculateIntSum();
        Task<string> CallculateDecimalMedian();
    }
}
