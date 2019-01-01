using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;
using System.Linq;

namespace ProjectLondon
{
    public class OverworldManager
    {
        ContentManager contentManager;
        GraphicsDevice graphicsDevice;

        TiledMap MapCurrent;
        TiledMapRenderer MapRenderer;
        public Camera2D MapCamera { get; private set; }

        List<TiledMap> AdjacentMaps;
        List<MapObject> MapObjects;

        Song MapBackgroundMusic;
        string MapBackgroundMusicAssetName;

        public bool IsPlayerSpawn { get; private set; }
        public int PlayerSpawnX, PlayerSpawnY;

        public OverworldManager(GraphicsDevice gfxDevice, ContentManager content)
        {
            graphicsDevice = gfxDevice;
            contentManager = content;

            MapCurrent = null;
            MapRenderer = new TiledMapRenderer(graphicsDevice);
            MapCamera = new Camera2D(graphicsDevice);
            MapCamera.Origin = new Vector2(0, 0);
            MapCamera.Position = new Vector2(0, 0);
            MapCamera.Zoom = 4.0f;

            AdjacentMaps = new List<TiledMap>();
            MapObjects = new List<MapObject>();

            MapBackgroundMusic = null;
            MapBackgroundMusicAssetName = "";

            IsPlayerSpawn = false;
        }

        /// <summary>
        /// Loads TiledMap into memory by MapName string
        /// </summary>
        /// <param name="mapName"></param>
        public void LoadMap(string mapName)
        {
            MapCurrent = contentManager.Load<TiledMap>(mapName);
            LoadMapObjects();
            LoadAdjacentMaps();

            if (MapBackgroundMusic == null)
            {
                MapBackgroundMusic = contentManager.Load<Song>("bgm//" + MapCurrent.Properties["mapBackgroundMusic"]);
                MediaPlayer.Volume = 1.0f;
                MediaPlayer.Play(MapBackgroundMusic);
            }
            else
            {
                if (MapCurrent.Properties["mapBackgroundMusic"] != MapBackgroundMusicAssetName)
                {
                    MapBackgroundMusic = contentManager.Load<Song>("bgm//" + MapCurrent.Properties["mapBackgroundMusic"]);
                }

                MediaPlayer.Volume = 1.0f;
                MediaPlayer.Play(MapBackgroundMusic);
            }

            MapBackgroundMusicAssetName = MapCurrent.Properties["mapBackgroundMusic"];
        }
        
        /// <summary>
        /// Iterates through list of adjacent maps, loading each one into memory
        /// </summary>
        /// <returns></returns>
        private TiledMap LoadAdjacentMaps()
        {
            // Unload existing AdjacentMaps list
            if(AdjacentMaps.Count > 0)
            {
                for(int i = AdjacentMaps.Count - 1; i >= 0; i--)
                {
                    AdjacentMaps.RemoveAt(i);
                }
            }

            // Get the TiledMap Property for "adjacentMaps"
            string mapListString = MapCurrent.Properties["adjacentMaps"];

            // Separate the string at each semicolon (;) and create a List using set of strings
            List<string> mapNames = mapListString.Split(';').ToList();
            
            // Take List<string> compiled and load each map without MapObjects 
            foreach(string mapName in mapNames)
            {
                // Load Each Map

            }

            return null;
        }

        /// <summary>
        /// Loads MapObjects for the Active Map
        /// </summary>
        private void LoadMapObjects()
        {
            TiledMapObjectLayer mapObjectLayer = (TiledMapObjectLayer)MapCurrent.GetLayer("MapObjects");

            foreach (TiledMapObject mapObject in mapObjectLayer.Objects)
            {
                switch (mapObject.Name)
                {
                    case "mapPlayerSpawn":
                        {
                            // Run Player Spawn Code Here
                            PlayerSpawnX = Convert.ToInt32(mapObject.Position.X);
                            PlayerSpawnY = Convert.ToInt32(mapObject.Position.Y);
                            IsPlayerSpawn = true;
                            break;
                        }
                    case "mapTransition":
                        {
                            // Create Transition Objects
                            float destinationX, destinationY;

                            destinationX = Convert.ToInt32(mapObject.Properties["mapDestinationX"]);
                            destinationY = Convert.ToInt32(mapObject.Properties["mapDestinationY"]);

                            MapTransitionHandler mapTransition = new MapTransitionHandler(mapObject.Type, mapObject.Properties["mapDestination"],
                                new Vector2((float)destinationX, (float)destinationY), new Rectangle((int)mapObject.Position.X, (int)mapObject.Position.Y, (int)mapObject.Size.Width, (int)mapObject.Size.Height));

                            MapObjects.Add(mapTransition);
                            break;
                        }
                    case "mapEntitySpawn":
                        {
                            // Spawn the Entity

                            break;
                        }
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            MapRenderer.Update(MapCurrent, gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            MapRenderer.Draw(MapCurrent, MapCamera.GetViewMatrix());
        }
    }
}
