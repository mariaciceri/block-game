using System;
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

namespace PuzzleGame.Utils.Scenes;

public class GameScene : Scene
{
    private enum GameState
    {
        Playing,
        Paused,
        LevelCleared
    }
    
    private MovableObjects _player;
    private LevelDesign _level;
    private BoardLayout _board;
    
    private Texture2D _pixelTextureTest;
    
    private Tilemap _tilemap; //draw the columns and rows on the screen maybe
    private Rectangle _roomBounds;

    private SoundEffect _snapSoundEffect;

    private int _score;

    private GameSceneUI _ui;
    
    private GameState _state;

    public override void Initialize()
    {
        base.Initialize();

        Core.ExitOnEscape = false;

        // Create the room bounds by getting the bounds of the screen then
        // using the Inflate method to "Deflate" the bounds by the width and
        // height of a tile so that the bounds only covers the inside room of
        // the dungeon tilemap.
        _roomBounds = Core.GraphicsDevice.PresentationParameters.Bounds;
        _roomBounds.Inflate(-_tilemap.TileWidth, -_tilemap.TileHeight);

        //_slime.BodyCollision += OnSlimeBodyCollision;
        
        GumService.Default.Root.Children.Clear();
        
        InitializeUI();
        InitializeNewGame();
    }

    private void InitializeUI()
    {
        GumService.Default.Root.Children.Clear();
        
        _ui = new GameSceneUI();

        _ui.ResumeButtonClick += OnResumeButtonClicked;
        _ui.RetryButtonClick += OnRetryButtonClicked;
        _ui.QuitButtonClick += OnQuitButtonClicked;
    }
    
    private void OnResumeButtonClicked(object sender, EventArgs args)
    {
        _state = GameState.Playing;
    }

    private void OnRetryButtonClicked(object sender, EventArgs args)
    {
        InitializeNewGame();
    }

    private void OnQuitButtonClicked(object sender, EventArgs args)
    {
        Core.ChangeScene(new TitleScene());
    }

    private void InitializeNewGame()
    {
        //currently passing the position manually - a function will give us these
        _level = new LevelDesign(0);
        _board = new BoardLayout(_level);
        _player = new MovableObjects(_roomBounds, _board);
        _player.Initialize(_level, (int)_tilemap.TileWidth);
        
        _score = 0;
        _state = GameState.Playing;
    }
    
    public override void LoadContent()
    {
        TextureAtlas atlas = TextureAtlas.FromFile(Core.Content, "images/atlas-definition.xml");
        
        _tilemap = Tilemap.FromFile(Content, "images/tilemap-definition.xml");
        _tilemap.Scale = new Vector2(1.5f, 1.5f);
        
        SoundEffect bounceSoundEffect = Content.Load<SoundEffect>("audio/bounce");
        
        //_collectSoundEffect = Content.Load<SoundEffect>("audio/collect");
    }

    public override void Update(GameTime gameTime)
    {
        // Ensure the UI is always updated.
        _ui.Update(gameTime);
        
        if (_state == GameState.LevelCleared)
        {
            return;
        }

        if (GameController.Pause())
        {
            TogglePause();
        }
        
        if (_state == GameState.Paused)
        {
            return;
        }
        
        _player.Update(gameTime);
        if (_player.IsLevelCleared)
        {
            _state = GameState.LevelCleared;
            LevelCleared();
        }
        _ui.UpdateScoreText(_player.Moves);
        //calculate final points. will be displayed in a level completed menu though
        
        //CollisionChecks();
    }

    private void TogglePause()
    {
        if (_state == GameState.Paused)
        {
            _ui.HidePausePanel();
            _state = GameState.Playing;
        }
        else
        {
            {
                _ui.ShowPausePanel();
                _state = GameState.Paused;
            }
        }
    }

    private void LevelCleared()
    {
        _ui.ShowLevelClearedPanel();
        _state = GameState.LevelCleared;
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.DimGray);
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        
        _tilemap.Draw(Core.SpriteBatch);

        _player.Draw();
        
        Core.SpriteBatch.End();
        
        _ui.Draw();
    }
}