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
    private readonly Point _position;
    private readonly int _cellSize;

    public GridConverter(Point gridPosition, int cellSize)
    {
        _position = gridPosition;
        _cellSize = cellSize;
    }

    public Point PixelToIndex()
    {
        int column = (int)Math.Round((float)_position.Y / _cellSize) - 1;
        int row = (int)Math.Round((float)_position.X / _cellSize) - 1 ;
        
        return new Point(column, row);
    }

    public Point IndexToPixel()
    {
        int columnInGrid = _position.X + 1;
        int rowInGrid = _position.Y + 1;
        
        int column = columnInGrid * _cellSize;
        int row = rowInGrid * _cellSize;
        
        return new Point(row, column);
    }
}

