using System.Collections.Generic;
using Avalonia;

namespace Snake;

public class Snake(List<Point> snakeParts)
{
   public List<Point> SnakeParts = snakeParts; // Holds the position of each snake part
   public Direction CurrentDirection;
   public int Score;
   public string Name;

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