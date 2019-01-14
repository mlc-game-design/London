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
        private AnimationLibrary AnimationLibrary { get; set; }
        private string CurrentAnimation { get; set; }

        public MapEntityStatic(bool isSolid, Vector2 position, int width, int height, string animationLibraryName)
        {
            IsSolid = isSolid;
            AnimationLibrary = new AnimationLibrary(animationLibraryName);
            AnimationManager = null;

            Position = position;

            BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, width, height);
        }

        public void ConstructAnimationLibrary(string name, string currentAnimation)
        {
            AnimationLibrary = new AnimationLibrary(name, AssetManager.GetAnimationLibrary(name).Animations);

            CurrentAnimation = currentAnimation;

            AnimationManager = new AnimationManager(AnimationLibrary);
            AnimationManager.Play(CurrentAnimation);
        }

        public override void Update(GameTime gameTime)
        {
            AnimationManager.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            AnimationManager.Draw(spriteBatch, Position);
        }
    }
}
