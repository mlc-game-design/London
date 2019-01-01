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
        private Vector2 Position { get; set; }

        public Rectangle BoundingBox { get; protected set; }

        public float Health { get; protected set; }
        private float HealthMax { get; set; }

        AnimationManager AnimationManager { get; set; }

        private Dictionary<string, Animation> Animations;
        private string CurrentAnimation;

        public PlayerActor(ContentManager content, Vector2 position, float healthMax)
        {
            Content = content;
            ControllerIndex = PlayerIndex.One;

            MoveSpeed = 1.0f;
            Position = position;

            HealthMax = healthMax;
            Health = HealthMax;

            CreateAnimationDictionary();

            UpdateBoundingBoxPosition();
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

            AnimationManager.Play(Animations[CurrentAnimation]);
            AnimationManager.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            AnimationManager.Draw(spriteBatch, Position);
        }

        private void CreateAnimationDictionary()
        {
            Animations = new Dictionary<string, Animation>();

            Animations.Add("MoveUp", new Animation(AssetManager.SpriteSheets["PlayerSheet"], 2, 16, 15, 0.3f, new Vector2(65, 36)));
            Animations.Add("MoveDown", new Animation(AssetManager.SpriteSheets["PlayerSheet"], 2, 16, 15, 0.3f, new Vector2(36, 36)));
            Animations.Add("MoveLeft", new Animation(AssetManager.SpriteSheets["PlayerSheet"], 2, 16, 15, 0.3f, new Vector2(6, 36)));
            Animations.Add("MoveRight", new Animation(AssetManager.SpriteSheets["PlayerSheet"], 2, 16, 15, 0.3f, new Vector2(94, 36)));

            CurrentAnimation = "MoveDown";

            AnimationManager = new AnimationManager(Animations[CurrentAnimation]);
            AnimationManager.Play(Animations[CurrentAnimation]);
        }

        private void UpdateBoundingBoxPosition()
        {
            BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, 15, 16);
        }
    }
}
