using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PuzzleGame.Utils.Graphics;

public class TextureRegion
{
    public Texture2D Texture { get; set; }

    public Rectangle SourceRectangle { get; set; }

    public int Width => SourceRectangle.Width;

    public int Height => SourceRectangle.Height;
    
    /// Gets the top normalized texture coordinate of this region.
    public float TopTextureCoordinate => SourceRectangle.Top / (float)Texture.Height;
    
    /// Gets the bottom normalized texture coordinate of this region.
    public float BottomTextureCoordinate => SourceRectangle.Bottom / (float)Texture.Height;
    
    ///  Gets the left normalized texture coordinate of this region.
    public float LeftTextureCoordinate => SourceRectangle.Left / (float)Texture.Width;
    
    /// Gets the right normalized texture coordinate of this region.
    public float RightTextureCoordinate => SourceRectangle.Right / (float)Texture.Width;

    
    public TextureRegion() {}

    public TextureRegion(Texture2D texture, int x, int y, int width, int height)
    {
        Texture = texture;
        SourceRectangle = new Rectangle(x, y, width, height);
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color)
    {
        Draw(
            spriteBatch,
            position,
            color,
            0.0f,
            Vector2.Zero,
            Vector2.One,
            SpriteEffects.None,
            0.0f);
    }
    
    public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
    {
        Draw(
            spriteBatch,
            position,
            color,
            rotation,
            origin,
            new Vector2(scale, scale),
            effects,
            layerDepth
        );
    }
    
    public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
    {
        spriteBatch.Draw(
            Texture,
            position,
            SourceRectangle,
            color,
            rotation,
            origin,
            scale,
            effects,
            layerDepth
        );
    }
    
    
    
}
