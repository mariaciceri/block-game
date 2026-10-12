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
        public int Height = height;
        public int Width = width;
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
    
    public List<Point> GetAvailableCells(Point currentObjectGridPosition, Directions direction)
    {
        List<Point> objectsInTheWay = new();
        (int TopLeft, int DownRight) availableCells;
        ObjectsOnScene draggingObject = _board.Find(o => o.Location == currentObjectGridPosition);

        //Get the size/occupied space of the block being dragged
        ObjectSize movingObjectOccupiedSpaceVertical = new(
            draggingObject.Location.X, draggingObject.Height + draggingObject.Location.X - 1
            );
        ObjectSize movingObjectOccupiedSpaceHorizontal = new(
            draggingObject.Location.Y, draggingObject.Width + draggingObject.Location.Y - 1
        );
        
        if (direction == Directions.Horizontal)
        {
            foreach (ObjectsOnScene obstacle in _board)
            {
                //Only occupied vertical space matters when dragging horizontally
                ObjectSize obstacleOccupiedSpaceVertical = new(
                        obstacle.Location.X, obstacle.Height + obstacle.Location.X - 1
                    );
                
                if (RangesOverlap(movingObjectOccupiedSpaceVertical, obstacleOccupiedSpaceVertical))
                {
                    objectsInTheWay.Add(obstacle.Location);
                }
                
            }
            objectsInTheWay.Sort((a, b) => a.Y.CompareTo(b.Y));
            
            int currentObjectIndex = objectsInTheWay.IndexOf(currentObjectGridPosition);
            
            availableCells.TopLeft = currentObjectIndex >= 1 
                ? objectsInTheWay[currentObjectIndex - 1].Y : -1;
            availableCells.DownRight = currentObjectIndex < objectsInTheWay.Count - 1
                ? objectsInTheWay[currentObjectIndex + 1].Y
                : -1;
        }
        else
        {
            foreach (ObjectsOnScene obstacle in _board)
            {
                //Only horizontal occupied space matters when dragging vertically
                ObjectSize obstacleOccupiedSpaceHorizontal = new(
                    obstacle.Location.Y, obstacle.Width + obstacle.Location.Y - 1
                );

                if (RangesOverlap(movingObjectOccupiedSpaceHorizontal, obstacleOccupiedSpaceHorizontal))
                {
                    objectsInTheWay.Add(obstacle.Location);
                }
            }
            objectsInTheWay.Sort((a, b) => a.X.CompareTo(b.X));
            
            int currentObjectIndex = objectsInTheWay.IndexOf(currentObjectGridPosition);
            
            
            availableCells.TopLeft = currentObjectIndex >= 1 
                ? objectsInTheWay[currentObjectIndex - 1].X : -1;
            availableCells.DownRight = currentObjectIndex < objectsInTheWay.Count - 1
                ? objectsInTheWay[currentObjectIndex + 1].X
                : -1;
        }
        
        return objectsInTheWay;
        
        //return index - 1 and index + 1
        //if no index lower or higher (meaning no box on one side) return -1.
    }

    private static bool RangesOverlap(ObjectSize movingObject, ObjectSize obstacleObject)
    {
        return movingObject.StartAt <= obstacleObject.EndAt && movingObject.EndAt >= obstacleObject.StartAt;
    }
}

