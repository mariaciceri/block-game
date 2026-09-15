using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PuzzleGame.Utils;
using PuzzleGame.Utils.Input;

namespace PuzzleGame.Utils;

/// Provides a game-specific input abstraction that maps physical inputs
/// to game actions, bridging our input system with game-specific functionality.
public static class GameController
{
    private static KeyboardInfo SKeyboard => Core.Input.Keyboard;

    private static MouseInfo MouseMove => Core.Input.Mouse;

    public static bool MovePlayer()
    {
        return MouseMove.IsButtonDown(MouseButton.Left);
    }

    public static Vector2 MousePosition()
    {
        int X = MouseMove.X;
        int Y = MouseMove.Y;
        
        return new Vector2(X, Y);
    }

    public static bool WasPlayerJustReleased()
    {
        return MouseMove.WasButtonJustReleased(MouseButton.Left);
    }
    
    public static bool MoveUp()
    {
        return SKeyboard.WasKeyJustPressed(Keys.Up) ||
               SKeyboard.WasKeyJustPressed(Keys.W); //||
        //s_gamePad.WasButtonJustPressed(Buttons.DPadUp) ||
        //s_gamePad.WasButtonJustPressed(Buttons.LeftThumbstickUp);
    }
    
    public static bool MoveDown()
    {
        return SKeyboard.WasKeyJustPressed(Keys.Down) ||
               SKeyboard.WasKeyJustPressed(Keys.S); //||
        //s_gamePad.WasButtonJustPressed(Buttons.DPadDown) ||
        //s_gamePad.WasButtonJustPressed(Buttons.LeftThumbstickDown);
    }
    
    public static bool MoveLeft()
    {
        return SKeyboard.WasKeyJustPressed(Keys.Left) ||
               SKeyboard.WasKeyJustPressed(Keys.A); //||
              // s_gamePad.WasButtonJustPressed(Buttons.DPadLeft) ||
               //s_gamePad.WasButtonJustPressed(Buttons.LeftThumbstickLeft);
    }

    public static bool MoveRight()
    {
        return SKeyboard.WasKeyJustPressed(Keys.Right) ||
               SKeyboard.WasKeyJustPressed(Keys.D); //||
               //s_gamePad.WasButtonJustPressed(Buttons.DPadRight) ||
               //s_gamePad.WasButtonJustPressed(Buttons.LeftThumbstickRight);
    }
    
    public static bool Pause()
    {
        return SKeyboard.WasKeyJustPressed(Keys.Escape); //||
        //s_gamePad.WasButtonJustPressed(Buttons.Start);
    }
    
    /// Returns true if the player has triggered the "action" button,
    /// typically used for menu confirmation.
    public static bool Action()
    {
        return SKeyboard.WasKeyJustPressed(Keys.Enter); // ||
               //s_gamePad.WasButtonJustPressed(Buttons.A);
    }
}
