using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ProjectLondon
{
    public class MapAreaDefinition : MapObject
    {
        public string Name { get; protected set; }
        public Vector2 Position { get; protected set; }
        public Rectangle BoundingBox { get; protected set; }

        public MapAreaDefinition(string name, Vector2 position, Rectangle areaRectangle)
        {
            Name = name;
            Position = position;
            BoundingBox = areaRectangle;
        }
    }
}
