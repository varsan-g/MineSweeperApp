using MineSweeper.Model;
using MineSweeper.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;

namespace MineSweeper
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;

            _viewModel.MineFieldLogic.PropertyChanged += MineFieldLogic_PropertyChanged;
        }

        private void MineFieldLogic_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MineFieldLogic.GameStatusMessage))
            {
                MessageBox.Show(_viewModel.MineFieldLogic.GameStatusMessage, "Game Status", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void MineFieldButton_RightClick(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is Button button))
                return;

            var mineFieldElement = (MineFieldElement)button.DataContext;
            _viewModel.MineFieldRightClickCommand.Execute(mineFieldElement);

            e.Handled = true;
        }
    }
}