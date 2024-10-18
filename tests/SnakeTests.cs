using System.Collections.Generic;
using Avalonia;
using Xunit;

namespace Snake.Tests
{
    public class SnakeTests
    {
        [Fact]
        public void InitSnake_ShouldInitializeSnakeParts()
        {
            // Arrange
            var snake = new Snake(new List<Point>());

            // Act
            snake.InitSnake();

            // Assert
            Assert.Single(snake.SnakeParts);
            Assert.Equal(new Point(100, 100), snake.SnakeParts[0]);
        }

        [Fact]
        public void InitSnake_ShouldSetCurrentDirectionToRight()
        {
            // Arrange
            var snake = new Snake(new List<Point>());

            // Act
            snake.InitSnake();

            // Assert
            Assert.Equal(Direction.Right, snake.CurrentDirection);
        }

        [Fact]
        public void InitSnake_ShouldSetScoreToZero()
        {
            // Arrange
            var snake = new Snake(new List<Point>());

            // Act
            snake.InitSnake();

            // Assert
            Assert.Equal(0, snake.Score);
        }
    }
}