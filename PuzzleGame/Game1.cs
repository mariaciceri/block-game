using PuzzleGame.Utils.Scenes;
using Microsoft.Xna.Framework.Media;
using PuzzleGame.Utils;

namespace PuzzleGame;

public class Game1 : Core
{
    private Song _themeSong;

    public Game1() : base("Puzzle Game", 1280, 720, false)
    {

    }

    protected override void Initialize()
    {
        base.Initialize();

        Audio.PlaySong(_themeSong);

        // Start the game with the title scene.
        ChangeScene(new TitleScene());
    }

    protected override void LoadContent()
    {
        // Load the background theme music.
        _themeSong = Content.Load<Song>("audio/theme");
    }
}