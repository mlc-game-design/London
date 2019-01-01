using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ProjectLondon
{
    public class Animation
    {
        #region Properties
        public int CurrentFrame { get; set; }
        public int FrameCount { get; protected set; }
        public int FrameHeight { get; protected set; }
        public float FrameSpeed { get; protected set; }
        public int FrameWidth { get; protected set; }
        public bool IsLooping { get; protected set; }
        public Vector2 FrameStartPosition { get; protected set; }
        public Texture2D Texture { get; protected set; }
        #endregion

        public Animation(Texture2D texture, int frameCount, int frameHeight, int frameWidth, float frameSpeed)
        {
            CurrentFrame = 0;
            Texture = texture;
            FrameCount = frameCount;
            FrameHeight = frameHeight;
            FrameWidth = frameWidth;
            IsLooping = true;
            FrameSpeed = frameSpeed;
            FrameStartPosition = new Vector2(0, 0);
        }

        public Animation(Texture2D texture, int frameCount, int frameHeight, int frameWidth, float frameSpeed, Vector2 frameStartPosition)
        {
            CurrentFrame = 0;
            Texture = texture;
            FrameCount = frameCount;
            FrameHeight = frameHeight;
            FrameWidth = frameWidth;
            IsLooping = true;
            FrameSpeed = frameSpeed;
            FrameStartPosition = frameStartPosition;
        }
    }

    public class AnimationManager
    {
        private Animation _animation;
        private float _timer;
        public Vector2 Position { get; protected set; }

        public AnimationManager(Animation animation)
        {
            _animation = animation;
        }

        public void Play(Animation animation)
        {
            if(_animation == animation)
            {
                return;
            }

            _animation = animation;
            _animation.CurrentFrame = 0;
            _timer = 0;
        }

        public void Stop()
        {
            _timer = 0;
            _animation.CurrentFrame = 0;
        }

        public void Update(GameTime gameTime)
        {
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(_timer > _animation.FrameSpeed)
            {
                _timer = 0f;
                _animation.CurrentFrame++;

                if(_animation.CurrentFrame >= _animation.FrameCount)
                {
                    _animation.CurrentFrame = 0;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.Draw(_animation.Texture, position,
                new Rectangle(Convert.ToInt32(_animation.FrameStartPosition.X) + (_animation.CurrentFrame * _animation.FrameWidth),
                Convert.ToInt32(_animation.FrameStartPosition.Y), _animation.FrameWidth, _animation.FrameHeight),
                Color.White);
        }
    }
}
