using System;
using Avalonia;

namespace Snake;

public class Food
{
    public enum TypeOfFood
    {
        Normal,
        Special
    }
    public Point FoodPosition; // Position of the food
    public TypeOfFood FoodType;

    public Food(Snake snake)
    {
       SpawnFood(snake); 
    }

    private Point GetRandomFoodPosition(Snake snake)
    {
        Random rand = new Random();
        int x = rand.Next(0, 400/Square.SquareSize)*Square.SquareSize;
        int y = rand.Next(0, 400/Square.SquareSize)*Square.SquareSize;
        while (snake.SnakeParts.Contains(new Point(x, y)))
        {
            x = rand.Next(0, 400/Square.SquareSize)*Square.SquareSize;
            y = rand.Next(0, 400/Square.SquareSize)*Square.SquareSize;
        }
        return new Point(x, y);
    }

    public void Eat(Snake snake)
    {
        if (FoodType==TypeOfFood.Normal)
        {
            Point tail = snake.SnakeParts[^1];
            snake.SnakeParts.Add(new Point(tail.X, tail.Y)); 
        }
        else if (FoodType==TypeOfFood.Special)
        {
            Point tail = snake.SnakeParts[^1];
            for(int i = 0; i < 5; i++)
                snake.SnakeParts.Add(new Point(tail.X, tail.Y));
        }
        
        SpawnFood(snake);
    }

    internal void SetFoodType()
    {
        Random rand = new Random();
        int x = rand.Next(0, 10); // 10% chance of special food
        FoodType = x == 0 ? TypeOfFood.Special : TypeOfFood.Normal;
    }

    private void SpawnFood(Snake snake)
    {
        
        SetFoodType();
        FoodPosition = GetRandomFoodPosition(snake);
    }
}