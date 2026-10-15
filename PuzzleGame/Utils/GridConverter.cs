using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PuzzleGame.Utils.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace PuzzleGame.Utils;

public class GridConverter
{
    private readonly int _cellSize;
    
    public GridConverter(int cellSize)
    {
        _cellSize = cellSize;
    }

    public GridPosition PixelToIndex(Point location)
    {
        int column = (int)Math.Round((float)location.Y / _cellSize) - 1;
        int row = (int)Math.Round((float)location.X / _cellSize) - 1;
        
        return new GridPosition(column, row);//to store the values as if there were in an array 2D, column must go first
    }

    //THIS HAS NOT BEEN USED YET; NOT SURE IF IT IS WORKING AS INTENDED
    public GridPosition IndexToPixel(Point location)
    {
        int rowInGrid = location.X + 1;// location.X = 0 -> 1
        int columnInGrid = location.Y + 1;// location.Y = 2 -> 3
        
        int row = rowInGrid * _cellSize;//96 -> first row
        int column = columnInGrid * _cellSize;// 384 -> third column
        
        return new GridPosition(row, column);
    }

    //Return the range available for movement
    public GridPosition ConvertCollisionBounds(int x, int y, int length)
    {
        int pixelX = x == -1 ? -1 : (x + 2) * _cellSize; //x is topright, to get the pixel boundary +2 to the grid position is needed (+1 for roombounds and +1 to blocker body size)
        
        Console.WriteLine("pixelX: " + pixelX);
        Console.WriteLine("X: " + x);
        
        if (y == -1)
        {
            return new GridPosition(pixelX, y);
        }
        if (length > 1)
        {
            return new GridPosition(pixelX, y * _cellSize - (length - 1) * _cellSize);
        }
        return new GridPosition(pixelX, y * _cellSize);
    }
}

