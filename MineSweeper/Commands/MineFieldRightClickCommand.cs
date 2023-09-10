﻿using System;
using System.Windows.Input;
using MineSweeper.Model;

namespace MineSweeper.Commands
{
    public class MineFieldRightClickCommand : ICommand
    {
        private readonly Action<MineFieldElement> _execute;

        public MineFieldRightClickCommand(Action<MineFieldElement> execute)
        {
            _execute = execute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _execute((MineFieldElement)parameter);
        }
    }
}