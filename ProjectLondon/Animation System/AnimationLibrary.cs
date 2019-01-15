using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace ProjectLondon
{
    public static class AnimationLibrary
    {
        public static readonly Dictionary<string, Texture2D> SpriteSheets = new Dictionary<string, Texture2D>();
        public static readonly Dictionary<string, bool> MapEntitySwitches = new Dictionary<string, bool>();
        public static readonly List<AnimationBook> AnimationLibraries = new List<AnimationBook>();

        public static void PopulateLists(ContentManager content)
        {
            PopulateSpritesheets(content);
            CreateBaseDictionaries();
        }

        public static void PopulateSpritesheets(ContentManager content)
        {
            Texture2D entitySheet = content.Load<Texture2D>("spritesheets//Sprites");
            Texture2D dungeonSheet = content.Load<Texture2D>("mapTiles//Dungeons");
            Texture2D overworldLight = content.Load<Texture2D>("mapTiles//Overworld_Light");

            SpriteSheets.Add("PlayerSheet", entitySheet);
            SpriteSheets.Add("Dungeons", dungeonSheet);
            SpriteSheets.Add("OverworldLight", overworldLight);
        }

        public static void CreateBaseDictionaries()
        {
            BuildEntityAnimationDictionaries();
            BuildMapEntitySwitchDictionary();
        }

        private static void BuildEntityAnimationDictionaries()
        {
            Dictionary<string, Animation> _dungeonTorchAnimations = new Dictionary<string, Animation>();
            _dungeonTorchAnimations.Add("Idle", new Animation(SpriteSheets["Dungeons"], 4, 16, 16, 0.2f, new Vector2(0, 16)));

            AnimationBook _dungeonTorchLibrary = new AnimationBook("DungeonTorch", _dungeonTorchAnimations);

            AnimationLibraries.Add(_dungeonTorchLibrary);
        }
        private static void BuildMapEntitySwitchDictionary()
        {
            MapEntitySwitches.Add("TestDungeon001", false);
        }

        public static AnimationBook GetAnimationLibrary(string libraryName)
        {
            foreach (AnimationBook al in AnimationLibraries)
            {
                if(al.Name == libraryName)
                {
                    return al;
                }
            }

            return null;
        }
    }
}
