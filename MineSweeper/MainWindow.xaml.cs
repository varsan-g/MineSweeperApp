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
        public MainWindow()
        {
            InitializeComponent();
            MainWindowViewModel vm = new MainWindowViewModel();
            DataContext = vm;

            var viewModel = DataContext as MainWindowViewModel;
            viewModel.GameOverEvent += ViewModel_GameOverEvent;
        }
        private void ViewModel_GameOverEvent(object sender, string e)
        {
            MessageBox.Show(e, "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MineFieldButton_RightClick(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is Button button))
                return;

            var mineFieldElement = (MineFieldElement)button.DataContext;

            ((MainWindowViewModel)DataContext).MineFieldRightClickCommand.Execute(mineFieldElement);

            e.Handled = true;
        }
    }
}