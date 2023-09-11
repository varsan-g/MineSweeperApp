using MineSweeper.Commands;
using MineSweeper.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;

namespace MineSweeper.ViewModel
{
    /* TODO:
     * - Flyt MainWindow til View-folder
     * - Split logic fra MainViewModel ind i "MineFieldLogic" og "GameLogic" i Model, da der ikke bliver fulgt SRP lige nu. MineFieldLogic skal indeholde metoderne til at initialisere MineField.
     * - Tilføj 'Genstart' knap
     * - Første MineField man klikker på skal være 'safe' - !isMine
     * - Formatér score - indeholder ms i UI efter GameOver() bliver kaldet
     * - Difficulty modes - easy 3 miner, medium 8 miner, hard 15 miner
     */
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private Stopwatch stopwatch;
        private DispatcherTimer timer;
        public ICommand MineFieldButtonClick { get; private set; }
        public ICommand MineFieldRightClickCommand { get; private set; }

        public delegate void GameOverEventHandler(object sender, string e);
        public event GameOverEventHandler GameOverEvent;

        public MineFieldElement[,] MineField { get; set; }

        private const int TotalMines = 5;  // Totale mængde af miner

        public MainWindowViewModel()
        {
            MineFieldButtonClick = new MineFieldButtonClickCommand(OnMineFieldButtonClick);
            MineFieldRightClickCommand = new MineFieldRightClickCommand(OnMineFieldRightClick);
            InitializeMineField();
            stopwatch = new Stopwatch();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;

            timer.Start();
            stopwatch.Start();
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
                    x = random.Next(0, MineField.GetLength(0)); 
                    y = random.Next(0, MineField.GetLength(1)); 
                } while (MineField[x, y].IsMine); 

                MineField[x, y].IsMine = true; 
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
                
                mineFieldElement.IsRevealed = true;

                
                GameOver();
            }
            else
            {
                mineFieldElement.IsRevealed = true;

                (int x, int y) = GetPosition(mineFieldElement);

                mineFieldElement.NeighboringMines = CalculateNeighboringMines(x, y);

                if (mineFieldElement.NeighboringMines == 0)
                {
                    RevealNeighbors(x, y);
                }
            }
        }

        private void RevealNeighbors(int x, int y)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;

                    int neighborX = x + i;
                    int neighborY = y + j;

                    if (neighborX < 0 || neighborY < 0 || neighborX >= MineField.GetLength(0) || neighborY >= MineField.GetLength(1))
                        continue;

                    var neighbor = MineField[neighborX, neighborY];

                    if (neighbor.IsRevealed || neighbor.Flagged)
                        continue;

                    neighbor.IsRevealed = true;

                    neighbor.NeighboringMines = CalculateNeighboringMines(neighborX, neighborY);

                    if (neighbor.NeighboringMines == 0)
                    {
                        RevealNeighbors(neighborX, neighborY);
                    }
                }
            }
        }


        private void GameOver()
        {
            foreach (var mineFieldElement in MineFieldElements)
            {
                if (mineFieldElement.IsMine)
                {
                    mineFieldElement.IsRevealed = true;
                }
            }
            timer.Stop();
            stopwatch.Stop();
            TimeSpan score = stopwatch.Elapsed;

            Score = score;

            GameStatusMessage = "Game Over! Du klikkede på en mine!";

            GameOverEvent?.Invoke(this, GameStatusMessage);

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
                    if (i == 0 && j == 0)
                        continue;

                    int neighborX = x + i;
                    int neighborY = y + j;

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

            timer.Stop();
            stopwatch.Stop();

            TimeSpan score = stopwatch.Elapsed;

            Score = score;

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

            throw new Exception("MineFieldElement kunne ikke findes.");
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

        private TimeSpan _score;
        public TimeSpan Score
        {
            get { return _score; }
            set
            {
                if (_score != value)
                {
                    _score = value;
                    OnPropertyChanged(nameof(Score));
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsed = stopwatch.Elapsed;

            TimeSpan score = new TimeSpan(elapsed.Hours, elapsed.Minutes, elapsed.Seconds);

            Score = score;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
