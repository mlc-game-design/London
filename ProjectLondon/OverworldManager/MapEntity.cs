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
        public string Type { get; protected set; }
        public Vector2 Position { get; protected set; }
        public Rectangle BoundingBox { get; protected set; }
        public bool IsAnimated { get; protected set; }
        public bool IsSolid { get; protected set; }
    }
}
