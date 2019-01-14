using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace ProjectLondon
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MainGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        MainMenuManager MainMenuManager;
        SaveManager SaveManager;
        OverworldManager OverworldManager;
        PlayerActor MainPlayer;
        MainGameState State = MainGameState.MainMenu;
        enum MainGameState
        {
            MainMenu,
            NormalPlay
        }

        Texture2D DebugOverlayTexture;

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 640;
            graphics.PreferredBackBufferHeight = 640;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            AssetManager.PopulateLists(Content);
            MainMenuManager = new MainMenuManager(Content);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            DebugOverlayTexture = Content.Load<Texture2D>("SinglePixel");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            switch (State)
            {
                case MainGameState.MainMenu:
                    {
                        if(MainMenuManager.State != MainMenuManager.MainMenuState.Complete)
                        {
                            MainMenuManager.Update(gameTime);
                        }
                        else
                        {
                            MediaPlayer.Stop();
                            MainMenuManager = null;
                            MainPlayer = new PlayerActor(Content, new Vector2(0), 6);
                            OverworldManager = new OverworldManager(GraphicsDevice, Content, MainPlayer);
                            OverworldManager.LoadMap("maps/mapZoneTest2");
                            MainPlayer.Activate();
                            OverworldManager.Activate();
                            MainPlayer.SetPosition(new Vector2(OverworldManager.PlayerSpawnX, OverworldManager.PlayerSpawnY));
                            State = MainGameState.NormalPlay;
                        }
                        
                        break;
                    }
                case MainGameState.NormalPlay:
                    {
                        if (OverworldManager.IsActive == true)
                        {
                            OverworldManager.Update(gameTime);
                        }
                        break;
                    }
            }
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            switch (State)
            {
                case MainGameState.NormalPlay:
                    {
                        spriteBatch.Begin(transformMatrix: OverworldManager.MapCamera.GetViewMatrix(), samplerState: SamplerState.PointClamp);

                        if (OverworldManager.IsActive == true)
                        {
                            OverworldManager.Draw(spriteBatch);
                        }

                        spriteBatch.End();
                        break;
                    }
                case MainGameState.MainMenu:
                    {
                        spriteBatch.Begin();
                        MainMenuManager.Draw(spriteBatch);
                        spriteBatch.End();
                        break;
                    }
            }

            

            base.Draw(gameTime);
        }
    }
}
