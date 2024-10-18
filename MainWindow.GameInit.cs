using System;
using System.Collections.Generic;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;

namespace Snake
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitGame();
            Console.WriteLine("Initialized game");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        private void InitGame()
        {
            Canvas gameArea = new Canvas
            {
                Background = Brushes.LightGray,
                Width = 400,
                Height = 400
            };

            Content = gameArea;
            GameArea = gameArea;
            _snake = new Snake(new List<Point>());
            _snake.InitSnake();
            _food = new Food(_snake);

            _isRendered = true;
            _gameTimer = new Timer();
            _gameTimer.Interval = 100;
            _gameTimer.Elapsed += OnGameTick;
            _gameTimer.Start();
        }

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
                    InitGame();
                    break;
                case Key.Q:
                    Close();
                    break;
            }
            if (!_isRendered) { return; }
            switch (e.Key)
            {
                case Key.Up:
                    if (_snake.CurrentDirection != Direction.Down)
                        _snake.CurrentDirection = Direction.Up;
                    break;
                case Key.Down:
                    if (_snake.CurrentDirection != Direction.Up)
                        _snake.CurrentDirection = Direction.Down;
                    break;
                case Key.Left:
                    if (_snake.CurrentDirection != Direction.Right)
                        _snake.CurrentDirection = Direction.Left;
                    break;
                case Key.Right:
                    if (_snake.CurrentDirection != Direction.Left)
                        _snake.CurrentDirection = Direction.Right;
                    break;
            }

            _isRendered = false;
        }
    }
}