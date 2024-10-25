using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using Microsoft.Extensions.Configuration;

namespace Snake
{
    public partial class MainWindow : Window
    {
        private Queue<Direction> _moveQueue;
        private readonly DatabaseService _databaseService;
        public MainWindow()
        {
            InitializeComponent();
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            _databaseService = new DatabaseService(config);
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
            _moveQueue = new Queue<Direction>();

            _gameTimer = new Timer();
            _gameTimer.Interval = 100;
            _gameTimer.Elapsed += OnGameTick;
            _gameTimer.Start();
        }
        private void RestartGame()
        {
            _snake = new Snake(new List<Point>());
            _snake.InitSnake();
            _food = new Food(_snake);
            _moveQueue = new Queue<Direction>();
            _gameTimer.Start();
            
        }
    }
}