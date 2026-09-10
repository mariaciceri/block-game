using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PuzzleGame.Utils.Graphics;

namespace PuzzleGame.Utils.GameObjects;

public class PlayerBall
{
    private Rectangle _player;
    private bool _isDragging;
    private Vector2 _dragOffset;
    
    private Rectangle _roomBounds;
    
    private Texture2D _pixelTextureTest;

    public PlayerBall(int x, int y, int width, int height, Rectangle roomBounds)
    {
        _player = new Rectangle(x, y, width, height);
        _roomBounds = roomBounds;
    }
    
    private void HandleInput()
    {
        //when click can move up and down as long as there is nothing in the way
        //when release the click, it must fall into place to the nearest grid space
        
        Vector2 mousePosition = GameController.MousePosition();
        bool isHoldingClick = GameController.HoldItem();

        if (isHoldingClick && !_isDragging && _player.Contains(mousePosition))
        {
            StartDrag(mousePosition);
        }
        else if (isHoldingClick && _isDragging)
        {
            ContinueDrag(mousePosition);
        }
        else if (!isHoldingClick)
        {
            EndDrag();
        }
    }

    private void StartDrag(Vector2 mousePosition)
    {
        _isDragging = true;
        _dragOffset = new Vector2(mousePosition.X - _player.X, mousePosition.Y - _player.Y);
    }

    private void ContinueDrag(Vector2 mousePosition)
    {
        float playerPositionX = mousePosition.X - _dragOffset.X;
        float playerPositionY = mousePosition.Y - _dragOffset.Y;

        int newPositionX = Math.Clamp((int)playerPositionX, _roomBounds.Left, _roomBounds.Right - _player.Width);
        int newPositionY = Math.Clamp((int)playerPositionY, _roomBounds.Top, _roomBounds.Bottom - _player.Height);
        
        bool collisionDetected = CollisionCheck(_player);

        if (!collisionDetected)
        {
            _player.Location = new Point(newPositionX, newPositionY);
        }
        
        // maybe clamp to valid bounds, check collisions with blocks, etc.
    }

    private bool CollisionCheck(Rectangle player)
    {
        //check if X or Y are in the same place as a block, if so, return true.
        return false;
    }

    private void EndDrag()
    {
        _isDragging = false;
        //snap player into nearest grid tile based on current position.
    }

    public void Update(GameTime gameTime)
    {
        HandleInput();
    }

    public void Draw()
    {
        //must make it a boll or something else..maybe an image or something trying to scape?
        _pixelTextureTest = new Texture2D(Core.GraphicsDevice, 1, 1);
        _pixelTextureTest.SetData(new[] { Color.White });
        
        Core.SpriteBatch.Draw(_pixelTextureTest, _player, Color.Green);
    }
}