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
    private int _levelNumber;
    public int MinMoves { get; } // will get from another file
    
    public Vector2 PlayerInitialPosition;
    public Vector2 GoalPosition;
    public List<Rectangle> Obstacles; //set will come from the logic yeah?!
    
    public LevelDesign(int levelNumber)
    {
        _levelNumber = levelNumber;
        Obstacles = new List<Rectangle> //testing manually
        {
            new Rectangle(96, 192, 96, 384),
            new Rectangle(192, 96, 96, 96),
            new Rectangle(288, 96, 96, 96)
        };
        PlayerInitialPosition = new Vector2(96, 96);
        GoalPosition = new Vector2(480, 480);
    }
    
}

