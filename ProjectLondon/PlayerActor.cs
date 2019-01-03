using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace ProjectLondon
{
    public class PlayerActor
    {
        private ContentManager Content { get; set; }
        private PlayerIndex ControllerIndex { get; set; }

        private float MoveSpeed { get; set; }
        public Vector2 Position { get; private set; }
        public Vector2 OriginPoint { get; private set; }
        public Vector2 TransformPosition { get; private set; }

        public Rectangle BoundingBox { get; protected set; }
        public Rectangle SolidBoundingBox { get; protected set; }

        public float Health { get; protected set; }
        private float HealthMax { get; set; }

        AnimationManager AnimationManager { get; set; }

        private Dictionary<string, Animation> Animations;
        private string CurrentAnimation;

        private bool IsVisible { get; set; }
        public AreaTransitionMoveState TransitionMoveState { get; private set; }

        public enum AreaTransitionMoveState 
        {
            Start,
            SlideUp,
            SlideDown,
            SlideRight,
            SlideLeft,
            Complete
        }

        public PlayerActor(ContentManager content, Vector2 position, float healthMax)
        {
            Content = content;
            ControllerIndex = PlayerIndex.One;

            MoveSpeed = 1.0f;
            Position = position;
            OriginPoint = Position + new Vector2(8, 8);

            HealthMax = healthMax;
            Health = HealthMax;

            IsVisible = true;

            CreateAnimationDictionary();

            UpdateBoundingBoxPosition();

            TransitionMoveState = AreaTransitionMoveState.Start;
        }

        public void Update(GameTime gameTime)
        {
            GamePadState gamePadState = GamePad.GetState(ControllerIndex);

            //If Negative, stick is leaning more vertically than horizontally

            Vector2 _position = new Vector2(0);

            if (gamePadState.IsButtonDown(Buttons.DPadUp) == true)
            {
                CurrentAnimation = "MoveUp";
                _position = _position + new Vector2(0, -MoveSpeed);
            }

            if (gamePadState.IsButtonDown(Buttons.DPadDown) == true)
            {
                CurrentAnimation = "MoveDown";
                _position = _position + new Vector2(0, MoveSpeed);
            }

            if (gamePadState.IsButtonDown(Buttons.DPadRight) == true)
            {
                CurrentAnimation = "MoveRight";
                _position = _position + new Vector2(MoveSpeed, 0);
            }

            if (gamePadState.IsButtonDown(Buttons.DPadLeft) == true)
            {
                CurrentAnimation = "MoveLeft";
                _position = _position + new Vector2(-MoveSpeed, 0);
            }

            Position = Position + _position;
            OriginPoint = Position + new Vector2(8, 8);
            UpdateBoundingBoxPosition();

            AnimationManager.Play(Animations[CurrentAnimation]);
            AnimationManager.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(IsVisible == true)
            {
                AnimationManager.Draw(spriteBatch, Position);
            }
        }

        private void CreateAnimationDictionary()
        {
            Animations = new Dictionary<string, Animation>();

            Animations.Add("MoveUp", new Animation(AssetManager.SpriteSheets["PlayerSheet"], 2, 16, 15, 0.2f, new Vector2(65, 36)));
            Animations.Add("MoveDown", new Animation(AssetManager.SpriteSheets["PlayerSheet"], 2, 16, 15, 0.2f, new Vector2(36, 36)));
            Animations.Add("MoveLeft", new Animation(AssetManager.SpriteSheets["PlayerSheet"], 2, 16, 15, 0.2f, new Vector2(6, 36)));
            Animations.Add("MoveRight", new Animation(AssetManager.SpriteSheets["PlayerSheet"], 2, 16, 15, 0.2f, new Vector2(94, 36)));

            CurrentAnimation = "MoveDown";

            AnimationManager = new AnimationManager(Animations[CurrentAnimation]);
            AnimationManager.Play(Animations[CurrentAnimation]);
        }

        private void UpdateBoundingBoxPosition()
        {
            BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, 15, 16);
            SolidBoundingBox = new Rectangle((int)Position.X, (int)Position.Y + 8, 15, 8);
        }

        public void Uncollide(Rectangle collisionRectangle)
        {
            Vector2 movePosition = new Vector2(0,0);

            // Check Rectangle width/height
            float _collisionWidth, _collisionHeight;

            _collisionHeight = collisionRectangle.Height;
            _collisionWidth = collisionRectangle.Width;

            if(_collisionWidth > _collisionHeight)
            {
                // Uncollide using HEIGHT
                if(Position.Y + 8 < collisionRectangle.Y)
                {
                    movePosition = movePosition + new Vector2(0, -(_collisionHeight));
                }
                else
                {
                    movePosition = movePosition + new Vector2(0, _collisionHeight);
                }

                Position = Position + movePosition;
                UpdateBoundingBoxPosition();
                return;
            }
            else if(_collisionWidth < _collisionHeight)
            {
                // Uncollide using WIDTH
                if (Position.X < collisionRectangle.X)
                {
                    movePosition = movePosition + new Vector2(-(_collisionWidth), 0);
                }
                else
                {
                    movePosition = movePosition + new Vector2(_collisionWidth, 0);
                }

                Position = Position + movePosition;
                UpdateBoundingBoxPosition();
                return;
            }
            else
            {
                // Uncollide using HEIGHT
                if (Position.Y + 8 < collisionRectangle.Y)
                {
                    movePosition = movePosition + new Vector2(0, -(_collisionHeight));
                }
                else
                {
                    movePosition = movePosition + new Vector2(0, _collisionHeight);
                }

                // Uncollide using WIDTH
                if (Position.X < collisionRectangle.X)
                {
                    movePosition = movePosition + new Vector2(-(_collisionWidth), 0);
                }
                else
                {
                    movePosition = movePosition + new Vector2(_collisionWidth, 0);
                }

                Position = Position + movePosition;
                UpdateBoundingBoxPosition();
                return;
            }
        }

        public void MoveToNewArea(Rectangle collisionRectangle, Rectangle areaRectangle, GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(TransitionMoveState == AreaTransitionMoveState.Start)
            {
                // Check Rectangle width/height
                float _collisionWidth, _collisionHeight;

                _collisionHeight = collisionRectangle.Height;
                _collisionWidth = collisionRectangle.Width;

                if (_collisionWidth < _collisionHeight)
                {
                    // Slide into new area using X-coordinate
                    if (OriginPoint.X < areaRectangle.Left)
                    {
                        TransitionMoveState = AreaTransitionMoveState.SlideRight;
                        TransformPosition = Position + new Vector2(16, 0);
                    }
                    else
                    {
                        TransitionMoveState = AreaTransitionMoveState.SlideLeft;
                        TransformPosition = Position + new Vector2(-16, 0);
                    }
                    return;

                }
                else
                {
                    // Slide into new area using Y-coordinate
                    if (OriginPoint.Y > areaRectangle.Bottom)
                    {
                        TransitionMoveState = AreaTransitionMoveState.SlideUp;
                        TransformPosition = Position + new Vector2(0, -16);
                    }
                    else
                    {
                        TransitionMoveState = AreaTransitionMoveState.SlideDown;
                        TransformPosition = Position + new Vector2(0, 16);
                    }
                    return;
                }
            }
            else
            {
                Vector2 movePosition = new Vector2(0, 0);

                switch (TransitionMoveState)
                {
                    case AreaTransitionMoveState.SlideDown:
                        {
                            if(Position.Y >= TransformPosition.Y)
                            {
                                TransitionMoveState = AreaTransitionMoveState.Complete;
                            }
                            else
                            {
                                movePosition = movePosition + new Vector2(0, (64 * deltaTime));
                            }
                            
                            break;
                        }
                    case AreaTransitionMoveState.SlideUp:
                        {
                            if (Position.Y <= TransformPosition.Y)
                            {
                                TransitionMoveState = AreaTransitionMoveState.Complete;
                            }
                            else
                            {
                                movePosition = movePosition + new Vector2(0, -(64 * deltaTime));
                            }
                            break;
                        }
                    case AreaTransitionMoveState.SlideRight:
                        {
                            if (Position.X >= TransformPosition.X)
                            {
                                TransitionMoveState = AreaTransitionMoveState.Complete;
                            }
                            else
                            {
                                movePosition = movePosition + new Vector2((64 * deltaTime), 0);
                            }

                            break;
                        }
                    case AreaTransitionMoveState.SlideLeft:
                        {
                            if (Position.X <= TransformPosition.X)
                            {
                                TransitionMoveState = AreaTransitionMoveState.Complete;
                            }
                            else
                            {
                                movePosition = movePosition + new Vector2(-(64 * deltaTime), 0);
                            }

                            break;
                        }
                }

                Position = Position + movePosition;
                UpdateBoundingBoxPosition();

                AnimationManager.Update(gameTime);
            }
        }

        public void SetVisibility(bool visible)
        {
            IsVisible = visible;
        }

        public void ResetAreaTransitionState()
        {
            TransitionMoveState = AreaTransitionMoveState.Start;
        }
    }
}
