using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace ProjectLondon
{
    public class MainMenuManager
    {
        private List<Texture2D> Splashscreens { get; set; }
        private int CurrentSplashscreen { get; set; }
        private ContentManager Content { get; set; }
        private Animation TitleScreenAnimation { get; set; }
        private AnimationManager AnimationManager { get; set; }
        private Song MainMenuSong { get; set; }
        private Vector2 PressStartTextPosition { get; set; }
        private Vector2 TitleTextPosition { get; set; }
        private SpriteFont TitleFont { get; set; }
        private SpriteFont PressStartFont { get; set; }
        private Texture2D MainMenuBackground { get; set; }
        private float InputTimer { get; set; }
        private float PressStartTimer { get; set; }
        private float SplashTimer { get; set; }
        private bool PressStartVisible { get; set; }
        private int SelectedMenuOption { get; set; }
        public MainMenuState State { get; protected set; }
        public enum MainMenuState
        {
            Splashscreen,
            Title,
            Complete
        }
        private SplashscreenState SplashState { get; set; }
        private enum SplashscreenState
        {
            FadeIn,
            Hold,
            FadeOut,
            Complete
        }

        public MainMenuManager(ContentManager content)
        {
            Content = content;
            State = MainMenuState.Splashscreen;
            SelectedMenuOption = 0;
            InputTimer = 0f;
            PressStartTimer = 1.0f;
            PressStartVisible = true;
            MainMenuBackground = Content.Load<Texture2D>("textures//mainMenu//MainMenuBackBeta01");
            PressStartFont = Content.Load<SpriteFont>("spritefonts//MainMenuFont");
            MainMenuSong = Content.Load<Song>("bgm/Brittle Rille");
            PressStartTextPosition = new Vector2(320, 544);
            TitleTextPosition = new Vector2(320, 96);
            PressStartTextPosition = AdjustPosition(PressStartFont, PressStartTextPosition, "press start!");
            TitleTextPosition = AdjustPosition(PressStartFont, TitleTextPosition, "Project London");

            Splashscreens = new List<Texture2D>();
            Splashscreens.Add(Content.Load<Texture2D>("textures//mainMenu//MLCLogo"));
            SplashTimer = 0f;
            CurrentSplashscreen = 0;

            SplashState = SplashscreenState.FadeIn;

            MediaPlayer.Volume = 0.5f;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(MainMenuSong);
        }

        public Vector2 AdjustPosition(SpriteFont stringFont, Vector2 originalVector, string stringText)
        {
            string _string = stringText;
            Vector2 _stringMeasure = stringFont.MeasureString(_string);
            int _adjustedX, _adjustedY;

            _adjustedX = (int)originalVector.X - ((int)_stringMeasure.X / 2);
            _adjustedY = (int)originalVector.Y - ((int)_stringMeasure.Y / 2);

            Vector2 _newVector2 = new Vector2(_adjustedX, _adjustedY);
            return _newVector2;
            
        }
        public static void DrawText(SpriteBatch spriteBatch, SpriteFont font, string text, Color backColor, Color frontColor, float scale, Vector2 position)
        {
            Vector2 origin = Vector2.Zero;

            spriteBatch.DrawString(font, text, position + new Vector2(1 * scale, 1 * scale), backColor, 0, origin, scale, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, text, position + new Vector2(-1 * scale, 1 * scale), backColor, 0, origin, scale, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, text, position + new Vector2(-1 * scale, -1 * scale), backColor, 0, origin, scale, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, text, position + new Vector2(1 * scale, -1 * scale), backColor, 0, origin, scale, SpriteEffects.None, 1f);          

            spriteBatch.DrawString(font, text, position, frontColor, 0, origin, scale, SpriteEffects.None, 0f);
        }

        public void Update(GameTime gameTime)
        {
            GamePadState _gamepadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState _keyState = Keyboard.GetState();
            float _deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch (State)
            {
                case MainMenuState.Splashscreen:
                    {
                        switch (SplashState)
                        {
                            case SplashscreenState.FadeIn:
                                {
                                    SplashTimer += _deltaTime;

                                    if(SplashTimer >= 1.0f)
                                    {
                                        SplashTimer = 2.5f;
                                        SplashState = SplashscreenState.Hold;
                                    }
                                    break;
                                }
                            case SplashscreenState.Hold:
                                {
                                    SplashTimer -= _deltaTime;

                                    if (SplashTimer <= 0)
                                    {
                                        SplashTimer = 1.0f;
                                        SplashState = SplashscreenState.FadeOut;
                                    }
                                    break;
                                }
                            case SplashscreenState.FadeOut:
                                {
                                    SplashTimer -= _deltaTime;

                                    if (SplashTimer <= 0)
                                    {
                                        SplashTimer = 0f;
                                        SplashState = SplashscreenState.Complete;
                                    }
                                    break;
                                }
                            case SplashscreenState.Complete:
                                {
                                    CurrentSplashscreen += 1;

                                    if(CurrentSplashscreen == Splashscreens.Count())
                                    {
                                        State = MainMenuState.Title;
                                    }
                                    else
                                    {
                                        SplashState = SplashscreenState.FadeIn;
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case MainMenuState.Title:
                    {

                        break;
                    }
            }

            if(PressStartTimer <= 0)
            {
                switch (PressStartVisible)
                {
                    case true:
                        {
                            PressStartTimer = 0.3f;
                            PressStartVisible = false;
                            break;
                        }
                    case false:
                        {
                            PressStartTimer = 1.0f;
                            PressStartVisible = true;
                            break;
                        }
                }
            }
            else
            {
                PressStartTimer -= _deltaTime;
            }

            if(_gamepadState.IsButtonDown(Buttons.Start) == true || _gamepadState.IsButtonDown(Buttons.A) == true || _keyState.IsKeyDown(Keys.Enter))
            {
                State = MainMenuState.Complete;
            }

            //AnimationManager.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            switch (State)
            {
                case MainMenuState.Splashscreen:
                    {
                        float _splashAlpha = 0f;

                        switch (SplashState)
                        {
                            case SplashscreenState.FadeIn:
                                {
                                    _splashAlpha = SplashTimer * 2;
                                    break;
                                }
                            case SplashscreenState.Hold:
                                {
                                    _splashAlpha = 1f;
                                    break;
                                }
                            case SplashscreenState.FadeOut:
                                {
                                    _splashAlpha = SplashTimer * 2;
                                    break;
                                }
                            case SplashscreenState.Complete:
                                {
                                    return;
                                }
                        }
                        
                        spriteBatch.Draw(Splashscreens.ElementAt(CurrentSplashscreen), new Rectangle(0, 0, 640, 640), Color.White * _splashAlpha);
                        break;
                    }
                case MainMenuState.Title:
                    {
                        spriteBatch.Draw(MainMenuBackground, new Rectangle(0, 0, 640, 640), Color.White);
                        DrawText(spriteBatch, PressStartFont, "Project London", Color.Black, Color.Yellow, 1.0f, TitleTextPosition);

                        if (PressStartVisible == true)
                        {
                            DrawText(spriteBatch, PressStartFont, "press start!", Color.Black, Color.Purple, 1.0f, PressStartTextPosition);
                        }
                        break;
                    }
            }

            
        }
        
    }
}
