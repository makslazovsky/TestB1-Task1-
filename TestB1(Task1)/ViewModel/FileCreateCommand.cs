using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestB1_Task1_.ViewModel
{
    public class FileCreateCommand : CommandBase
    {
        public FileCreateCommand(BL bl) : base(bl)
        {
        }

        public override void Execute(object parameter)
        {
            Bl.DoTask1();
        }
    }
}
