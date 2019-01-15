using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Tiled;

namespace ProjectLondon
{
    public static class MapManager
    {
        private static ContentManager Content;

        private const string MapStoreFolder = "maps\\";

        public static void Intialize(ContentManager content)
        {
            Content = content;
        }

        public static MapManagerStore LoadMap(string assetPath)
        {
            MapManagerStore _mapManagerStore = null;

            TiledMap _newMap = Content.Load<TiledMap>(assetPath);



            return _mapManagerStore;
        }
    }
}
