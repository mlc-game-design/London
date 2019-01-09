using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ProjectLondon
{
    public class MapCollisionSolidStatic : MapCollisionObject
    {
        public MapCollisionSolidStatic(Vector2 position, int width, int height)
        {
            Type = "MapCollisionStatic";
            Position = position;
            BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, width, height);
        }
    }
}
