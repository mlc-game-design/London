using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Tiled;

namespace ProjectLondon
{
    public static class MapManager
    {
        private static ContentManager Content;
        public static TiledMap MapCurrent;

        private const string MapStoreFolder = "maps\\";

        public static void Intialize(ContentManager content)
        {
            Content = content;
            MapCurrent = null;
        }
        private static string CreateAssetPath(string assetName)
        {
            string _fullPath = MapStoreFolder + assetName;
            return _fullPath;
        }

        public static MapManagerStore LoadMap(string assetName)
        {
            MapManagerStore _mapManagerStore = null;

            string _assetPath = CreateAssetPath(assetName);
            MapCurrent = Content.Load<TiledMap>(_assetPath);
            LoadMapObjectLayers(MapCurrent);

            if (MapCurrent.Properties.ContainsKey("startZone") == true)
            {
                SetAreaBoundaries(MapCurrent.Properties["startZone"]);
            }

            if (MapBackgroundMusic == null)
            {
                MapBackgroundMusic = contentManager.Load<Song>("bgm//" + MapCurrent.Properties["mapBackgroundMusic"]);
                MediaPlayer.Volume = 0.4f;
                MediaPlayer.Play(MapBackgroundMusic);
            }
            else
            {
                if (MapCurrent.Properties["mapBackgroundMusic"] != MapBackgroundMusicAssetName)
                {
                    MapBackgroundMusic = contentManager.Load<Song>("bgm//" + MapCurrent.Properties["mapBackgroundMusic"]);
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

        


    }
}
