//store some of the past moves to include an undo button

using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PuzzleGame.Utils.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace PuzzleGame.Utils.GameObjects;

public class BoardLayout
{
    //It differs from a regular Rectangle because location will be the indexes, not pixel position.
    public struct ObjectsOnScene(Point location, int height, int width)
    { 
        public Point Location = location;
        public readonly int Height = height;
        public readonly int Width = width;
    }

    private struct ObjectSize(int x, int y)
    {
        public readonly int StartAt = x;
        public readonly int EndAt = y;
    }
    
    private LevelDesign _level;
    private GridConverter _gridConverter;
    private List<ObjectsOnScene> _board = new();
    private readonly int _cellSize = 96;
    
    public BoardLayout(LevelDesign level)
    {
        _level = level;
    }
    
    public List<ObjectsOnScene> GetBoard()
    {
        return _board;
    }
    
    public void SetBoard(List<GameObject> objects)
    {
        _gridConverter = new GridConverter(_cellSize);
        
        foreach (var obj in objects)
        {
            Point location = _gridConverter.PixelToIndex(obj.Bounds.Location);
            
            ObjectsOnScene objectOnScene = new ObjectsOnScene(
                location,
                obj.Bounds.Height / _cellSize,
                obj.Bounds.Width / _cellSize
                );
            _board.Add(objectOnScene);
        }
    }


    public void UpdateBoard()
    {
        //update one position or all the positions? if all, this method is redundant
    }
    
    public (int, int) GetAvailableCells(Point currentObjectGridPosition, Directions direction)
    {
        List<Point> objectsInTheWay = new();
        (int TopLeft, int DownRight) availableCells;
        ObjectsOnScene draggingObject = _board.Find(o => o.Location == currentObjectGridPosition);

        //Get the size/occupied space of the block being dragged
        (ObjectSize movingObjectOccupiedSpaceVertical, 
            ObjectSize movingObjectOccupiedSpaceHorizontal) = GetObjectSize(draggingObject);
        
        if (direction == Directions.Horizontal)
        {
            foreach (ObjectsOnScene obstacle in _board)
            {
                //Only occupied vertical space matters when dragging horizontally
                (ObjectSize obstacleOccupiedSpaceVertical, _) = GetObjectSize(obstacle);
                
                if (RangesOverlap(movingObjectOccupiedSpaceVertical, obstacleOccupiedSpaceVertical))
                {
                    objectsInTheWay.Add(obstacle.Location);
                }
                
            }
            objectsInTheWay.Sort((a, b) => a.Y.CompareTo(b.Y));
            
            int currentObjectIndex = objectsInTheWay.IndexOf(currentObjectGridPosition);
            
            availableCells = GetOccupiedCells(currentObjectIndex, direction, objectsInTheWay);
        }
        else
        {
            foreach (ObjectsOnScene obstacle in _board)
            {
                //Only horizontal occupied space matters when dragging vertically
                (_, ObjectSize obstacleOccupiedSpaceHorizontal) = GetObjectSize(obstacle);

                if (RangesOverlap(movingObjectOccupiedSpaceHorizontal, obstacleOccupiedSpaceHorizontal))
                {
                    objectsInTheWay.Add(obstacle.Location);
                }
            }
            objectsInTheWay.Sort((a, b) => a.X.CompareTo(b.X));
            
            int currentObjectIndex = objectsInTheWay.IndexOf(currentObjectGridPosition);
            
            availableCells = GetOccupiedCells(currentObjectIndex, direction, objectsInTheWay);
        }

        return availableCells;
    }

    private static bool RangesOverlap(ObjectSize movingObject, ObjectSize obstacleObject)
    {
        return movingObject.StartAt <= obstacleObject.EndAt && movingObject.EndAt >= obstacleObject.StartAt;
    }

    private static (ObjectSize, ObjectSize) GetObjectSize(ObjectsOnScene currentObject)
    {
        ObjectSize verticalSpace = new(
            currentObject.Location.X, currentObject.Height + currentObject.Location.X - 1
        );
        ObjectSize horizontalSpace = new(
            currentObject.Location.Y, currentObject.Width + currentObject.Location.Y - 1
        );
        
        return (verticalSpace, horizontalSpace);
    }

    private static (int, int) GetOccupiedCells(
        int currentObjectIndex,
        Directions direction, 
        List<Point> objectsInTheWay
        )
    {
        int topLeft;
        int downRight;
        
        if (direction == Directions.Horizontal)
        {
            topLeft = currentObjectIndex >= 1
                ? objectsInTheWay[currentObjectIndex - 1].Y
                : -1;
            downRight = currentObjectIndex < objectsInTheWay.Count - 1
                ? objectsInTheWay[currentObjectIndex + 1].Y
                : -1;
        }
        else
        {
            topLeft = currentObjectIndex >= 1
                ? objectsInTheWay[currentObjectIndex - 1].X
                : -1;
            downRight = currentObjectIndex < objectsInTheWay.Count - 1
                ? objectsInTheWay[currentObjectIndex + 1].X
                : -1;
        }
        
        return (topLeft, downRight);
    }
}

