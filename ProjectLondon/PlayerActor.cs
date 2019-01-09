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
        private Dictionary<string, bool> ControlKeys { get; set; }
        public bool IsActive { get; protected set; }

        private float MoveSpeed { get; set; }
        public Vector2 Position { get; private set; }
        public Vector2 OriginPoint { get; private set; }
        public Vector2 TransformPosition { get; private set; }

        public Rectangle BoundingBox { get; protected set; }
        public Rectangle SolidBoundingBox { get; protected set; }

        public float Health { get; protected set; }
        private float HealthMax { get; set; }
        private List<PlayerActorItem> PlayerItems { get; set; }

        AnimationManager AnimationManager { get; set; }
        public bool IsAnimationOverride { get; protected set; }
        private float AnimationOverrideTimer { get; set; }

        private Dictionary<string, Animation> Animations;
        private string CurrentAnimation;

        private bool IsVisible { get; set; }

        public AreaTransitionMoveState TransitionMoveState { get; private set; }
        public PlayerFacing Facing { get; private set; }
        public PlayerController ControlStyle { get; private set; }
        public enum AreaTransitionMoveState 
        {
            Start,
            SlideUp,
            SlideDown,
            SlideRight,
            SlideLeft,
            Complete
        }
        public enum PlayerFacing
        {
            Up,
            Down,
            Left,
            Right
        }
        public enum PlayerController
        {
            Keyboard,
            Gamepad
        }

        public PlayerActor(ContentManager content, Vector2 position, float healthMax)
        {
            GamePadState _gamepadState = GamePad.GetState(PlayerIndex.One);

            Content = content;
            IsActive = false;

            if(_gamepadState.IsConnected == true)
            {
                ControlStyle = PlayerController.Gamepad;
            }
            else{
                ControlStyle = PlayerController.Keyboard;
            }

            CreateControlConfiguration();

            MoveSpeed = 1.0f;
            Position = position;
            OriginPoint = Position + new Vector2(8, 8);

            HealthMax = healthMax;
            Health = HealthMax;
            PlayerItems = new List<PlayerActorItem>();

            IsVisible = true;

            CreateAnimationDictionary();
            UpdateBoundingBoxPosition();

            TransitionMoveState = AreaTransitionMoveState.Start;
            Facing = PlayerFacing.Down;
        }

        public void Update(GameTime gameTime)
        {
            float _deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(AnimationOverrideTimer <= 0 && IsAnimationOverride == true)
            {
                ReleaseAnimationOverride();
            }
            else
            {
                AnimationOverrideTimer -= _deltaTime;
            }

            HandleInput();
            Vector2 _position = new Vector2(0);

            if (ControlKeys["UpKey"] == true)
            {
                if (IsAnimationOverride == false)
                {
                    CurrentAnimation = "MoveUp";
                }
                _position = _position + new Vector2(0, -MoveSpeed);
                Facing = PlayerFacing.Up;
            }

            if (ControlKeys["DownKey"] == true)
            {
                if (IsAnimationOverride == false)
                {
                    CurrentAnimation = "MoveDown";
                }
                _position = _position + new Vector2(0, MoveSpeed);
                Facing = PlayerFacing.Down;
            }

            if (ControlKeys["RightKey"] == true)
            {
                if (IsAnimationOverride == false)
                {
                    CurrentAnimation = "MoveRight";
                }
                _position = _position + new Vector2(MoveSpeed, 0);
                Facing = PlayerFacing.Right;
            }

            if (ControlKeys["LeftKey"] == true)
            {
                if (IsAnimationOverride == false)
                {
                    CurrentAnimation = "MoveLeft";
                }
                _position = _position + new Vector2(-MoveSpeed, 0);
                Facing = PlayerFacing.Left;
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

        private void CreateControlConfiguration()
        {
            ControlKeys = new Dictionary<string, bool>();

            ControlKeys.Add("UpKey", false);
            ControlKeys.Add("DownKey", false);
            ControlKeys.Add("RightKey", false);
            ControlKeys.Add("LeftKey", false);
            ControlKeys.Add("AttackKey", false);
            ControlKeys.Add("StartKey", false);
        }
        public void HandleInput()
        {
            switch (ControlStyle)
            {
                case PlayerController.Keyboard:
                    {
                        KeyboardState _keystate = Keyboard.GetState();

                        if (_keystate.IsKeyDown(Keys.Up) == true)
                        {
                            ControlKeys["UpKey"] = true;
                        }
                        else
                        {
                            ControlKeys["UpKey"] = false;
                        }

                        if (_keystate.IsKeyDown(Keys.Down) == true)
                        {
                            ControlKeys["DownKey"] = true;
                        }
                        else
                        {
                            ControlKeys["DownKey"] = false;
                        }

                        if (_keystate.IsKeyDown(Keys.Right) == true)
                        {
                            ControlKeys["RightKey"] = true;
                        }
                        else
                        {
                            ControlKeys["RightKey"] = false;
                        }

                        if (_keystate.IsKeyDown(Keys.Left) == true)
                        {
                            ControlKeys["LeftKey"] = true;
                        }
                        else
                        {
                            ControlKeys["LeftKey"] = false;
                        }
                        break;
                    }
                case PlayerController.Gamepad:
                    {
                        GamePadState gamePadState = GamePad.GetState(ControllerIndex);

                        if (gamePadState.IsButtonDown(Buttons.DPadUp) == true)
                        {
                            ControlKeys["UpKey"] = true;
                        }
                        else
                        {
                            ControlKeys["UpKey"] = false;
                        }

                        if (gamePadState.IsButtonDown(Buttons.DPadDown) == true)
                        {
                            ControlKeys["DownKey"] = true;
                        }
                        else
                        {
                            ControlKeys["DownKey"] = false;
                        }

                        if (gamePadState.IsButtonDown(Buttons.DPadRight) == true)
                        {
                            ControlKeys["RightKey"] = true;
                        }
                        else
                        {
                            ControlKeys["RightKey"] = false;
                        }

                        if (gamePadState.IsButtonDown(Buttons.DPadLeft) == true)
                        {
                            ControlKeys["LeftKey"] = true;
                        }
                        else
                        {
                            ControlKeys["LeftKey"] = false;
                        }
                        break;
                    }
            }
        }

        public void Activate()
        {
            IsActive = true;
        }
        public void Deactivate()
        {
            IsActive = false;
        }
        public void SetVisibility(bool visible)
        {
            IsVisible = visible;
        }

        private void CreateAnimationDictionary()
        {
            Animations = new Dictionary<string, Animation>();

            Animations.Add("MoveUp", new Animation(AssetManager.SpriteSheets["PlayerSheet"], 2, 16, 16, 0.2f, new Vector2(32, 160)));
            Animations.Add("MoveDown", new Animation(AssetManager.SpriteSheets["PlayerSheet"], 2, 16, 16, 0.2f, new Vector2(0, 160)));
            Animations.Add("MoveLeft", new Animation(AssetManager.SpriteSheets["PlayerSheet"], 2, 16, 16, 0.2f, new Vector2(96, 160)));
            Animations.Add("MoveRight", new Animation(AssetManager.SpriteSheets["PlayerSheet"], 2, 16, 16, 0.2f, new Vector2(64, 160)));

            Animations.Add("SwordDown", new Animation(AssetManager.SpriteSheets["PlayerSheet"], 2, 16, 16, 0.2f, new Vector2(0, 176)));
            Animations.Add("SwordUp", new Animation(AssetManager.SpriteSheets["PlayerSheet"], 2, 16, 16, 0.2f, new Vector2(32, 176)));
            Animations.Add("SwordRight", new Animation(AssetManager.SpriteSheets["PlayerSheet"], 2, 16, 16, 0.2f, new Vector2(64, 176)));
            Animations.Add("SwordLeft", new Animation(AssetManager.SpriteSheets["PlayerSheet"], 2, 16, 16, 0.2f, new Vector2(96, 176)));

            Animations.Add("PushingDown", new Animation(AssetManager.SpriteSheets["PlayerSheet"], 2, 16, 16, 0.2f, new Vector2(0, 192)));
            Animations.Add("PushingUp", new Animation(AssetManager.SpriteSheets["PlayerSheet"], 2, 16, 16, 0.2f, new Vector2(32, 192)));
            Animations.Add("PushingRight", new Animation(AssetManager.SpriteSheets["PlayerSheet"], 2, 16, 16, 0.2f, new Vector2(64, 192)));
            Animations.Add("PushingLeft", new Animation(AssetManager.SpriteSheets["PlayerSheet"], 2, 16, 16, 0.2f, new Vector2(96, 192)));

            Animations.Add("PullingDown", new Animation(AssetManager.SpriteSheets["PlayerSheet"], 2, 16, 16, 0.2f, new Vector2(0, 208)));
            Animations.Add("PullingUp", new Animation(AssetManager.SpriteSheets["PlayerSheet"], 2, 16, 16, 0.2f, new Vector2(32, 208)));
            Animations.Add("PullingRight", new Animation(AssetManager.SpriteSheets["PlayerSheet"], 2, 16, 16, 0.2f, new Vector2(64, 208)));
            Animations.Add("PullingLeft", new Animation(AssetManager.SpriteSheets["PlayerSheet"], 2, 16, 16, 0.2f, new Vector2(96, 208)));

            Animations.Add("CarryDown", new Animation(AssetManager.SpriteSheets["PlayerSheet"], 2, 16, 16, 0.2f, new Vector2(0, 224)));
            Animations.Add("CarryUp", new Animation(AssetManager.SpriteSheets["PlayerSheet"], 2, 16, 16, 0.2f, new Vector2(32, 224)));
            Animations.Add("CarryRight", new Animation(AssetManager.SpriteSheets["PlayerSheet"], 2, 16, 16, 0.2f, new Vector2(64, 224)));
            Animations.Add("CarryLeft", new Animation(AssetManager.SpriteSheets["PlayerSheet"], 2, 16, 16, 0.2f, new Vector2(96, 224)));

            CurrentAnimation = "MoveDown";

            AnimationManager = new AnimationManager(Animations[CurrentAnimation]);
            AnimationManager.Play(Animations[CurrentAnimation]);

            
        }
        public void AnimationOverride(string animationName)
        {
            CurrentAnimation = animationName;
            AnimationManager.Play(Animations[CurrentAnimation]);
            AnimationOverrideTimer = 0.1f;
            IsAnimationOverride = true;
        }
        public void ReleaseAnimationOverride()
        {
            IsAnimationOverride = false;
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
                                movePosition = movePosition + new Vector2(0, (32 * deltaTime));
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
                                movePosition = movePosition + new Vector2(0, -(32 * deltaTime));
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
                                movePosition = movePosition + new Vector2((32 * deltaTime), 0);
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
                                movePosition = movePosition + new Vector2(-(32 * deltaTime), 0);
                            }

                            break;
                        }
                }

                Position = Position + movePosition;
                UpdateBoundingBoxPosition();

                AnimationManager.Update(gameTime);
            }
        }
        public void ResetAreaTransitionState()
        {
            TransitionMoveState = AreaTransitionMoveState.Start;
        }

        public void SetPosition(Vector2 newPosition)
        {
            Position = newPosition;
        }
        public void SetFacing(string facing)
        {
            switch (facing)
            {
                case "Up":
                    {
                        CurrentAnimation = "MoveUp";
                        Facing = PlayerFacing.Up;
                        break;
                    }
                case "Down":
                    {
                        CurrentAnimation = "MoveDown";
                        Facing = PlayerFacing.Down;
                        break;
                    }
                case "Right":
                    {
                        CurrentAnimation = "MoveRight";
                        Facing = PlayerFacing.Right;
                        break;
                    }
                case "Left":
                    {
                        CurrentAnimation = "MoveLeft";
                        Facing = PlayerFacing.Left;
                        break;
                    }
            }
        }
        public void LoadGameHandler()
        {
            // USE THIS TO LOAD PLAYER FROM A SAVEGAME
        }
    }
}
