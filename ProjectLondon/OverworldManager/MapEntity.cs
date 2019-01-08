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
    public abstract class MapEntity
    {
        public bool IsAnimated { get; protected set; }
        public bool IsSolid { get; protected set; }

        public Vector2 Position { get; protected set; }
        public Rectangle BoundingBox { get; protected set; }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
