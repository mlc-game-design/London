using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended.Tiled;
using Microsoft.Xna.Framework;

namespace ProjectLondon
{
    public class MapTransitionHandler : MapObject
    {
        private string Type { get; set; }
        public string DestinationMap { get; private set; }
        public Vector2 DestinationPosition { get; private set; }
        public Rectangle BoundingBox { get; private set; }

        public MapTransitionHandler(string type, string destinationMap, Vector2 destinationPosition, Rectangle boundingBox)
        {
            Type = type;
            DestinationMap = destinationMap;
            DestinationPosition = destinationPosition;
            BoundingBox = boundingBox;
        }

        public void Update(GameTime gameTime)
        {
            // Handle the Transition
        }
    }
}
