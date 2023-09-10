using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MineSweeper.Model;

namespace MineSweeper
{
    public class MineFieldButtonClickCommand : ICommand
    {
        private readonly Action<MineFieldElement> _execute;

        public MineFieldButtonClickCommand(Action<MineFieldElement> execute)
        {
            _execute = execute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            // Here, you can include logic to disable the button if necessary.
            // For now, we'll just allow the command to always be executed.
            return true;
        }

        public void Execute(object parameter)
        {
            _execute.Invoke(parameter as MineFieldElement);
        }
    }
}
