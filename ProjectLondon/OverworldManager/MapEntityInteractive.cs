using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ProjectLondon
{
    public abstract class MapEntityInteractive
    {
        public string SubType { get; protected set; }
        public bool IsActive { get; protected set; }
        public Vector2 Position { get; protected set; }
        public Rectangle BoundingBox { get; protected set; }
    }
}
