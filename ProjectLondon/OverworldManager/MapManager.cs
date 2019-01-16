using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;

namespace ProjectLondon
{
    public static class MapManager
    {
        private static ContentManager Content { get; set; }
        public static TiledMap MapCurrent { get; private set; }
        public static MapManagerStore Store { get; private set; }

        private const string MapStoreFolder = "maps\\";
        private const string BGMStoreFolder = "bgm\\";

        public static void Intialize(ContentManager content)
        {
            Content = content;
            MapCurrent = null;
            Store = null;

        }

        private static string CreateMapAssetPath(string assetName)
        {
            string _fullPath = MapStoreFolder + assetName;
            return _fullPath;
        }
        private static string CreateBGMAssetPath(string assetName)
        {
            string _fullpath = BGMStoreFolder + assetName;
            return _fullpath;
        }

        public static MapManagerStore LoadMap(string assetName)
        {
            string _mapPath = CreateMapAssetPath(assetName);
            MapCurrent = Content.Load<TiledMap>(_mapPath);

            LoadMapObjectLayers(MapCurrent);

            SetAreaBoundaries(MapCurrent.Properties["Default Area"]);

            if (MapBackgroundMusic == null)
            {
                MapBackgroundMusic = Content.Load<Song>("bgm//" + MapCurrent.Properties["mapBackgroundMusic"]);
                MediaPlayer.Volume = 0.4f;
                MediaPlayer.Play(MapBackgroundMusic);
            }
            else
            {
                if (MapCurrent.Properties["mapBackgroundMusic"] != MapBackgroundMusicAssetName)
                {
                    MapBackgroundMusic = Content.Load<Song>("bgm//" + MapCurrent.Properties["mapBackgroundMusic"]);
                }

                MediaPlayer.Volume = 0.4f;
                MediaPlayer.Play(MapBackgroundMusic);
            }

            MapBackgroundMusicAssetName = MapCurrent.Properties["mapBackgroundMusic"];

            return _mapManagerStore;
        }
        private static void LoadMapObjectLayers(TiledMap tiledMap)
        {
            TiledMapObjectLayer _mapEntityObjectLayer = MapCurrent.GetLayer<TiledMapObjectLayer>("Entity Layer");
            TiledMapObjectLayer _mapCollisionLayer = MapCurrent.GetLayer<TiledMapObjectLayer>("Collision Layer");
            TiledMapObjectLayer _mapAreaDefinitionLayer = MapCurrent.GetLayer<TiledMapObjectLayer>("Area Definitions");

            foreach (TiledMapObject _entityObject in _mapEntityObjectLayer.Objects)
            {
                switch (_entityObject.Name)
                {
                    case "mapPlayerSpawn":
                        {
                            // Run Player Spawn Code Here
                            PlayerSpawnX = Convert.ToInt32(_entityObject.Position.X);
                            PlayerSpawnY = Convert.ToInt32(_entityObject.Position.Y);
                            IsPlayerSpawn = true;

                            break;
                        }
                    case "mapTransition":
                        {
                            // Create Transition Objects
                            float destinationX, destinationY;

                            destinationX = Convert.ToInt32(_entityObject.Properties["mapDestinationX"]);
                            destinationY = Convert.ToInt32(_entityObject.Properties["mapDestinationY"]);

                            MapTransitionHandler mapTransition = new MapTransitionHandler(contentManager, _entityObject.Properties["mapDestination"],
                                new Vector2((float)destinationX, (float)destinationY), new Rectangle((int)_entityObject.Position.X, (int)_entityObject.Position.Y, (int)_entityObject.Size.Width, (int)_entityObject.Size.Height),
                                _entityObject.Properties["mapDestinationArea"], _entityObject.Properties["mapDestinationFacing"]);

                            MapObjects.Add(mapTransition);
                            break;
                        }
                    case "mapEntitySpawn":
                        {
                            // Get AssetManager Data
                            AnimationBook _animationLibrary = AnimationLibrary.GetAnimationLibrary(_entityObject.Properties["AnimationLibraryName"]);

                            // Spawn the Entity
                            bool isSolid = Convert.ToBoolean(_entityObject.Properties["IsSolid"]);

                            MapEntityStatic _mapEntity = new MapEntityStatic(isSolid, new Vector2(_entityObject.Position.X, _entityObject.Position.Y), (int)_entityObject.Size.Width, (int)_entityObject.Size.Height, _entityObject.Properties["AnimationLibraryName"]);
                            _mapEntity.ConstructAnimationLibrary(_animationLibrary.Name, _entityObject.Properties["CurrentAnimation"]);

                            Entities.Add(_mapEntity);
                            break;
                        }
                }
            }
            foreach (TiledMapObject _collisionObject in _mapCollisionLayer.Objects)
            {
                switch (_collisionObject.Name)
                {
                    case "solidStatic":
                        {
                            MapCollisionSolidStatic solid = new MapCollisionSolidStatic(_collisionObject.Position, (int)_collisionObject.Size.Width, (int)_collisionObject.Size.Height);
                            CollisionObjects.Add(solid);
                            break;
                        }
                }
            }
            foreach (TiledMapObject _mapEntityArea in _mapAreaDefinitionLayer.Objects)
            {
                MapEntityArea area = new MapEntityArea(_mapEntityArea.Name, new Vector2((int)_mapEntityArea.Position.X, (int)_mapEntityArea.Position.Y),
                    new Rectangle((int)_mapEntityArea.Position.X, (int)_mapEntityArea.Position.Y, (int)_mapEntityArea.Size.Width, (int)_mapEntityArea.Size.Height));

                Areas.Add(area);
            }
        }

        public static void PlayBGM()
        {
            MediaPlayer.Volume = 1.0f;
            MediaPlayer.Play(Store.BackgroundMusic);
        }
        public static void PlayBGM(float volume)
        {
            MediaPlayer.Volume = volume;
            MediaPlayer.Play(Store.BackgroundMusic);
        }

        public static Rectangle SetAreaBoundaries(string areaName)
        {
            Rectangle _currentArea = null;

            _currentArea = Store.GetAreaByName(areaName);

            if (ActiveArea != null)
            {
                ActiveArea = _currentArea.BoundingBox;
            }
            else
            {
                ActiveArea = new Rectangle((int)_currentArea.X, (int)_currentArea.Y, (int)_currentArea.Width, (int)_currentArea.Height);
            }
        }
    }
}
