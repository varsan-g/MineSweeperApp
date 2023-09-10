using System;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MineSweeper.Model;
using MineSweeper.Commands;
using System.Linq;

namespace MineSweeper.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public ICommand MineFieldButtonClick { get; private set; }
        public ICommand MineFieldRightClickCommand { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public delegate void GameOverEventHandler(object sender, string e);
        public event GameOverEventHandler GameOverEvent;

        public MineFieldElement[,] MineField { get; set; }

        private const int TotalMines = 5;  // The total number of mines

        public MainWindowViewModel()
        {
            MineFieldButtonClick = new MineFieldButtonClickCommand(OnMineFieldButtonClick);
            MineFieldRightClickCommand = new MineFieldRightClickCommand(OnMineFieldRightClick);
            InitializeMineField();
        }

        public IEnumerable<MineFieldElement> MineFieldElements
        {
            get
            {
                return MineField.Cast<MineFieldElement>();
            }
        }

        private void InitializeMineField()
        {
            MineField = new MineFieldElement[10, 5];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    MineField[i, j] = new MineFieldElement();
                }
            }

            GenerateMines(TotalMines);

        }

        private void GenerateMines(int numberOfMines)
        {
            Random random = new Random();
            for (int i = 0; i < numberOfMines; i++)
            {
                int x, y;
                do
                {
                    x = random.Next(0, MineField.GetLength(0)); // Get random x coordinate
                    y = random.Next(0, MineField.GetLength(1)); // Get random y coordinate
                } while (MineField[x, y].IsMine); // Keep getting new coordinates if the cell already contains a mine

                MineField[x, y].IsMine = true; // Set the cell to be a mine
            }
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

        private void RevealNeighbors(int x, int y)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    // Skip the square itself
                    if (i == 0 && j == 0)
                        continue;

                    int neighborX = x + i;
                    int neighborY = y + j;

                    // Skip squares outside the minefield
                    if (neighborX < 0 || neighborY < 0 || neighborX >= MineField.GetLength(0) || neighborY >= MineField.GetLength(1))
                        continue;

                    var neighbor = MineField[neighborX, neighborY];

                    // Skip if the neighbor is already revealed or flagged
                    if (neighbor.IsRevealed || neighbor.Flagged)
                        continue;

                    // Reveal the neighbor
                    neighbor.IsRevealed = true;

                    neighbor.NeighboringMines = CalculateNeighboringMines(neighborX, neighborY);

                    // If the neighbor has no neighboring mines, reveal its neighbors recursively
                    if (neighbor.NeighboringMines == 0)
                    {
                        RevealNeighbors(neighborX, neighborY);
                    }
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

        private int CalculateNeighboringMines(int x, int y)
        {
            int mineCount = 0;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    // Skip the square itself
                    if (i == 0 && j == 0)
                        continue;

                    int neighborX = x + i;
                    int neighborY = y + j;

                    // Skip squares outside the minefield
                    if (neighborX < 0 || neighborY < 0 || neighborX >= MineField.GetLength(0) || neighborY >= MineField.GetLength(1))
                        continue;

                    if (MineField[neighborX, neighborY].IsMine)
                        mineCount++;
                }
            }

            return mineCount;
        }

        private bool CheckForWin()
        {
            foreach (var element in MineField)
            {
                if (!element.IsMine && !element.IsRevealed)
                    return false;
            }
            GameStatusMessage = "Fedt! Du fandt alle minerne!";
            GameOverEvent?.Invoke(this, GameStatusMessage);
            return true;
        }


        private (int, int) GetPosition(MineFieldElement mineFieldElement)
        {
            for (int i = 0; i < MineField.GetLength(0); i++)
            {
                for (int j = 0; j < MineField.GetLength(1); j++)
                {
                    if (MineField[i, j] == mineFieldElement)
                        return (i, j);
                }
            }

            throw new Exception("MineFieldElement not found in MineField.");
        }

        public int MinesRemaining
        {
            get
            {
                int flaggedCount = MineFieldElements.Count(m => m.Flagged);
                return TotalMines - flaggedCount;
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
