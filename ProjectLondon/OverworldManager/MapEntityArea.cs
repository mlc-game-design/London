using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ProjectLondon
{
    public class MapEntityArea : MapObject, IMapObject
    {
        public string Name { get; protected set; }

        public MapEntityArea(string name, Vector2 position, Rectangle areaRectangle)
        {
            Type = "MapEntityArea";
            Name = name;
            Position = position;
            BoundingBox = areaRectangle;
        }
    }
}
