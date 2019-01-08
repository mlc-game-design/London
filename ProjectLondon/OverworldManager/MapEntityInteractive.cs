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
    public class MapEntityInteractive : MapEntity
    {
        private AnimationManager AnimationManager { get; set; }
        private Dictionary<string, Animation> Animations { get; set; }
        private string CurrentAnimation { get; set; }

        public MapEntityInteractive(bool isSolid, Vector2 position, int width, int height)
        {
            IsSolid = isSolid;
            Animations = new Dictionary<string, Animation>();

            Position = position;

            BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, width, height);
        }

        public void CreateAnimationDictionary(Dictionary<string, Animation> animations, string currentAnimation)
        {
            Animations = new Dictionary<string, Animation>(animations);

            CurrentAnimation = currentAnimation;

            AnimationManager = new AnimationManager(Animations[CurrentAnimation]);
            AnimationManager.Play(Animations[CurrentAnimation]);
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
