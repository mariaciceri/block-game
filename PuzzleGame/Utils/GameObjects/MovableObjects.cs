using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PuzzleGame.Utils.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace PuzzleGame.Utils.GameObjects;

public enum Directions {None, Horizontal, Vertical}

public class MovableObjects
{
    private List<GameObject> _objects;
    private GameObject _player;
    private GameObject _goal;
    private GameObject _draggingObject;
    private Vector2 _dragOffset;
    private BoardLayout  _board;

    private Directions _movementDirection;
    private Vector2 _dragStartMousePosition;
    private const float AxisLockThreshold = 3f;
    
    public int Moves { get; private set; }
    public bool IsLevelCleared;
    
    private Rectangle _roomBounds;
    private GridConverter _gridConverter = new (96);
    private struct CollisionBounds(int startAt, int endAt)
    {
        public int StartAt = startAt;
        public int EndAt = endAt;
    }
    private CollisionBounds _collisionBounds;

    private GridPosition _objectGridPosition;
    private GridPosition _goalGridPosition;
    private enum ConversionType
    {
        ToIndexPosition,
        ToPixelPosition,
        CollisionBounds
    }
    
    private Texture2D _pixelTextureTest;

    public void Initialize(LevelDesign levelDesign, int tileSize)
    {
        _objects = new List<GameObject>();
        
        _player = new GameObject(
            GameObjectType.Player,
            new Rectangle(
                (int)levelDesign.PlayerInitialPosition.X, 
                (int)levelDesign.PlayerInitialPosition.Y,
                tileSize, 
                tileSize
                )
            );
        _objects.Add(_player);
        
        _goal = new GameObject(
            GameObjectType.Goal, 
            new Rectangle(
                (int)levelDesign.GoalPosition.X, 
                (int)levelDesign.GoalPosition.Y, 
                tileSize, 
                tileSize)
            );
        _objects.Add(_goal);
        
        foreach (Rectangle obstacle in levelDesign.Obstacles)
        {
            _objects.Add(new GameObject(
                GameObjectType.Obstacle,
                obstacle
            ));
        }

        _goalGridPosition = ConvertGridPosition(
            480, 480,
            ConversionType.ToIndexPosition
        );
        
        _board.SetBoard(_objects);
    }

    public MovableObjects(Rectangle roomBounds, BoardLayout board)
    {
        _roomBounds = roomBounds;
        _board = board;
    }
    
    private void HandleInput()
    {
        //when click can move up and down as long as there is nothing in the way
        //when release the click, it must fall into place to the nearest grid space
        
        Vector2 mousePosition = GameController.MousePosition();
        bool isHoldingClick = GameController.MovePlayer();
            
        if (isHoldingClick && _draggingObject == null)
        {
            OnStartMoving(mousePosition);
        }
        else if (isHoldingClick && _draggingObject != null)
        {
            OnMovingPlayer(mousePosition);
        }
        else if (GameController.WasPlayerJustReleased() && _draggingObject != null)
        {
            OnStoppedMoving();
        }
    }

    private void OnStartMoving(Vector2 mousePosition)
    {
        _draggingObject = FindClickedObject(mousePosition);
        if (_draggingObject == null) return;
        
        _objectGridPosition = ConvertGridPosition(
            _draggingObject.Bounds.X,
            _draggingObject.Bounds.Y,
            ConversionType.ToIndexPosition
        );
        
        _dragOffset = new Vector2(
            mousePosition.X - _draggingObject.Bounds.X,
            mousePosition.Y - _draggingObject.Bounds.Y
            );

        _movementDirection = Directions.None;
        _dragStartMousePosition = mousePosition;
    }

    private GameObject FindClickedObject(Vector2 mousePosition)
    {
        foreach (GameObject gameObject in _objects)
        {
            if (gameObject.Type != GameObjectType.Goal &&
                gameObject.Bounds.Contains(mousePosition))
            {
                return gameObject;
            }
        }
        return null;
    }
    
    private void OnMovingPlayer(Vector2 mousePosition)
    {
        CollisionBounds collisionDetected = CollisionCheck();
        int length = _movementDirection == Directions.Horizontal ?
            _draggingObject.Bounds.Width/96 : _draggingObject.Bounds.Height/96;
        
        if (_movementDirection == Directions.Horizontal)
        {
            float objectPositionX = mousePosition.X - _dragOffset.X;
            GridPosition convertedCollisionBounds = ConvertGridPosition(
                collisionDetected.StartAt, 
                collisionDetected.EndAt, 
                ConversionType.CollisionBounds,
                length
            );
            CollisionBounds pixelBounds = new (convertedCollisionBounds.Row, convertedCollisionBounds.Column);
            
            int lowestPositionAvailable = _roomBounds.Left;
            int highestPositionAvailable = _roomBounds.Right - _draggingObject.Bounds.Width;
            
            if (pixelBounds.StartAt != -1)
            {
                lowestPositionAvailable = pixelBounds.StartAt;
            }
            if (pixelBounds.EndAt != -1)
            {
                highestPositionAvailable = pixelBounds.EndAt;
            }
            
            int newPositionX = Math.Clamp(
                (int)objectPositionX, 
                lowestPositionAvailable, 
                highestPositionAvailable
            );
            
            _draggingObject.Bounds.Location = new Point(
                newPositionX , _draggingObject.Bounds.Y
            );
        }
        else if (_movementDirection == Directions.Vertical)
        {
            float objectPositionY = mousePosition.Y - _dragOffset.Y;
            
            GridPosition convertedCollisionBounds = ConvertGridPosition(
                collisionDetected.StartAt, 
                collisionDetected.EndAt, 
                ConversionType.CollisionBounds,
                length
            );
            CollisionBounds pixelBounds = new (convertedCollisionBounds.Row, convertedCollisionBounds.Column);

            int lowestPositionAvailable = _roomBounds.Top;
            int highestPositionAvailable = _roomBounds.Bottom - _draggingObject.Bounds.Height;

            if (pixelBounds.StartAt != -1)
            {
                lowestPositionAvailable = pixelBounds.StartAt;
            }
            if (pixelBounds.EndAt != -1)
            {
                highestPositionAvailable = pixelBounds.EndAt;
            }
            
            int newPositionY = Math.Clamp(
                (int)objectPositionY, 
                lowestPositionAvailable, 
                highestPositionAvailable
                );
            
            _draggingObject.Bounds.Location = new Point(_draggingObject.Bounds.X, newPositionY);
        }
        else
        {
            CheckDraggingDirection(mousePosition);
        }
        
    }

    private void CheckDraggingDirection(Vector2 mousePosition)
    {
        float deltaX = mousePosition.X - _dragStartMousePosition.X;
        float deltaY = mousePosition.Y - _dragStartMousePosition.Y;

        if (Math.Abs(deltaX) > AxisLockThreshold || 
            Math.Abs(deltaY) > AxisLockThreshold)
        {
            _movementDirection = Math.Abs(deltaX) > Math.Abs(deltaY) 
                ? Directions.Horizontal : Directions.Vertical;
        }
    }

    private CollisionBounds CollisionCheck()
    {
        if (_movementDirection == Directions.None) return new CollisionBounds(-1,-1);

        var (topLeft, downRight) = _board.GetAvailableCells(_objectGridPosition, _movementDirection);
        _collisionBounds = new(topLeft, downRight);
        
        return _collisionBounds;
    }

    private void OnStoppedMoving()
    {
        GridPosition newObjectLocation = ConvertGridPosition(
            _draggingObject.Bounds.X,
            _draggingObject.Bounds.Y,
            ConversionType.ToIndexPosition
            );
            
        if (_movementDirection == Directions.Horizontal)
        {
            int newX = (newObjectLocation.Column + 1) * _player.Bounds.Width;
            _draggingObject.Bounds.Location = new Point(newX, _draggingObject.Bounds.Y);
        }
        else if (_movementDirection == Directions.Vertical)
        {
            int newY = (newObjectLocation.Row + 1) * _player.Bounds.Height;
            _draggingObject.Bounds.Location = new Point(_draggingObject.Bounds.X, newY);
        }
        
        if (!_objectGridPosition.Equals(newObjectLocation))
        {
            Moves++;
        }
        _board.UpdateBoard(_objectGridPosition, newObjectLocation);
        _objectGridPosition = newObjectLocation;
        LevelClearedCheck();
        _draggingObject = null;
    }

    //This will give the position of the first cell, so if the object passed
    // is a rectangle, the return position will be the left/top most cell
    private GridPosition ConvertGridPosition(int x, int y, ConversionType typeOfConversion, int length = 1)
    {
        switch (typeOfConversion)
        {
            case ConversionType.ToIndexPosition:
                return _gridConverter.PixelToIndex(new(x, y));
            case ConversionType.ToPixelPosition:
                return _gridConverter.IndexToPixel(new(x, y));
            case ConversionType.CollisionBounds:
                return _gridConverter.ConvertCollisionBounds(x, y, length);
            default:
                return new GridPosition(-1, -1);
        }
    }

    public void Update(GameTime gameTime)
    {
        HandleInput();
    }

    private bool LevelClearedCheck()
    {
        if (_objectGridPosition == _goalGridPosition &&
            _draggingObject.Type == GameObjectType.Player)
        {
            IsLevelCleared = true;
        }

        return false;
    }

    public void Draw()
    {
        _pixelTextureTest = new Texture2D(Core.GraphicsDevice, 1, 1);
        _pixelTextureTest.SetData(new[] { Color.White });

        foreach (var gameObject in _objects)
        {
            Color color;
            
            switch (gameObject.Type)
            {
                case GameObjectType.Player:
                    color = Color.Red;
                    break;
                case GameObjectType.Goal:
                    color = Color.Green;
                    break;
                case GameObjectType.Obstacle:
                    color = Color.Blue;
                    break;
                default:
                    color = Color.White;
                    break;
            }
            
            Core.SpriteBatch.Draw(_pixelTextureTest, gameObject.Bounds, color);
        }
        
        
    }
}