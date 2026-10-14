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

    public Point PixelToIndex(Point location)
    {
        int column = (int)Math.Round((float)location.Y / _cellSize) - 1;
        int row = (int)Math.Round((float)location.X / _cellSize) - 1;
        
        return new Point(column, row);
    }

    public Point IndexToPixel(Point location)
    {
        int rowInGrid = location.X + 1;// location.X = 0 -> 1
        int columnInGrid = location.Y + 1;// location.Y = 2 -> 3
        
        int row = rowInGrid * _cellSize;//96 -> first row
        int column = columnInGrid * _cellSize;// 384 -> third column
        
        return new Point(row, column);
    }

    //Return the range available for movement
    public Point ConvertCollisionBounds(int x, int y)
    {
        int pixelX = x == -1 ? -1 : (x + 2) * _cellSize; //x is topright, to get the pixel boundary +2 to the grid position is needed (+1 for roombounds and +1 to blocker body size)
        int pixelY = y == -1 ? -1 : y * _cellSize; //no addition to the right/down position as we need to account for the moving object body size (96pixels)
        return new Point(pixelX, pixelY);
    }
}

