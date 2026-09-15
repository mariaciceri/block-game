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
    private Rectangle _player;
    private Rectangle _goal;
    private bool _isDragging;
    private Vector2 _dragOffset;

    private enum DragAxis {None, Horizontal, Vertical}
    private DragAxis _movementDirection;
    private Vector2 _dragStartMousePosition;
    private const float AxisLockThreshold = 3f;
    
    public int Moves { get; private set; }
    
    private Rectangle _roomBounds;
    private GridConverter _gridConverter;

    private Point _playerGridPosition;
    
    private Texture2D _pixelTextureTest;

    public PlayerBall(int x, int y, int width, int height, Rectangle roomBounds)
    {
        _player = new Rectangle(x, y, width, height);
        _roomBounds = roomBounds;

        _playerGridPosition = GridPositionConverter(ConversionType.ToCellPosition);
    }
    
    private void HandleInput()
    {
        //when click can move up and down as long as there is nothing in the way
        //when release the click, it must fall into place to the nearest grid space
        
        Vector2 mousePosition = GameController.MousePosition();
        bool isHoldingClick = GameController.MovePlayer();

        if (isHoldingClick && !_isDragging && _player.Contains(mousePosition))
        {
            OnStartMoving(mousePosition);
        }
        else if (isHoldingClick && _isDragging)
        {
            OnMovingPlayer(mousePosition);
        }
        else if (GameController.WasPlayerJustReleased() && _isDragging)
        {
            OnStoppedMoving();
        }
    }

    private void OnStartMoving(Vector2 mousePosition)
    {
        _dragOffset = new Vector2(
            mousePosition.X - _player.X,
            mousePosition.Y - _player.Y
            );
        _isDragging = true;
        _movementDirection = DragAxis.None;
        _dragStartMousePosition = mousePosition;
    }

    private void OnMovingPlayer(Vector2 mousePosition)
    {
        bool collisionDetected = CollisionCheck(_player);

        if (_movementDirection == DragAxis.None)
        {
            float deltaX = mousePosition.X - _dragStartMousePosition.X;
            float deltaY = mousePosition.Y - _dragStartMousePosition.Y;

            if (Math.Abs(deltaX) > AxisLockThreshold || Math.Abs(deltaY) > AxisLockThreshold)
            {
                _movementDirection = Math.Abs(deltaX) > Math.Abs(deltaY)
                    ? DragAxis.Horizontal
                    : DragAxis.Vertical;
            }
        }
        else if (_movementDirection == DragAxis.Horizontal)
        {
            float playerPositionX = mousePosition.X - _dragOffset.X;
            int newPositionX = Math.Clamp(
                (int)playerPositionX, _roomBounds.Left, _roomBounds.Right - _player.Width
                );

            if (!collisionDetected)
            {
                _player.Location = new Point(newPositionX , _player.Y);
            }
        }
        else if (_movementDirection == DragAxis.Vertical)
        {
            float playerPositionY = mousePosition.Y - _dragOffset.Y;
            int newPositionY = Math.Clamp(
                (int)playerPositionY, _roomBounds.Top, _roomBounds.Bottom - _player.Height
                );
            
            if (!collisionDetected)
            {
                _player.Location = new Point(_player.X, newPositionY);
            }
        }
    }

    private bool CollisionCheck(Rectangle player)
    {
        //check if X or Y are in the same place as a block, if so, return true.
        return false;
    }

    private void OnStoppedMoving()
    {
        Point oldPlayerLocation = _playerGridPosition;
        Point newPlayerLocation = GridPositionConverter(ConversionType.ToCellPosition); //(1,0)
            
        if (_movementDirection == DragAxis.Horizontal)
        {
            int newX = (newPlayerLocation.Y + 1) * _player.Width;
            _player.Location = new Point(newX, _player.Y);
        }
        else if (_movementDirection == DragAxis.Vertical)
        {
            int newY = (newPlayerLocation.X + 1) * _player.Height;
            _player.Location = new Point(_player.X, newY);
        }
        
        _playerGridPosition = newPlayerLocation;
        _isDragging = false;
        if (!oldPlayerLocation.Equals(newPlayerLocation))
        {
            Moves++;
        }
    }

    private enum ConversionType
    {
        ToCellPosition,
        ToPixelPosition
    }

    private Point GridPositionConverter(ConversionType typeOfConversion)
    {
        _gridConverter = new GridConverter(new Point(_player.X, _player.Y), _player.Width);
        switch (typeOfConversion)
        {
            case ConversionType.ToCellPosition:
                return _gridConverter.PixelToGrid();
            case ConversionType.ToPixelPosition:
                return _gridConverter.GridToPixel();
            default:
                return new Point(0, 0);
        }
    }

    public void Update(GameTime gameTime)
    {
        HandleInput();
    }

    public void Draw()
    {
        _pixelTextureTest = new Texture2D(Core.GraphicsDevice, 1, 1);
        _pixelTextureTest.SetData(new[] { Color.White });
        
        Core.SpriteBatch.Draw(_pixelTextureTest, _player, Color.Green);
        
        _goal = new Rectangle(480, 480, 96, 96);
        
        Core.SpriteBatch.Draw(_pixelTextureTest, _goal, Color.Black);
    }
}