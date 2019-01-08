﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended.Tiled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace ProjectLondon
{
    public class MapTransitionHandler : MapObject
    {
        private ContentManager Content;

        private string TransitionType { get; set; }
        private TiledMap CurrentMap { get; set; }
        public string DestinationMapName { get; private set; }
        public Vector2 DestinationPosition { get; private set; }
        private Rectangle MapCameraRectangle { get; set; }
        public Rectangle BoundingBox { get; private set; }
        private Texture2D FadeTexture { get; set; }
        public string DestinationAreaName { get; private set; }
        public string DestinationFacing { get; private set; }

        private float Timer { get; set; }
        private float TimerReset { get; set; }
        private float FadeAlpha { get; set; }
        private float TransitionSpeed { get; set; }
        public TransitionState State { get; private set; }

        private SoundEffect TransitionSFX { get; set; }

        public enum TransitionState
        {
            Start,
            FadeOut,
            MapChange,
            FadeIn,
            Complete
        }

        public MapTransitionHandler(ContentManager content, string destinationMapName, Vector2 destinationPosition, 
            Rectangle boundingBox, string destinationAreaName, string destinationFacing)
        {
            Content = content;

            Type = "mapTransition";
            DestinationMapName = destinationMapName;
            DestinationPosition = destinationPosition;
            BoundingBox = boundingBox;
            DestinationAreaName = destinationAreaName;
            DestinationFacing = destinationFacing;
            Timer = 0f;
            FadeAlpha = 0f;

            CurrentMap = null;

            State = TransitionState.Start;
        }

        public void InitializeTransition(TiledMap currentMap, Rectangle mapCameraBoundingbox, SoundEffect transitionSFX, GraphicsDevice graphics)
        {
            CurrentMap = currentMap;
            MapCameraRectangle = mapCameraBoundingbox;
            TransitionSFX = transitionSFX;
            FadeTexture = Content.Load<Texture2D>("SinglePixel");
            State = TransitionState.Start;
        }

        public void MapChangeComplete(Rectangle mapNewRectangle)
        {
            State = TransitionState.FadeIn;
            MapCameraRectangle = mapNewRectangle;
        }

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch (State)
            {
                case TransitionState.Start:
                    {
                        TransitionSFX.CreateInstance().Play();
                        
                        Timer = 0.0f;
                        State = TransitionState.FadeOut;
                        break;
                    }
                case TransitionState.FadeOut:
                    {
                        if(Timer <= 0.5f)
                        {
                            Timer = Timer + deltaTime;
                            FadeAlpha = Timer * 2.5f;
                        }
                        else
                        {
                            MediaPlayer.Stop();
                            Timer = 0f;
                            FadeAlpha = 1.0f;
                            State = TransitionState.MapChange;
                        }
                        break;
                    }
                case TransitionState.MapChange:
                    {
                        // Intentionall Blank, OverworldManager handles Unloading/Loading at this point
                        break;
                    }
                case TransitionState.FadeIn:
                    {
                        if(Timer < 0.5f)
                        {
                            Timer = Timer + deltaTime;
                            FadeAlpha = FadeAlpha - (1.0f * deltaTime);
                        }
                        else
                        {
                            State = TransitionState.Complete;
                        }
                        break;
                    }
                case TransitionState.Complete:
                    {

                        break;
                    }
            }
        }

        public void Draw (SpriteBatch spriteBatch)
        {
            if(State == TransitionState.FadeIn)
            {
                spriteBatch.Draw(FadeTexture, MapCameraRectangle, Color.White * FadeAlpha);
            }
            
            if(State == TransitionState.FadeOut)
            {
                spriteBatch.Draw(FadeTexture, MapCameraRectangle, Color.White * FadeAlpha);
            }
        }
    }
}
