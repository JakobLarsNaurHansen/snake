using System.Collections.Generic;
using Avalonia;

namespace Snake;

public class Snake
{
   public List<Point> SnakeParts; // Holds the position of each snake part
   public Direction CurrentDirection;
   public int Score;

   public Snake(List<Point> snakeParts)
   {
      SnakeParts = snakeParts;
   }

   public void InitSnake()
   {
      SnakeParts = new List<Point>
      {
         new Point(100, 100), // Starting point of the snake
      };
      CurrentDirection = Direction.Right;
      Score = 0;
   }
   
}