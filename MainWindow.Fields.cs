// MainWindow.Fields.cs
using System.Timers;
using Avalonia.Controls;

namespace Snake
{
    public partial class MainWindow : Window
    {
        private Snake _snake;
        private Food _food;
        private Timer _gameTimer;
    }
}