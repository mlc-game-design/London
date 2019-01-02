using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLondon
{
    public class MapObjectSolid : MapObject
    {
        public Vector2 Position { get; protected set; }
        public Rectangle BoundingBox { get; protected set; }

        public MapObjectSolid(Vector2 position, int width, int height)
        {
            Position = position;
            BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, width, height);
        }
    }
}
