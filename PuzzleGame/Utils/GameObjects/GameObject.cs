using System;
using Microsoft.Xna.Framework;


namespace PuzzleGame.Utils.GameObjects;

public enum GameObjectType
{
    Player,
    Obstacle,
    Goal
}

public class GameObject
{
    public GameObjectType Type;
    public Rectangle Bounds; // position + size combined

    public GameObject(GameObjectType type, Rectangle bounds)
    {
        Type = type;
        Bounds = bounds;
    }
}