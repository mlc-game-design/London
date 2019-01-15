using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectLondon
{
    public class Animation : ICloneable
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

        public void GotoNextFrame()
        {
            CurrentFrame = CurrentFrame + 1;

            if (CurrentFrame >= FrameCount)
            {
                CurrentFrame = 0;
            }
        }

        
        public object Clone()
        {
            return new Animation(Texture, FrameCount, FrameHeight, FrameWidth, FrameSpeed, FrameStartPosition);
        }
    }
}
