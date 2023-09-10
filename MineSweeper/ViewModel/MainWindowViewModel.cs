using System;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MineSweeper.Model;
using MineSweeper.Commands;

namespace MineSweeper.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly MineFieldLogic _mineFieldLogic;

        public ICommand RestartGameCommand { get; }
        public ICommand MineFieldButtonClick { get; }
        public ICommand MineFieldRightClickCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindowViewModel()
        {
            _mineFieldLogic = new MineFieldLogic();
            MineFieldButtonClick = new MineFieldButtonClickCommand(_mineFieldLogic.OnMineFieldButtonClick);
            MineFieldRightClickCommand = new MineFieldRightClickCommand(_mineFieldLogic.OnMineFieldRightClick);
        }


        private void OnMineFieldButtonClick(MineFieldElement mineFieldElement)
        {
            if (mineFieldElement.Flagged)
            {
                return;
            }

            if (mineFieldElement.IsMine)
            {
                // Reveal the mine
                mineFieldElement.IsRevealed = true;

                // Handle game over
                GameOver();
            }
            else
            {
                // Reveal the square
                mineFieldElement.IsRevealed = true;

                // Assuming you have a method to get the position of a MineFieldElement
                (int x, int y) = GetPosition(mineFieldElement);

                mineFieldElement.NeighboringMines = CalculateNeighboringMines(x, y);

                // If there are no neighboring mines, reveal the neighbors recursively
                if (mineFieldElement.NeighboringMines == 0)
                {
                    RevealNeighbors(x, y);
                }

                // Check if all non-mine squares have been revealed
                if (CheckForWin())
                {
                    // Handle game win
                }
            }
        }



        private void GameOver()
        {
            // Reveal all mines
            foreach (var mineFieldElement in MineFieldElements)
            {
                if (mineFieldElement.IsMine)
                {
                    mineFieldElement.IsRevealed = true;
                }
            }

            // Update the game status message
            GameStatusMessage = "Game Over! You clicked on a mine.";

            // Raise the GameOverEvent
            GameOverEvent?.Invoke(this, GameStatusMessage);

            // You could raise a PropertyChanged event for the MineField property to ensure the UI updates
            OnPropertyChanged(nameof(MineField));
        }

        private void OnMineFieldRightClick(MineFieldElement mineFieldElement)
        {
            if (!mineFieldElement.IsRevealed)
            {
                mineFieldElement.Flagged = !mineFieldElement.Flagged;

                OnPropertyChanged(nameof(MinesRemaining));
            }
        }







        private string _gameStatusMessage;
        public string GameStatusMessage
        {
            get { return _gameStatusMessage; }
            set
            {
                if (_gameStatusMessage != value)
                {
                    _gameStatusMessage = value;
                    OnPropertyChanged(nameof(GameStatusMessage));
                }
            }
        }


    }

}
