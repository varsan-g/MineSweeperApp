using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.Model
{
    public class MineFieldElement : INotifyPropertyChanged
    {
        private bool _isMine;
        private bool _isRevealed;
        private int _neighboringMines;

        public bool IsMine
        {
            get { return _isMine; }
            set
            {
                if (_isMine != value)
                {
                    _isMine = value;
                    OnPropertyChanged(nameof(IsMine));
                    OnPropertyChanged(nameof(IsMineAndRevealed));
                    OnPropertyChanged(nameof(IsRevealedAndNotMine));

                }
            }
        }

        public bool IsRevealed
        {
            get { return _isRevealed; }
            set
            {
                if (_isRevealed != value)
                {
                    _isRevealed = value;
                    OnPropertyChanged(nameof(IsRevealed));
                    OnPropertyChanged(nameof(IsMineAndRevealed));
                    OnPropertyChanged(nameof(IsRevealedAndNotMine));

                }
            }
        }

        public bool IsMineAndRevealed
        {
            get { return IsMine && IsRevealed; }
        }

        public bool IsRevealedAndNotMine
        {
            get { return !IsMine && IsRevealed; }
        }


        private bool _flagged;
        public bool Flagged
        {
            get { return _flagged; }
            set
            {
                if (_flagged != value)
                {
                    _flagged = value;
                    OnPropertyChanged(nameof(Flagged));
                }
            }
        }

        public int NeighboringMines
        {
            get { return _neighboringMines; }
            set
            {
                if (_neighboringMines != value)
                {
                    _neighboringMines = value;
                    OnPropertyChanged(nameof(NeighboringMines));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
