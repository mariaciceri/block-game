using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using PuzzleGame.Utils.Graphics;
using MonoGameGum;
using Gum.Forms.Controls;
using MonoGameGum.GueDeriving;
using PuzzleGame.Utils.UI;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace PuzzleGame.Utils.Scenes;

public class TitleScene : Scene
{
    private const string PuzzleText = "Maze\n  Craze";
    
    private SpriteFont _font5x;
    
    private Vector2 _dungeonTextPos;
    private Vector2 _dungeonTextOrigin;

    private Vector2 _slimeTextPos;
    private Vector2 _slimeTextOrigin;

    private Texture2D _backgroundPattern;
    private Rectangle _backgroundDestination;
    private Vector2 _backgroundOffset;
    private readonly float _scrollSpeed = 50.0f;
    
    private SoundEffect _uiSoundEffect;
    private Panel _titleScreenButtonsPanel;
    private Panel _optionsPanel;
    private AnimatedButton _optionsButton;
    private AnimatedButton _optionsBackButton;
    
    private TextureAtlas _atlas;

    
    public override void Initialize()
    {
        base.Initialize();
        
        Core.ExitOnEscape = true;
        
        _backgroundOffset = Vector2.Zero;
        _backgroundDestination = Core.GraphicsDevice.PresentationParameters.Bounds;

        // Set the position and origin for the Dungeon text.
        Vector2 size = _font5x.MeasureString(PuzzleText);
        _dungeonTextPos = new Vector2(
            _backgroundDestination.Width * 0.5f,
            _backgroundDestination.Height * 0.3f
            );
        _dungeonTextOrigin = size * 0.5f;

        // Set the position and origin for the press enter text.
        //size = _font.MeasureString(PressEnterText);
        //_pressEnterPos = new Vector2(640, 620);
        //_pressEnterOrigin = size * 0.5f;

        
        
        InitializeUI();
    }

    public override void LoadContent()
    {
        _font5x = Content.Load<SpriteFont>("font/TitleFont");
        
        _backgroundPattern = Content.Load<Texture2D>("images/background-pattern");
        
        _uiSoundEffect = Core.Content.Load<SoundEffect>("audio/ui");
        
        _atlas = TextureAtlas.FromFile(Core.Content, "images/atlas-definition.xml");
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
        
        GumService.Default.Update(gameTime);
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

        if (_titleScreenButtonsPanel.IsVisible)
        {
            Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
            Color dropShadowColor = Color.Black * 0.5f;
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

            Core.SpriteBatch.End();
        }
        
        GumService.Default.Draw();
    }

    public void CreateTitlePanel()
    {
        //Container that holds the buttons
        _titleScreenButtonsPanel = new Panel();
        _titleScreenButtonsPanel.Dock(Gum.Wireframe.Dock.Fill);
        _titleScreenButtonsPanel.AddToRoot();
        float buttonSpacing = _backgroundDestination.Height * 0.02f;
        float bottomOffset = _backgroundDestination.Height * 0.2f;
        
        AnimatedButton startButton = new AnimatedButton(_atlas);
        startButton.Anchor(Gum.Wireframe.Anchor.CenterHorizontally);
        startButton.Y = (bottomOffset + buttonSpacing);
        startButton.Text = "Start";
        startButton.Click += HandleStartClicked;
        _titleScreenButtonsPanel.AddChild(startButton);
        
        _optionsButton = new AnimatedButton(_atlas);
        _optionsButton.Anchor(Gum.Wireframe.Anchor.CenterHorizontally);
        _optionsButton.Y = bottomOffset;
        _optionsButton.Text = "Options";
        _optionsButton.Click += HandleOptionsClicked;
        _titleScreenButtonsPanel.AddChild(_optionsButton);

        startButton.IsFocused = true;
    }
    
    private void HandleStartClicked(object sender, EventArgs e)
    {
        Core.Audio.PlaySoundEffect(_uiSoundEffect);
        
        Core.ChangeScene(new GameScene());
    }

    private void HandleOptionsClicked(object sender, EventArgs e)
    {
        Core.Audio.PlaySoundEffect(_uiSoundEffect);
        
        _titleScreenButtonsPanel.IsVisible = false;

        _optionsPanel.IsVisible = true;

        _optionsBackButton.IsFocused = true;
    }

    private void CreateOptionsPanel()
    {
        _optionsPanel = new Panel();
        _optionsPanel.Dock(Gum.Wireframe.Dock.Fill);
        _optionsPanel.IsVisible = false;
        _optionsPanel.AddToRoot();

        TextRuntime optionsText = new TextRuntime();
        optionsText.X = 10;
        optionsText.Y = 10;
        optionsText.Text = "OPTIONS";
        optionsText.UseCustomFont = true;
        optionsText.FontScale = 0.5f;
        optionsText.CustomFontFile = @"font/MyFont.fnt";
        _optionsPanel.AddChild(optionsText);

        OptionsSlider musicSlider = new OptionsSlider(_atlas);
        musicSlider.Name = "MusicSlider";
        musicSlider.Text = "MUSIC";
        musicSlider.Anchor(Gum.Wireframe.Anchor.Top);
        musicSlider.Y = 30f;
        musicSlider.Minimum = 0;
        musicSlider.Maximum = 1;
        musicSlider.Value = Core.Audio.SongVolume;
        musicSlider.SmallChange = .1;
        musicSlider.LargeChange = .2;
        musicSlider.ValueChanged += HandleMusicSliderValueChanged;
        musicSlider.ValueChangeCompleted += HandleMusicSliderValueChangeCompleted;
        _optionsPanel.AddChild(musicSlider);

        OptionsSlider sfxSlider = new OptionsSlider(_atlas);
        sfxSlider.Name = "SfxSlider";
        sfxSlider.Text = "SFX";
        sfxSlider.Anchor(Gum.Wireframe.Anchor.Top);
        sfxSlider.Y = 93;
        sfxSlider.Minimum = 0;
        sfxSlider.Maximum = 1;
        sfxSlider.Value = Core.Audio.SoundEffectVolume;
        sfxSlider.SmallChange = .1;
        sfxSlider.LargeChange = .2;
        sfxSlider.ValueChanged += HandleSfxSliderChanged;
        sfxSlider.ValueChangeCompleted += HandleSfxSliderChangeCompleted;
        _optionsPanel.AddChild(sfxSlider);

        _optionsBackButton = new AnimatedButton(_atlas);
        _optionsBackButton.Text = "BACK";
        _optionsBackButton.Anchor(Gum.Wireframe.Anchor.BottomRight);
        _optionsBackButton.X = -28f;
        _optionsBackButton.Y = -10f;
        _optionsBackButton.Click += HandleOptionsButtonBack;
        _optionsPanel.AddChild(_optionsBackButton);
    }

    private void HandleSfxSliderChanged(object sender, EventArgs e)
    {
        
        // Intentionally not playing the UI sound effect here so that it is not
        // constantly triggered as the user adjusts the slider's thumb on the
        // track.

        var slider = (Slider)sender;
        
        Core.Audio.SoundEffectVolume = (float)slider.Value;
    }

    private void HandleSfxSliderChangeCompleted(object sender, EventArgs e)
    {
        Core.Audio.PlaySoundEffect(_uiSoundEffect);
    }

    private void HandleMusicSliderValueChanged(object sender, EventArgs e)
    {
        var slider = (Slider)sender;
        
        Core.Audio.SongVolume = (float)slider.Value;
    }
    
    private void HandleMusicSliderValueChangeCompleted(object sender, EventArgs args)
    {
        // A UI interaction occurred, play the sound effect
        Core.Audio.PlaySoundEffect(_uiSoundEffect);
    }

    private void HandleOptionsButtonBack(object sencer, EventArgs e)
    {
        Core.Audio.PlaySoundEffect(_uiSoundEffect);

        _titleScreenButtonsPanel.IsVisible = true;
        _optionsPanel.IsVisible = false;
        _optionsBackButton.IsFocused = false;
    }

    private void InitializeUI()
    {
        // Clear out any previous UI in case we came here from a different screen
        GumService.Default.Root.Children.Clear();
        
        CreateTitlePanel();
        CreateOptionsPanel();
    }
}