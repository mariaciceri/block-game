using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PuzzleGame.Utils;
using PuzzleGame.Utils.Scenes;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace PuzzleGame.Utils.Scenes;

public class TitleScene : Scene
{
    private const string PuzzleText = "Puzzle Game";
    private const string SlimeText = "Slime";
    private const string PressEnterText = "Press Enter to Start";
    
    private SpriteFont _font;
    private SpriteFont _font5x;
    
    private Vector2 _dungeonTextPos;
    private Vector2 _dungeonTextOrigin;

    private Vector2 _slimeTextPos;
    private Vector2 _slimeTextOrigin;

    private Vector2 _pressEnterPos;
    private Vector2 _pressEnterOrigin;

    private Texture2D _backgroundPattern;
    private Rectangle _backgroundDestination;
    private Vector2 _backgroundOffset;
    private readonly float _scrollSpeed = 50.0f;
    
    public override void Initialize()
    {
        base.Initialize();
        
        Core.ExitOnEscape = true;

        // Set the position and origin for the Dungeon text.
        Vector2 size = _font5x.MeasureString(PuzzleText);
        _dungeonTextPos = new Vector2(640, 100);
        _dungeonTextOrigin = size * 0.5f;

        // Set the position and origin for the Slime text.
        size = _font5x.MeasureString(SlimeText);
        _slimeTextPos = new Vector2(757, 207);
        _slimeTextOrigin = size * 0.5f;

        // Set the position and origin for the press enter text.
        size = _font.MeasureString(PressEnterText);
        _pressEnterPos = new Vector2(640, 620);
        _pressEnterOrigin = size * 0.5f;

        _backgroundOffset = Vector2.Zero;
        _backgroundDestination = Core.GraphicsDevice.PresentationParameters.Bounds;
    }

    public override void LoadContent()
    {
        //Loading the cached font.
        _font = Core.Content.Load<SpriteFont>("font/MyFont");
        
        _font5x = Content.Load<SpriteFont>("font/TitleFont");
        
        _backgroundPattern = Content.Load<Texture2D>("images/background-pattern");
    }

    public override void Update(GameTime gameTime)
    {
        // If the user presses enter, switch to the game scene.
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Enter))
        {
            Core.ChangeScene(new GameScene());
        }
        // Scrolls down and right
        float offset = _scrollSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        _backgroundOffset.X -= offset;
        _backgroundOffset.Y -= offset;
        
        // Ensure that the offsets do not go beyond the texture bounds
        _backgroundOffset.X %= _backgroundPattern.Width;
        _backgroundOffset.Y %= _backgroundPattern.Height;
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(new Color(32, 40, 78, 255));
        
        // Draw the background pattern first using the PointWrap sampler state.
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointWrap);
        Core.SpriteBatch.Draw(
            _backgroundPattern, 
            _backgroundDestination, 
            new Rectangle(
                _backgroundOffset.ToPoint(),
                _backgroundDestination.Size
                ),
            Color.White * 0.5f
            );
        Core.SpriteBatch.End();

        // Begin the sprite batch to prepare for rendering.
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        
        Color dropShadowColor = Color.Black * 0.5f;

        // Draw the Dungeon text slightly offset from it is original position and
        // with a transparent color to give it a drop shadow.
        Core.SpriteBatch.DrawString(
            _font5x,
            PuzzleText,
            _dungeonTextPos + new Vector2(10, 10),
            dropShadowColor,
            0.0f,
            _dungeonTextOrigin,
            1.0f,
            SpriteEffects.None,
            1.0f);

        // Draw the Dungeon text on top of that at its original position.
        Core.SpriteBatch.DrawString(
            _font5x,
            PuzzleText,
            _dungeonTextPos,
            Color.White,
            0.0f,
            _dungeonTextOrigin,
            1.0f,
            SpriteEffects.None,
            1.0f);

        // Draw the Slime text slightly offset from it is original position and
        // with a transparent color to give it a drop shadow.
        Core.SpriteBatch.DrawString(
            _font5x, 
            SlimeText,
            _slimeTextPos + new Vector2(10, 10),
            dropShadowColor, 
            0.0f,
            _slimeTextOrigin,
            1.0f,
            SpriteEffects.None, 
            1.0f);

        // Draw the Slime text on top of that at its original position.
        Core.SpriteBatch.DrawString(
            _font5x, 
            SlimeText, 
            _slimeTextPos,
            Color.White,
            0.0f,
            _slimeTextOrigin, 
            1.0f,
            SpriteEffects.None,
            1.0f);

        // Draw the press enter text.
        Core.SpriteBatch.DrawString(
            _font,
            PressEnterText,
            _pressEnterPos,
            Color.White,
            0.0f,
            _pressEnterOrigin,
            1.0f,
            SpriteEffects.None,
            0.0f);

        // Always end the sprite batch when finished.
        Core.SpriteBatch.End();
    }
}