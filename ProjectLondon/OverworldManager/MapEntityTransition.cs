using MonoGame.Extended.Tiled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace ProjectLondon
{
    public class MapEntityTransition : MapEntityStatic, IMapEntity
    {
        // Content Management
        private ContentManager Content { get; set; }

        // Destination Handling
        private string TransitionType { get; set; }
        private TiledMap CurrentMap { get; set; }
        public string DestinationMapName { get; private set; }
        public Vector2 DestinationPosition { get; private set; }
        private Rectangle CameraViewRectangle { get; set; }
        private Texture2D FadeTexture { get; set; }
        public string DestinationAreaName { get; private set; }
        public string DestinationFacing { get; private set; }

        // Transition Control
        private float Timer { get; set; }
        private float FadeAlpha { get; set; }
        private float TransitionSpeed { get; set; }
        public TransitionState State { get; private set; }
        public enum TransitionState
        {
            Start,
            FadeOut,
            MapChange,
            FadeIn,
            Complete
        }

        // Additional Resources
        private SoundEffect TransitionSFX { get; set; }

        public MapEntityTransition(ContentManager content, string destinationMapName, Vector2 destinationPosition,
            string destinationAreaName, string destinationFacing, Vector2 position, int width, int height) : base(false, position, width, height)
        {
            Content = content;

            Type = "MapEntityTransition";
            DestinationMapName = destinationMapName;
            DestinationPosition = destinationPosition;
            DestinationAreaName = destinationAreaName;
            DestinationFacing = destinationFacing;
            Timer = 0f;
            FadeAlpha = 0f;

            CurrentMap = null;

            State = TransitionState.Start;
        }

        /* -- METHODS -- */
        /// <summary>
        /// Starts a Map Transition with the information provided
        /// </summary>
        /// <param name="currentMap">The Map currently loaded into memory</param>
        /// <param name="mapCameraBoundingbox">The MapCamera's current viewing area</param>
        /// <param name="transitionSFX">The SoundEffect to play at the time of Transition</param>
        /// <param name="graphics"></param>
        public void InitializeTransition(TiledMap currentMap, Rectangle mapCameraBoundingbox, SoundEffect transitionSFX)
        {
            CurrentMap = currentMap;
            CameraViewRectangle = mapCameraBoundingbox;
            TransitionSFX = transitionSFX;
            FadeTexture = Content.Load<Texture2D>("SinglePixel");
            State = TransitionState.Start;
        }
        public void MapChangeComplete(Rectangle mapNewRectangle)
        {
            State = TransitionState.FadeIn;
            CameraViewRectangle = mapNewRectangle;
        }
        public new void Update(GameTime gameTime)
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
                        if (Timer <= 0.5f)
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
                        // Intentionally Blank, OverworldManager handles Unloading/Loading at this point
                        break;
                    }
                case TransitionState.FadeIn:
                    {
                        if (Timer < 0.5f)
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
        public new void Draw(SpriteBatch spriteBatch)
        {
            if (State == TransitionState.FadeIn)
            {
                spriteBatch.Draw(FadeTexture, CameraViewRectangle, Color.White * FadeAlpha);
            }

            if (State == TransitionState.FadeOut)
            {
                spriteBatch.Draw(FadeTexture, CameraViewRectangle, Color.White * FadeAlpha);
            }
        }

        public IMapEntity Clone()
        {
            return new MapEntityTransition(Content, DestinationMapName, DestinationPosition, DestinationAreaName, DestinationFacing, Position, BoundingBox.Width, BoundingBox.Height);
        }
    }
}
