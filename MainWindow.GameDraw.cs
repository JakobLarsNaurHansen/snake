using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Styling;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Snake
{
    public partial class MainWindow : Window
    {
        
        private void DrawGame()
        {
            GameArea.Children.Clear();
            foreach (var part in _snake.SnakeParts)
            {
                var snakePart = new Rectangle()
                {
                    Fill = Brushes.Green,
                    Width = Square.SquareSize,
                    Height = Square.SquareSize,
                    RadiusX = 6,
                    RadiusY = 6
                };
                Canvas.SetLeft(snakePart, part.X);
                Canvas.SetTop(snakePart, part.Y);
                GameArea.Children.Add(snakePart);
            }

            var food = new Rectangle
            {
                Fill = GetFoodColor(_food.FoodType),
                Width = Square.SquareSize,
                Height = Square.SquareSize,
                RadiusX = 8,
                RadiusY = 8
            };

            Canvas.SetLeft(food, _food.FoodPosition.X);
            Canvas.SetTop(food, _food.FoodPosition.Y);
            GameArea.Children.Add(food);

            var scoreText = new TextBlock
            {
                Text = $"Score: {_snake.Score}",
                Foreground = Brushes.Black,
                FontSize = 16,
                FontWeight = FontWeight.Bold
            };
            GameArea.Children.Add(scoreText);
        }

        private void AnimateSnakePartMovement(Point oldPosition, Point newPosition, Rectangle snakePart)
        {
            double oldX = oldPosition.X;
            double oldY = oldPosition.Y;
            double newX = newPosition.X;
            double newY = newPosition.Y;

            // Handle horizontal looping
            if (Math.Abs(oldX - newX) > 200)
            {
                if (oldX > newX)
                {
                    newX += 400;
                }
                else
                {
                    oldX += 400;
                }
            }

            // Handle vertical looping
            if (Math.Abs(oldY - newY) > 200)
            {
                if (oldY > newY)
                {
                    newY += 400;
                }
                else
                {
                    oldY += 400;
                }
            }

            var animation = new Animation
            {
                Duration = TimeSpan.FromMilliseconds(100),
                Easing = new SineEaseInOut(),
                Children =
                {
                    new KeyFrame
                    {
                        Cue = new Cue(0),
                        Setters =
                        {
                            new Setter(Canvas.LeftProperty, oldX),
                            new Setter(Canvas.TopProperty, oldY)
                        }
                    },
                    new KeyFrame
                    {
                        Cue = new Cue(1),
                        Setters =
                        {
                            new Setter(Canvas.LeftProperty, newX),
                            new Setter(Canvas.TopProperty, newY)
                        }
                    }
                }
            };
            animation.RunAsync(snakePart);
        }

        private IBrush GetFoodColor(Food.TypeOfFood foodType)
        {
            return foodType switch
            {
                Food.TypeOfFood.Normal => Brushes.Red,
                Food.TypeOfFood.Special => Brushes.Gold,
                _ => Brushes.Red
            };
        }
        private void DrawGameOver()
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                DrawTopScores();
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
                    Width = 150,
                    Foreground = Brushes.White,
                    Background = Brushes.Black,
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
                    if (_snake.Name != null) _databaseService.PostScore(_snake.Name, _snake.Score);
                    submitButton.IsEnabled = false;
                    ClearTopScores();
                    DrawTopScores();
                    
                };
                double centerX = (GameArea.Width / 2) - 180;
                double centerY = (GameArea.Height / 2) - 150;

                Canvas.SetLeft(gameOverText, centerX);
                Canvas.SetTop(gameOverText, centerY);

                Canvas.SetLeft(score, centerX);
                Canvas.SetTop(score, centerY + 30);

                Canvas.SetLeft(playAgain, centerX);
                Canvas.SetTop(playAgain, centerY + 60);
                
                Canvas.SetLeft(namePromt, centerX);
                Canvas.SetTop(namePromt, centerY + 100);
                
                Canvas.SetLeft(nameInput, centerX);
                Canvas.SetTop(nameInput, centerY + 130);
                
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
        private void ClearTopScores()
        {
            // Filter elements with the "TopScore" tag and remove them from GameArea
            foreach (var element in GameArea.Children.ToList())
            {
                if (element is Control control && control.Tag?.ToString() == "TopScore")
                {
                    GameArea.Children.Remove(control);
                }
            }

            GameArea.InvalidateVisual();
        }


        private void DrawTopScores()
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                var topScores = _databaseService.GetTopScores();
                var topScoresText = new TextBlock
                {
                    Text = "Top Scores",
                    Foreground = Brushes.Black,
                    FontSize = 24,
                    FontWeight = FontWeight.Bold,
                    Tag = "TopScore" 
                };
                double centerX = (GameArea.Width / 2) + 20;
                double centerY = (GameArea.Height / 2) - 150;

                Canvas.SetLeft(topScoresText, centerX);
                Canvas.SetTop(topScoresText, centerY);
                GameArea.Children.Add(topScoresText);

                for (int i = 0; i < topScores.Count; i++)
                {
                    var scoreText = new TextBlock
                    {
                        Text = $"{topScores[i].UserName}: {topScores[i].Score}",
                        Foreground = Brushes.Black,
                        FontSize = 16,
                        FontWeight = FontWeight.Bold,
                        Tag = "TopScore" 
                    };
                    Canvas.SetLeft(scoreText, centerX);
                    Canvas.SetTop(scoreText, centerY + 30 + (i * 30));
                    GameArea.Children.Add(scoreText);
                }

                GameArea.InvalidateVisual();
            });
        }

    }
}