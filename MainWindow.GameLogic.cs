using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Snake
{
    public partial class MainWindow : Window
    {
        private void OnGameTick(object? sender, ElapsedEventArgs e)
        {
            if (_moveQueue.Count > 0)
            {
                _snake.CurrentDirection = _moveQueue.Dequeue();
            }
            List<Point> oldPositions = new List<Point>(_snake.SnakeParts);
            Point newHeadPosition = oldPositions[0];

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

            if (_snake.SnakeParts[0] == _food.FoodPosition)
            {
                if (_food.FoodType == Food.TypeOfFood.Special)
                    _snake.Score += 5;
                else
                    _snake.Score++;
                _food.Eat(_snake);
            }

            _snake.SnakeParts.Insert(0, newHeadPosition);
            _snake.SnakeParts.RemoveAt(_snake.SnakeParts.Count - 1);

            if (CollisionOccured())
            {
                GameOver();
            }

            Dispatcher.UIThread.InvokeAsync(() =>
            {
                DrawGame();
                for (int i = 0; i < _snake.SnakeParts.Count; i++)
                {
                    AnimateSnakePartMovement(oldPositions[i], _snake.SnakeParts[i], (Rectangle)GameArea.Children[i]);
                }

                // Reset positions after animation
                for (int i = 0; i < _snake.SnakeParts.Count; i++)
                {
                    Canvas.SetLeft(GameArea.Children[i], _snake.SnakeParts[i].X);
                    Canvas.SetTop(GameArea.Children[i], _snake.SnakeParts[i].Y);
                }
            });
        }

        private bool CollisionOccured()
        {
            for (int i = 1; i < _snake.SnakeParts.Count; i++)
            {
                if (_snake.SnakeParts[i] == _snake.SnakeParts[0])
                {
                    return true;
                }
            }
            return false;
        }

        private void GameOver()
        {
            _gameTimer.Stop();
            Dispatcher.UIThread.InvokeAsync(DrawGameOver);
        }

        private void DrawGameOver()
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                var gameOverText = new TextBlock
                {
                    Text = "Game Over",
                    Foreground = Brushes.Black,
                    FontSize = 24,
                    FontWeight = FontWeight.Bold
                };
                var score = new TextBlock
                {
                    Text = $"Score: {_snake.Score}",
                    Foreground = Brushes.Black,
                    FontSize = 16,
                    FontWeight = FontWeight.Bold
                };
                var playAgain = new TextBlock
                {
                    Text = "Press \"R\" to play again",
                    Foreground = Brushes.Black,
                    FontSize = 16,
                    FontWeight = FontWeight.Bold
                };
                var namePromt = new TextBlock
                {
                    Text = "Enter your name:",
                    Foreground = Brushes.Black,
                    FontSize = 16,
                    FontWeight = FontWeight.Bold
                };
                var nameInput = new TextBox
                {
                    Width = 200,
                    Background = Brushes.White,
                    Foreground = Brushes.Black,
                    FontSize = 16,
                    FontWeight = FontWeight.Bold
                };
                var submitButton = new Button
                {
                    Content = "Submit",
                    Width = 100,
                    Background = Brushes.White,
                    Foreground = Brushes.Black,
                    FontSize = 16,
                    FontWeight = FontWeight.Bold
                };
                
                submitButton.Click += (sender, e) =>
                {
                    _snake.Name = nameInput.Text;
                    if (_snake.Name != null) PostScore(_snake.Name, _snake.Score);
                };
                double centerX = (GameArea.Width / 2) - 50;
                double centerY = (GameArea.Height / 2) - 50;

                Canvas.SetLeft(gameOverText, centerX);
                Canvas.SetTop(gameOverText, centerY);

                Canvas.SetLeft(score, centerX);
                Canvas.SetTop(score, centerY + 35);

                Canvas.SetLeft(playAgain, centerX);
                Canvas.SetTop(playAgain, centerY + 75);
                
                Canvas.SetLeft(namePromt, centerX);
                Canvas.SetTop(namePromt, centerY + 115);
                
                Canvas.SetLeft(nameInput, centerX);
                Canvas.SetTop(nameInput, centerY + 145);
                
                Canvas.SetLeft(submitButton, centerX);
                Canvas.SetTop(submitButton, centerY + 175);

                GameArea.Children.Add(gameOverText);
                GameArea.Children.Add(score);
                GameArea.Children.Add(playAgain);
                GameArea.Children.Add(namePromt);
                GameArea.Children.Add(nameInput);
                GameArea.Children.Add(submitButton);

                GameArea.InvalidateVisual();
            });
        }
        private void PostScore(string? username, int score)
        {
        try
        { 
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build(); 
            var databaseName = config["PostgreSql:DatabaseName"];
            var databaseUser = config["PostgreSql:User"];
            var databasePassword = config["PostgreSql:Password"];
            var databaseHost = config["PostgreSql:Host"];
            var databasePort = config["PostgreSql:Port"];

            if (string.IsNullOrEmpty(databaseName) || string.IsNullOrEmpty(databaseUser) ||
                string.IsNullOrEmpty(databasePassword) || string.IsNullOrEmpty(databaseHost) ||
                string.IsNullOrEmpty(databasePort))
            {
                throw new InvalidOperationException("One or more database configuration values are not set.");
            }

            string connectionString = $"Host={databaseHost};Port={databasePort};Username={databaseUser};Password={databasePassword};Database={databaseName}";

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = "INSERT INTO \x0068ighscore (UserName, Score) VALUES (@username, @score)";
                    cmd.Parameters.AddWithValue("username", username ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("score", score);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex) 
        {
            Console.WriteLine($"Error posting high score: {ex.Message}");
        }
        }
    }
}