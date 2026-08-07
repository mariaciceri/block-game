using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace PuzzleGame.Utils.Scenes;

public abstract class Scene : IDisposable
{
    /// Gets the ContentManager used for loading scene-specific assets.
    /// Assets loaded through this ContentManager will be automatically unloaded when this scene ends.
    protected ContentManager Content { get; }
    
    public bool IsDisposed { get; private set; }
    
    public Scene()
    {
        // Create a content manager for the scene
        Content = new ContentManager(Core.Content.ServiceProvider);

        // Set the root directory for content to the same as the root directory
        // for the game's content.
        Content.RootDirectory = Core.Content.RootDirectory;
    }

    // Finalizer, called when object is cleaned up by garbage collector.
    ~Scene() => Dispose(false);

    public virtual void Initialize()
    {
        LoadContent();
    }
    
    public virtual void LoadContent() { }
    
    public virtual void UnloadContent()
    {
        Content.Unload();
    }
    
    public virtual void Update(GameTime gameTime) { }

    public virtual void Draw(GameTime gameTime) { }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (IsDisposed)
        {
            return;
        }

        if (disposing)
        {
            UnloadContent();
            Content.Dispose();
        }
        IsDisposed = true; 
    }
}