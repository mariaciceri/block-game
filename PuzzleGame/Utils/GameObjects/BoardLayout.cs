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
    public struct ObjectsOnScene(GridPosition location, int height, int width)
    { 
        public GridPosition Location = location;
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
            GridPosition location = _gridConverter.PixelToIndex(obj.Bounds.Location);
            
            ObjectsOnScene objectOnScene = new ObjectsOnScene(
                location,
                obj.Bounds.Height / _cellSize,
                obj.Bounds.Width / _cellSize
                );
            _board.Add(objectOnScene);
        }
    }
    
    public void UpdateBoard(GridPosition currentObjectGridPosition, GridPosition newPosition)
    {
        int objectIndex = _board.FindIndex(o => o.Location == currentObjectGridPosition);
        ObjectsOnScene draggingObject = _board[objectIndex];
        draggingObject.Location = newPosition;
        _board[objectIndex] = draggingObject;
    }
    
    public (int, int) GetAvailableCells(
        GridPosition currentObjectGridPosition, Directions direction
        )
    {
        List<GridPosition> objectsInTheWay = new();
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
            objectsInTheWay.Sort((a, b) => a.Column.CompareTo(b.Column));
            
            int currentObjectIndex = objectsInTheWay.IndexOf(currentObjectGridPosition);
            
            //must create a helper function that will get the object immediately before our dragging object
            // and include the height/width in it
            
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
            objectsInTheWay.Sort((a, b) => a.Row.CompareTo(b.Row));
            
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
            currentObject.Location.Row, currentObject.Height + currentObject.Location.Row - 1
        );
        ObjectSize horizontalSpace = new(
            currentObject.Location.Column, currentObject.Width + currentObject.Location.Column - 1
        );
        
        return (verticalSpace, horizontalSpace);
    }

    private (int, int) GetOccupiedCells(
        int currentObjectIndex,
        Directions direction, 
        List<GridPosition> objectsInTheWay
        )
    {
        int topLeft;
        int downRight;
        int topLeftFinalHorizontal;
        int topLeftFinalVertical;
        
        //Find the object blocking to the left or up,
        //calculate their dimension and use their final position as the last unavailable grid position
        if (currentObjectIndex >= 1)
        {
            ObjectsOnScene topLeftBlocker = _board.Find(o => o.Location == objectsInTheWay[currentObjectIndex - 1]);
            (ObjectSize topLeftHorizontalSize, ObjectSize tofLeftVerticalSize) = GetObjectSize(topLeftBlocker);
            topLeftFinalHorizontal = topLeftHorizontalSize.EndAt;
            topLeftFinalVertical = tofLeftVerticalSize.EndAt;
        }
        else
        {
            topLeftFinalHorizontal = -1;
            topLeftFinalVertical = -1;
        }
        
        if (direction == Directions.Horizontal)
        {
            topLeft = currentObjectIndex >= 1
                ? topLeftFinalVertical
                : -1;
            downRight = currentObjectIndex < objectsInTheWay.Count - 1
                ? objectsInTheWay[currentObjectIndex + 1].Column
                : -1;
        }
        else
        {
            topLeft = currentObjectIndex >= 1
                ? topLeftFinalHorizontal
                : -1;
            downRight = currentObjectIndex < objectsInTheWay.Count - 1
                ? objectsInTheWay[currentObjectIndex + 1].Row
                : -1;
        }
        
        return (topLeft, downRight);
    }
}

