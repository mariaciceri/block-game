using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PuzzleGame.Utils.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace PuzzleGame.Utils.GameObjects;

public class PlayerBall
{
    private List<GameObject> _objects;
    private GameObject _player;
    private GameObject _goal;
    private GameObject _draggingObject;
    private Vector2 _dragOffset;

    private enum DragAxis {None, Horizontal, Vertical}
    private DragAxis _movementDirection;
    private Vector2 _dragStartMousePosition;
    private const float AxisLockThreshold = 3f;
    
    public int Moves { get; private set; }
    public bool IsLevelCleared;
    
    private Rectangle _roomBounds;
    private GridConverter _gridConverter;

    private Point _objectGridPosition;
    private Point _goalGridPosition;
    
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
    }

    public PlayerBall(Rectangle roomBounds)
    {
        _roomBounds = roomBounds;
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
        
        _dragOffset = new Vector2(
            mousePosition.X - _draggingObject.Bounds.X,
            mousePosition.Y - _draggingObject.Bounds.Y
            );

        _movementDirection = DragAxis.None;
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
        bool collisionDetected = CollisionCheck(_draggingObject);
        
        // Defines the movement direction;
        if (_movementDirection == DragAxis.None)
        {
            float deltaX = mousePosition.X - _dragStartMousePosition.X;
            float deltaY = mousePosition.Y - _dragStartMousePosition.Y;

            if (Math.Abs(deltaX) > AxisLockThreshold || 
                Math.Abs(deltaY) > AxisLockThreshold)
            {
                _movementDirection = Math.Abs(deltaX) > Math.Abs(deltaY)
                    ? DragAxis.Horizontal
                    : DragAxis.Vertical;
            }
        }
        else if (_movementDirection == DragAxis.Horizontal)
        {
            float objectPositionX = mousePosition.X - _dragOffset.X;
            int newPositionX = Math.Clamp(
                (int)objectPositionX, 
                _roomBounds.Left, 
                _roomBounds.Right - _draggingObject.Bounds.Width
                );

            if (!collisionDetected)
            {
                _draggingObject.Bounds.Location = new Point(
                    newPositionX , _draggingObject.Bounds.Y
                    );
            }
        }
        else if (_movementDirection == DragAxis.Vertical)
        {
            float objectPositionY = mousePosition.Y - _dragOffset.Y;
            int newPositionY = Math.Clamp(
                (int)objectPositionY, 
                _roomBounds.Top, 
                _roomBounds.Bottom - _draggingObject.Bounds.Height
                );
            
            if (!collisionDetected)
            {
                _draggingObject.Bounds.Location = new Point(_draggingObject.Bounds.X, newPositionY);
            }
        }
    }

    private bool CollisionCheck(GameObject movingObject)
    {
        foreach (GameObject gameObject in _objects)
        {
            if (movingObject == gameObject) continue;
            if (movingObject.Type == GameObjectType.Player && 
                gameObject.Type == GameObjectType.Goal) return false;
            
            bool collisionDetected = movingObject.Bounds.Intersects(gameObject.Bounds);
            if (collisionDetected) return true;
        }

        return false;
    }

    private void OnStoppedMoving()
    {
        _objectGridPosition = ConvertGridPosition(
            _draggingObject.CurrentLocation.X,
            _draggingObject.CurrentLocation.Y,
            ConversionType.ToIndexPosition
            );
        Point newPlayerLocation = ConvertGridPosition(
            _draggingObject.Bounds.X,
            _draggingObject.Bounds.Y,
            ConversionType.ToIndexPosition
            );
            
        if (_movementDirection == DragAxis.Horizontal)
        {
            int newX = (newPlayerLocation.Y + 1) * _player.Bounds.Width;
            _draggingObject.Bounds.Location = new Point(newX, _draggingObject.Bounds.Y);
        }
        else if (_movementDirection == DragAxis.Vertical)
        {
            int newY = (newPlayerLocation.X + 1) * _player.Bounds.Height;
            _draggingObject.Bounds.Location = new Point(_draggingObject.Bounds.X, newY);
        }
        
        if (!_objectGridPosition.Equals(newPlayerLocation))
        {
            Moves++;
        }
        _objectGridPosition = newPlayerLocation;
        LevelClearedCheck();
        _draggingObject = null;
    }

    private enum ConversionType
    {
        ToIndexPosition,
        ToPixelPosition
    }

    //This will give the position of the first cell, so if the object passed
    // is a rectangle, the return position will be the left/top most cell
    private Point ConvertGridPosition(int x, int y, ConversionType typeOfConversion)
    {
        _gridConverter = new GridConverter(new Point(x, y), _player.Bounds.Width); // can use player to calculate bc it returns top left corner
        switch (typeOfConversion)
        {
            case ConversionType.ToIndexPosition:
                return _gridConverter.PixelToIndex();
            case ConversionType.ToPixelPosition:
                return _gridConverter.IndexToPixel();
            default:
                return new Point(-1, -1);
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