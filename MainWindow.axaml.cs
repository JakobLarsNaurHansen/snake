using System;
using System.Collections.Generic;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;

namespace Snake
{
    public partial class MainWindow : Window
    {
        private Timer _gameTimer; // Timer to control the game loop
        private Snake _snake; // Snake object
        private Food _food; // Food object

        // Enum to represent direction
        

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

        public void InitGame()
        {
            Canvas gameArea = new Canvas
            {
                Background = Brushes.LightGray,
                Width = 400,
                Height = 400
            };

            // Add the canvas to the window
            this.Content = gameArea;

            GameArea = gameArea;
            _snake = new Snake(new List<Point>());
            _snake.InitSnake();
            _food = new Food();
            
            // Set up the game timer
            _gameTimer = new Timer();
            _gameTimer.Interval = 100; 
            _gameTimer.Elapsed += OnGameTick;
            _gameTimer.Start();
           
        }
        protected override void OnKeyDown(Avalonia.Input.KeyEventArgs e)
        {
            base.OnKeyDown(e);

            // Change direction based on user input
            switch (e.Key)
            {
                case Avalonia.Input.Key.Up:
                    if (_snake.CurrentDirection != Direction.Down) // Prevent 180-degree turns
                        _snake.CurrentDirection = Direction.Up;
                    break;
                case Avalonia.Input.Key.Down:
                    if (_snake.CurrentDirection != Direction.Up)
                        _snake.CurrentDirection = Direction.Down;
                    break;
                case Avalonia.Input.Key.Left:
                    if (_snake.CurrentDirection != Direction.Right)
                        _snake.CurrentDirection = Direction.Left;
                    break;
                case Avalonia.Input.Key.Right:
                    if (_snake.CurrentDirection != Direction.Left)
                        _snake.CurrentDirection = Direction.Right;
                    break;
            }
        }

        
        public bool CollisionOccured()
        {
            // Check for collisions with the snake's body
            for (int i = 1; i < _snake.SnakeParts.Count; i++)
            {
                if (_snake.SnakeParts[i] == _snake.SnakeParts[0])
                {
                    return true;
                }
            }
            return false;
        }
        public void DrawGameOver()
//TODO Center the text
        {
            Console.WriteLine("Drawing Game Over screen...");

            Dispatcher.UIThread.InvokeAsync(() =>
            {

                var gameOverText = new TextBlock
                {
                    Text = "Game Over",
                    Foreground = Brushes.Black,
                    FontSize = 24,
                    FontWeight = FontWeight.Bold
                };

                // Manually center the TextBlock
                double centerX = (GameArea.Width / 2) - (gameOverText.Bounds.Width / 2);
                double centerY = (GameArea.Height / 2) - (gameOverText.Bounds.Height / 2);

                // Explicitly set the position using Canvas
                Canvas.SetLeft(gameOverText, centerX);
                Canvas.SetTop(gameOverText, centerY);

                GameArea.Children.Add(gameOverText);

                // Force the UI to update
                GameArea.InvalidateVisual();

                Console.WriteLine("Game Over screen rendered.");
            });
        }


        public void GameOver()
        {
            _gameTimer.Stop();
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                DrawGameOver();
            });
            
        }

        public void DrawGame()
        {  
            GameArea.Children.Clear();
            
            if(_snake.SnakeParts[0] == _food.FoodPosition) {_food.Eat(_snake);}
            
            // Draw the snake
            foreach (var part in _snake.SnakeParts)
            {
                
                var snakePart = new Rectangle()
                {
                    Fill = Brushes.Green, // Use a visible color for debugging
                    Width = Square.SquareSize,
                    Height = Square.SquareSize,
                };

                // Log the positions to confirm they're within bounds
                Canvas.SetLeft(snakePart, part.X);
                Canvas.SetTop(snakePart, part.Y);
                GameArea.Children.Add(snakePart);
            }

            // Draw the food and make it round
            var food = new Rectangle
            {
                Fill = Brushes.Red,
                Width = Square.SquareSize,
                Height = Square.SquareSize,
                
            };

            Canvas.SetLeft(food, _food.FoodPosition.X);
            Canvas.SetTop(food, _food.FoodPosition.Y);
            GameArea.Children.Add(food);
        }


        public void OnGameTick(object sender, ElapsedEventArgs e)
        {
            // Move the snake based on the current direction
            Point newHeadPosition = _snake.SnakeParts[0]; // Get the current head position

            switch (_snake.CurrentDirection)
            {
                case Direction.Up:
                    newHeadPosition = new Point(newHeadPosition.X, newHeadPosition.Y - Square.SquareSize);
                    break;
                case Direction.Down:
                    newHeadPosition = new Point(newHeadPosition.X, newHeadPosition.Y + Square.SquareSize);
                    break;
                case Direction.Left:
                    newHeadPosition = new Point(newHeadPosition.X - Square.SquareSize, newHeadPosition.Y);
                    break;
                case Direction.Right:
                    newHeadPosition = new Point(newHeadPosition.X + Square.SquareSize, newHeadPosition.Y);
                    break;
            }
            // Loop through walls 
            if (newHeadPosition.X < 0) newHeadPosition = new Point(400 - Square.SquareSize, newHeadPosition.Y);
            if (newHeadPosition.X >= 400) newHeadPosition = new Point(0, newHeadPosition.Y);
            if (newHeadPosition.Y < 0) newHeadPosition = new Point(newHeadPosition.X, 400 - Square.SquareSize);
            if (newHeadPosition.Y >= 400) newHeadPosition = new Point(newHeadPosition.X, 0);
            
            

            // Add the new head position to the front of the list (snake moves forward)
            _snake.SnakeParts.Insert(0, newHeadPosition);

            // Remove the last part of the snake to simulate movement (unless the snake is growing)
            _snake.SnakeParts.RemoveAt(_snake.SnakeParts.Count - 1);
            
            // Check for collisions with the snake's body
            if (CollisionOccured()) { GameOver();}

            // Redraw the game (snake and food) after movement
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                DrawGame();
            });

            
        }

        

    }
}
