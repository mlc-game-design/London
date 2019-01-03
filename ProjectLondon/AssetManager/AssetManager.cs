using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace ProjectLondon
{
    public static class AssetManager
    {
        public static readonly Dictionary<string, Texture2D> SpriteSheets = new Dictionary<string, Texture2D>();
        public static readonly Dictionary<string, bool> MapSwitches = new Dictionary<string, bool>();

        public static void PopulateLists(ContentManager content)
        {
            PopulateSpritesheets(content);
            CreateBaseDictionaries();
        }

        public static void PopulateSpritesheets(ContentManager content)
        {
            Texture2D playerSheet = content.Load<Texture2D>("spritesheets//GBC_LA_Link_Sheet");

            SpriteSheets.Add("PlayerSheet", playerSheet);
        }

        public static void CreateBaseDictionaries()
        {
            
        }
    }
}
