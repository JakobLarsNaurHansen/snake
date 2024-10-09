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
        private const int SnakeSquareSize = 20; // Size of one square in the grid (pixels)
        private List<Point> _snakeParts; // Holds the position of each snake part
        private Point _foodPosition; // Position of the food
        private Timer _gameTimer; // Timer to control the game loop
        private Direction _currentDirection; // Snake's current movement direction

        // Enum to represent direction
        private enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

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

            // Add the canvas to the window
            this.Content = gameArea;

            GameArea = gameArea; 
            _snakeParts = new List<Point>
            {
                new Point(100, 100), // Starting point of the snake
            };
            Console.WriteLine($"Snake position: {_snakeParts[0]}");
            _foodPosition = GetRandomFoodPosition();
            _currentDirection = Direction.Right;
            // Set up the game timer
            _gameTimer = new Timer();
            _gameTimer.Interval = 100; // Update every 100 milliseconds
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
                    if (_currentDirection != Direction.Down) // Prevent 180-degree turns
                        _currentDirection = Direction.Up;
                    break;
                case Avalonia.Input.Key.Down:
                    if (_currentDirection != Direction.Up)
                        _currentDirection = Direction.Down;
                    break;
                case Avalonia.Input.Key.Left:
                    if (_currentDirection != Direction.Right)
                        _currentDirection = Direction.Left;
                    break;
                case Avalonia.Input.Key.Right:
                    if (_currentDirection != Direction.Left)
                        _currentDirection = Direction.Right;
                    break;
            }
        }

        private Point GetRandomFoodPosition()
        {
            Random rand = new Random();
            int x = rand.Next(0, 400 / SnakeSquareSize) * SnakeSquareSize;
            int y = rand.Next(0, 400 / SnakeSquareSize) * SnakeSquareSize;
            return new Point(x, y);
        }

        private void Eat()
        {
            // Add a new part to the snake
            Point tail = _snakeParts[_snakeParts.Count - 1];
            _snakeParts.Add(new Point(tail.X, tail.Y));
            // Generate a new food position
            _foodPosition = GetRandomFoodPosition();
        }
        private bool CollisionOccured()
        {
            // Check for collisions with the snake's body
            for (int i = 1; i < _snakeParts.Count; i++)
            {
                if (_snakeParts[i] == _snakeParts[0])
                {
                    return true;
                }
            }
            return false;
        }
        private void DrawGameOver()
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


        private void GameOver()
        {
            _gameTimer.Stop();
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                DrawGameOver();
            });
            
        }

        private void DrawGame()
        {  
            GameArea.Children.Clear();
            
            if(_snakeParts[0] == _foodPosition) {Eat();}
            
            // Draw the snake
            foreach (var part in _snakeParts)
            {
                var snakePart = new Rectangle()
                {
                    Fill = Brushes.Green, // Use a visible color for debugging
                    Width = SnakeSquareSize,
                    Height = SnakeSquareSize,
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
                Width = SnakeSquareSize,
                Height = SnakeSquareSize,
                
            };

            Canvas.SetLeft(food, _foodPosition.X);
            Canvas.SetTop(food, _foodPosition.Y);
            GameArea.Children.Add(food);
        }


        private void OnGameTick(object sender, ElapsedEventArgs e)
        {
            // Move the snake based on the current direction
            Point newHeadPosition = _snakeParts[0]; // Get the current head position

            switch (_currentDirection)
            {
                case Direction.Up:
                    newHeadPosition = new Point(newHeadPosition.X, newHeadPosition.Y - SnakeSquareSize);
                    break;
                case Direction.Down:
                    newHeadPosition = new Point(newHeadPosition.X, newHeadPosition.Y + SnakeSquareSize);
                    break;
                case Direction.Left:
                    newHeadPosition = new Point(newHeadPosition.X - SnakeSquareSize, newHeadPosition.Y);
                    break;
                case Direction.Right:
                    newHeadPosition = new Point(newHeadPosition.X + SnakeSquareSize, newHeadPosition.Y);
                    break;
            }
            // Loop through walls 
            if (newHeadPosition.X < 0) newHeadPosition = new Point(400 - SnakeSquareSize, newHeadPosition.Y);
            if (newHeadPosition.X >= 400) newHeadPosition = new Point(0, newHeadPosition.Y);
            if (newHeadPosition.Y < 0) newHeadPosition = new Point(newHeadPosition.X, 400 - SnakeSquareSize);
            if (newHeadPosition.Y >= 400) newHeadPosition = new Point(newHeadPosition.X, 0);
            
            

            // Add the new head position to the front of the list (snake moves forward)
            _snakeParts.Insert(0, newHeadPosition);

            // Remove the last part of the snake to simulate movement (unless the snake is growing)
            _snakeParts.RemoveAt(_snakeParts.Count - 1);
            
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
