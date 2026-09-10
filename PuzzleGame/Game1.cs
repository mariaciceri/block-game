using PuzzleGame.Utils.Scenes;
using Gum.Forms;
using Gum.Forms.Controls;
using MonoGameGum;
using Microsoft.Xna.Framework.Media;
using PuzzleGame.Utils;

namespace PuzzleGame;

public class Game1 : Core
{
    private Song _themeSong;

    public Game1() : base("Puzzle Game", 672, 960, false)
    {

    }

    protected override void Initialize()
    {
        base.Initialize();

        //Audio.PlaySong(_themeSong);
        
        InitializeGum();

        // Start the game with the title scene.
        ChangeScene(new TitleScene());
    }

    protected override void LoadContent()
    {
        // Load the background theme music.
        _themeSong = Content.Load<Song>("audio/theme");
    }

    private void InitializeGum()
    {
        GumService.Default.Initialize(this, DefaultVisualsVersion.V3);
        GumService.Default.ContentLoader.XnaContentManager = Core.Content;
        
        FrameworkElement.KeyboardsForUiControl.Add(GumService.Default.Keyboard);
        
        FrameworkElement.TabReverseKeyCombos.Add(
            new KeyCombo() {PushedKey = Microsoft.Xna.Framework.Input.Keys.Up});
        
        FrameworkElement.TabReverseKeyCombos.Add(
            new KeyCombo() {PushedKey = Microsoft.Xna.Framework.Input.Keys.Down});
        
        GumService.Default.CanvasWidth = GraphicsDevice.PresentationParameters.BackBufferWidth / 4.0f;
        GumService.Default.CanvasHeight = GraphicsDevice.PresentationParameters.BackBufferHeight / 4.0f;
        GumService.Default.Renderer.Camera.Zoom = 4.0f;
        
    }
}