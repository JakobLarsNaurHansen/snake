using System;
using System.Linq;
using Avalonia;
using Xunit;

namespace Snake.tests
{
    public class FoodTests
    {
        [Fact]
        public void Food_ShouldNotSpawnInsideSnake()
        {
            // Arrange
            var snake = new Snake(new System.Collections.Generic.List<Point>
            {
                new Point(0, 0),
                new Point(10, 0),
                new Point(20, 0)
            });

            // Act
            var food = new Food(snake);

            // Assert
            Assert.DoesNotContain(food.FoodPosition, snake.SnakeParts);
        }

        [Fact]
        public void Eat_ShouldIncreaseSnakeLength()
        {
            // Arrange
            var snake = new Snake(new System.Collections.Generic.List<Point>
            {
                new Point(0, 0),
                new Point(10, 0),
                new Point(20, 0)
            });
            var food = new Food(snake) { FoodType = Food.TypeOfFood.Normal };

            // Act
            food.Eat(snake);

            // Assert
            Assert.Equal(4, snake.SnakeParts.Count);
        }

        [Fact]
        public void Eat_SpecialFood_ShouldIncreaseSnakeLengthByFive()
        {
            // Arrange
            var snake = new Snake(new System.Collections.Generic.List<Point>
            {
                new Point(0, 0),
                new Point(10, 0),
                new Point(20, 0)
            });
            var food = new Food(snake) { FoodType = Food.TypeOfFood.Special };

            // Act
            food.Eat(snake);

            // Assert
            Assert.Equal(8, snake.SnakeParts.Count);
        }

        [Fact]
        public void SetFoodType_ShouldSetSpecialFoodWith10PercentChance()
        {
            // Arrange
            var snake = new Snake(new System.Collections.Generic.List<Point>());
            var food = new Food(snake);

            // Act
            int specialFoodCount = 0;
            for (int i = 0; i < 1000; i++)
            {
                food.SetFoodType();
                if (food.FoodType == Food.TypeOfFood.Special)
                {
                    specialFoodCount++;
                }
            }

            // Assert
            Assert.InRange(specialFoodCount, 50, 150); // 10% of 1000 is 100, allowing some variance
        }
    }
}