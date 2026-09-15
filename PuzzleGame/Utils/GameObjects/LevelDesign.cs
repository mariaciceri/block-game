using System;
using System.Collections.Generic;
using System.Numerics;
using PuzzleGame.Utils.GameObjects;
using PuzzleGame.Utils.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum;
using PuzzleGame.Utils.Graphics;
using PuzzleGame.Utils;
using PuzzleGame.Utils.Scenes;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace PuzzleGame.Utils.GameObjects;

public class LevelDesign
{
    public int MinMoves { get; } // will get from another file
    
    private Vector2 _playerInitialPosition;
    private Vector2 _goalPosition;
    
    public struct Obstacle
    {
        public Vector2 Position;
        public Vector2 Size; // or Rectangle bounds, depending on what you need

        public Obstacle(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;
        }
    }
    
    public List<Obstacle> obstacles { get; set; } //set will come from the logic yeah?!

    private Texture2D _pixelTextureTest;
    
    public LevelDesign()
    {
        //what do I need to initialize here?
    }
    
    public void Update(GameTime gameTime)
    {
        //HandleInput();
        
        //will need to handle moving the boxes if they are not fixed..or???
    }
    
}