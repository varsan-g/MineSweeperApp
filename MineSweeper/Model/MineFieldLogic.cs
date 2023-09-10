//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Runtime.CompilerServices;

//namespace MineSweeper.Model
//{
//    public class MineFieldLogic
//    {
//        public MineFieldElement[,] MineField { get; private set; }

//        private const int TotalMines = 5;  // The total number of mines
//        public MineFieldLogic()
//        {
//            InitializeMineField();
//        }

//        private void InitializeMineField()
//        {
//            MineField = new MineFieldElement[10, 5];
//            for (int i = 0; i < 10; i++)
//            {
//                for (int j = 0; j < 5; j++)
//                {
//                    MineField[i, j] = new MineFieldElement();
//                }
//            }

//            GenerateMines(TotalMines);
//        }

//        private void GenerateMines(int numberOfMines)
//        {
//            Random random = new Random();
//            for (int i = 0; i < numberOfMines; i++)
//            {
//                int x, y;
//                do
//                {
//                    x = random.Next(0, MineField.GetLength(0)); // Get random x coordinate
//                    y = random.Next(0, MineField.GetLength(1)); // Get random y coordinate
//                } while (MineField[x, y].IsMine); // Keep getting new coordinates if the cell already contains a mine

//                MineField[x, y].IsMine = true; // Set the cell to be a mine
//            }
//        }

//        public int MinesRemaining
//        {
//            get
//            {
//                int flaggedCount = MineFieldElements.Count(m => m.Flagged);
//                return TotalMines - flaggedCount;
//            }
//        }

//        private bool CheckForWin()
//        {
//            foreach (var element in MineField)
//            {
//                if (!element.IsMine && !element.IsRevealed)
//                    return false;
//            }
//            GameStatusMessage = "Fedt! Du fandt alle minerne!";
//            GameOverEvent?.Invoke(this, GameStatusMessage);
//            return true;
//        }


//        private (int, int) GetPosition(MineFieldElement mineFieldElement)
//        {
//            for (int i = 0; i < MineField.GetLength(0); i++)
//            {
//                for (int j = 0; j < MineField.GetLength(1); j++)
//                {
//                    if (MineField[i, j] == mineFieldElement)
//                        return (i, j);
//                }
//            }

//            throw new Exception("MineFieldElement not found in MineField.");
//        }

//        private int CalculateNeighboringMines(int x, int y)
//        {
//            int mineCount = 0;

//            for (int i = -1; i <= 1; i++)
//            {
//                for (int j = -1; j <= 1; j++)
//                {
//                    // Skip the square itself
//                    if (i == 0 && j == 0)
//                        continue;

//                    int neighborX = x + i;
//                    int neighborY = y + j;

//                    // Skip squares outside the minefield
//                    if (neighborX < 0 || neighborY < 0 || neighborX >= MineField.GetLength(0) || neighborY >= MineField.GetLength(1))
//                        continue;

//                    if (MineField[neighborX, neighborY].IsMine)
//                        mineCount++;
//                }
//            }

//            return mineCount;
//        }

//        private void RevealNeighbors(int x, int y)
//        {
//            for (int i = -1; i <= 1; i++)
//            {
//                for (int j = -1; j <= 1; j++)
//                {
//                    // Skip the square itself
//                    if (i == 0 && j == 0)
//                        continue;

//                    int neighborX = x + i;
//                    int neighborY = y + j;

//                    // Skip squares outside the minefield
//                    if (neighborX < 0 || neighborY < 0 || neighborX >= MineField.GetLength(0) || neighborY >= MineField.GetLength(1))
//                        continue;

//                    var neighbor = MineField[neighborX, neighborY];

//                    // Skip if the neighbor is already revealed or flagged
//                    if (neighbor.IsRevealed || neighbor.Flagged)
//                        continue;

//                    // Reveal the neighbor
//                    neighbor.IsRevealed = true;

//                    neighbor.NeighboringMines = CalculateNeighboringMines(neighborX, neighborY);

//                    // If the neighbor has no neighboring mines, reveal its neighbors recursively
//                    if (neighbor.NeighboringMines == 0)
//                    {
//                        RevealNeighbors(neighborX, neighborY);
//                    }
//                }
//            }
//        }

//        public event PropertyChangedEventHandler PropertyChanged;
//        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
//        {
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//        }

//        public IEnumerable<MineFieldElement> MineFieldElements
//        {
//            get
//            {
//                return _mineFieldLogic.MineField.Cast<MineFieldElement>();
//            }
//        }

//    }
//}