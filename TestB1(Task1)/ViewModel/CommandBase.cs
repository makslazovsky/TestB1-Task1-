using System;
using System.Windows.Input;

namespace TestB1_Task1_.ViewModel
{
    public abstract class CommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged;
        protected BL Bl {get;}
        public CommandBase(BL bl) 
        {
            Bl = bl;
        }
        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        public abstract void Execute(object parameter);

        protected void OnCanExecutedChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}
