using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended.Tiled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectLondon
{
    public class MapTransitionHandler : MapObject
    {
        private string TransitionType { get; set; }
        private TiledMap CurrentMap { get; set; }
        private TiledMap DestinationMap { get; set; }
        private PlayerActor PlayerActor { get; set; }
        public string DestinationMapName { get; private set; }
        public Vector2 DestinationPosition { get; private set; }
        public Rectangle BoundingBox { get; private set; }
        public bool IsTransitionComplete { get; private set; }

        private Texture2D CurrentMapTexture { get; set; }
        private Texture2D DestinationMapTexture { get; set; }

        private float Timer { get; set; }
        private float TimerReset { get; set; }
        private float TransitionSpeed { get; set; }

        public MapTransitionHandler(string type, string destinationMapName, Vector2 destinationPosition, Rectangle boundingBox)
        {
            Type = "mapTransition";
            TransitionType = type;
            DestinationMapName = destinationMapName;
            DestinationPosition = destinationPosition;
            BoundingBox = boundingBox;
            IsTransitionComplete = false;
            Timer = 0f;
            TimerReset = 0.075f;
            TransitionSpeed = 2.5f;

            CurrentMap = null;
            DestinationMap = null;
            PlayerActor = null;

            CurrentMapTexture = null;
            DestinationMapTexture = null;
        }

        public void InitializeTransition(TiledMap currentMap, TiledMap destinationMap, PlayerActor playerActor, GraphicsDevice graphics)
        {
            CurrentMap = currentMap;
            DestinationMap = destinationMap;
            PlayerActor = playerActor;
            IsTransitionComplete = false;

            PlayerActor.SetVisibility(false);
        }

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(Timer > 0)
            {
                Timer = Timer - deltaTime;
            }
            else
            {
                // Handle the Transition
                switch (TransitionType)
                {
                    case "SlideDown":
                        {
                            
                            break;
                        }
                    case "Instant":
                        {

                            break;
                        }
                }

                Timer = TimerReset;
            }
        }

        public void Draw (SpriteBatch spriteBatch)
        {
            
        }
    }
}
