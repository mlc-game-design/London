using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ProjectLondon
{
    public class MapAreaDefinition : MapObject, IMapObject
    {
        public string Name { get; protected set; }

        public MapAreaDefinition(string name, Vector2 position, Rectangle areaRectangle)
        {
            Type = "MapAreaDefinition";
            Name = name;
            Position = position;
            BoundingBox = areaRectangle;
        }
    }
}
