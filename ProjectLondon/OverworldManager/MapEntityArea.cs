using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectLondon
{
    public class MapEntityArea : MapEntity, IMapEntity
    {
        public string Name { get; protected set; }

        public MapEntityArea(string name, Vector2 position, Rectangle areaRectangle)
        {
            Type = "MapEntityArea";
            Name = name;
            Position = position;
            BoundingBox = areaRectangle;
        }

        public void Update(GameTime gameTime)
        {
            return;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            return;
        }

        public IMapEntity Clone()
        {
            return new MapEntityArea(Name, Position, BoundingBox);
        }
    }
}
