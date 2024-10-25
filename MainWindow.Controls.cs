using Avalonia.Controls;
using Avalonia.Input;

namespace Snake
{
    public partial class MainWindow : Window
    {
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.Key)
            {
                case Key.P:
                    if (_gameTimer.Enabled)
                        _gameTimer.Stop();
                    else _gameTimer.Start();
                    break;
                case Key.R:
                    RestartGame();
                    break;
                case Key.Q:
                    Close();
                    break;
            }
            Direction? newDirection = null;
            switch (e.Key)
            {
                case Key.Up:
                    if (_snake.CurrentDirection != Direction.Down)
                        newDirection = Direction.Up;
                    break;
                case Key.Down:
                    if (_snake.CurrentDirection != Direction.Up)
                        newDirection = Direction.Down;
                    break;
                case Key.Left:
                    if (_snake.CurrentDirection != Direction.Right)
                        newDirection = Direction.Left;
                    break;
                case Key.Right:
                    if (_snake.CurrentDirection != Direction.Left)
                        newDirection = Direction.Right;
                    break;
            }
            if (newDirection.HasValue)
            {
                _moveQueue.Enqueue(newDirection.Value);
            }
        }
    } 
}