using System;
using Avalonia;

namespace Snake;

public class Food
{
    private enum TypeOfFood
    {
        Normal,
        Special
    }
    public Point FoodPosition; // Position of the food
    private TypeOfFood _foodType;

    public Food()
    {
       SpawnFood(); 
    }

    public Point GetRandomFoodPosition()
    {
        Random rand = new Random();
        int x = rand.Next(0, 400/Square.SquareSize)*Square.SquareSize;
        int y = rand.Next(0, 400/Square.SquareSize)*Square.SquareSize;
        return new Point(x, y);
    }

    public void Eat(Snake snake)
    {
        if (_foodType==TypeOfFood.Normal)
        {
            Point tail = snake.SnakeParts[snake.SnakeParts.Count - 1];
            snake.SnakeParts.Add(new Point(tail.X, tail.Y)); 
        }
        else if (_foodType==TypeOfFood.Special)
        {
            Point tail = snake.SnakeParts[snake.SnakeParts.Count - 1];
            snake.SnakeParts.Add(new Point(tail.X, tail.Y));
            snake.SnakeParts.Add(new Point(tail.X, tail.Y));
        }
        // Add a new part to the snake
        
        // Generate a new food position
        //TODO Change to spawn food and make the types of food different 
        FoodPosition = GetRandomFoodPosition();
    }

    private void SetFoodType()
    {
        Random rand = new Random();
        int x = rand.Next(0, 2);
        if (x == 0)
        {
            _foodType = TypeOfFood.Normal;
        }
        else
        {
            _foodType = TypeOfFood.Special;
        }
    }

    private void SpawnFood()
    {
        SetFoodType();
        FoodPosition = GetRandomFoodPosition();
    }
}