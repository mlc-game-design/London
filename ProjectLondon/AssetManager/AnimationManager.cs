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
    public class AnimationManager
    {
        private Animation Animation;
        private float FrameTimer;

        public AnimationManager(Animation animation)
        {
            Animation = animation;
        }

        public void Play(Animation animation)
        {
            if(Animation == animation)
            {
                return;
            }

            Animation = animation;
            Animation.CurrentFrame = 0;
            FrameTimer = 0;
        }

        public void Stop()
        {
            FrameTimer = 0;
            Animation.CurrentFrame = 0;
        }

        public void Update(GameTime gameTime)
        {
            FrameTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(FrameTimer >= Animation.FrameSpeed)
            {
                FrameTimer = 0f;
                Animation.GotoNextFrame();
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.Draw(Animation.Texture, position,
                new Rectangle((int)Animation.FrameStartPosition.X + (Animation.CurrentFrame * Animation.FrameWidth),
                (int)Animation.FrameStartPosition.Y, Animation.FrameWidth, Animation.FrameHeight),
                Color.White);
        }
    }
}
