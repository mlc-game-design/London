using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace ProjectLondon
{
    public class MapEntityStatic : MapEntity
    {
        private AnimationManager AnimationManager { get; set; }
        private AnimationBook AnimationBook { get; set; }
        private string CurrentAnimation { get; set; }

        public MapEntityStatic(bool isSolid, Vector2 position, int width, int height)
        {
            IsAnimated = false;

            IsSolid = isSolid;
            Position = position;

            BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, width, height);

            AnimationBook = null;
            AnimationManager = null;
        }
        public MapEntityStatic(bool isSolid, Vector2 position, int width, int height, string animationLibraryName)
        {
            IsAnimated = true;

            IsSolid = isSolid;
            AnimationBook = new AnimationBook(animationLibraryName);
            AnimationManager = null;

            Position = position;

            BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, width, height);
        }

        public void ConstructAnimationLibrary(string name, string currentAnimation)
        {
            AnimationBook = new AnimationBook(name, AnimationLibrary.GetAnimationLibrary(name).Animations);

            CurrentAnimation = currentAnimation;

            AnimationManager = new AnimationManager(AnimationBook);
            AnimationManager.Play(CurrentAnimation);
        }

        public override void Update(GameTime gameTime)
        {
            if (IsAnimated == false)
            {
                return;
            }

            AnimationManager.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsAnimated == false)
            {
                return;
            }

            AnimationManager.Draw(spriteBatch, Position);
        }
    }
}
