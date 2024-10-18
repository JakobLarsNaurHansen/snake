using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Styling;

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
    }
}