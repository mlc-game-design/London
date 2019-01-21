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

        private static string LoadedSongName { get; set; }

        public static Rectangle ActiveArea { get; private set; }

        public static MapHandlingState State { get; private set; }
        public enum MapHandlingState
        {
            Normal,
            AreaTransition,
            MapChange
        }

        public static void Intialize(ContentManager content)
        {
            Content = content;
            MapCurrent = null;
            Store = null;
            LoadedSongName = "";
            ActiveArea = new Rectangle(0, 0, 0, 0);
            State = MapHandlingState.Normal;
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
            Store = new MapManagerStore(Content);

            string _mapPath = CreateMapAssetPath(assetName);
            MapCurrent = Content.Load<TiledMap>(_mapPath);

            LoadMapObjectLayers(Store, MapCurrent);

            SetAreaBoundaries(MapCurrent.Properties["Default Area"]);

            if ((LoadedSongName == "") || (LoadedSongName != MapCurrent.Properties["mapBackgroundMusic"]))
            {
                Song _song = Content.Load<Song>("bgm//" + MapCurrent.Properties["mapBackgroundMusic"]);
                float _volume = float.Parse(MapCurrent.Properties["Music Volume"]);
                Store.PlayBGM(_song, _volume);
            }

            LoadedSongName = MapCurrent.Properties["mapBackgroundMusic"];

            return Store;
        }
        private static void LoadMapObjectLayers(MapManagerStore store, TiledMap tiledMap)
        {
            TiledMapObjectLayer _mapEntityObjectLayer = MapCurrent.GetLayer<TiledMapObjectLayer>("Entity Layer");
            TiledMapObjectLayer _mapCollisionLayer = MapCurrent.GetLayer<TiledMapObjectLayer>("Collision Layer");
            TiledMapObjectLayer _mapAreaDefinitionLayer = MapCurrent.GetLayer<TiledMapObjectLayer>("Area Definitions");

            List<MapEntityArea> _areas = new List<MapEntityArea>();
            List<MapEntityStatic> _collisionObjects = new List<MapEntityStatic>();
            List<MapEntity> _entities = new List<MapEntity>();

            foreach (TiledMapObject _entityObject in _mapEntityObjectLayer.Objects)
            {
                switch (_entityObject.Name)
                {
                    case "mapPlayerSpawn":
                        {
                            // Run Player Spawn Code Here
                            Vector2 _playerSpawnPosition = new Vector2(Convert.ToInt32(_entityObject.Position.X), Convert.ToInt32(_entityObject.Position.Y));
                            store.SetPlayerDefaultSpawn(_playerSpawnPosition);
                            break;
                        }
                    case "mapTransition":
                        {
                            // Create Transition Objects
                            Vector2 _destinationPosition;
                            float _destinationX, _destinationY;

                            _destinationX = Convert.ToInt32(_entityObject.Properties["mapDestinationX"]);
                            _destinationY = Convert.ToInt32(_entityObject.Properties["mapDestinationY"]);
                            _destinationPosition = new Vector2(_destinationX, _destinationY);

                            int _width, _height;
                            Vector2 _position;

                            _width = Convert.ToInt32(_entityObject.Size.Width);
                            _height = Convert.ToInt32(_entityObject.Size.Height);
                            _position = _entityObject.Position;

                            MapEntityTransition mapTransition = new MapEntityTransition(Content, _entityObject.Properties["DestinationMap"], _destinationPosition,
                                _entityObject.Properties["Destination Area"], _entityObject.Properties["Destination Facing"], _position, _width, _height);

                            _collisionObjects.Add(mapTransition);
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

                            _entities.Add(_mapEntity);
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
                            MapEntityStatic solid = new MapEntityStatic(true, _collisionObject.Position, (int)_collisionObject.Size.Width, (int)_collisionObject.Size.Height);
                            _collisionObjects.Add(solid);
                            break;
                        }
                }
            }
            foreach (TiledMapObject _mapEntityArea in _mapAreaDefinitionLayer.Objects)
            {
                MapEntityArea area = new MapEntityArea(_mapEntityArea.Name, new Vector2((int)_mapEntityArea.Position.X, (int)_mapEntityArea.Position.Y),
                    new Rectangle((int)_mapEntityArea.Position.X, (int)_mapEntityArea.Position.Y, (int)_mapEntityArea.Size.Width, (int)_mapEntityArea.Size.Height));

                _areas.Add(area);
            }

            store.CloneAssetLists(_areas, _collisionObjects, _entities);
        }

        public static void SetAreaBoundaries(string areaName)
        {
            MapEntityArea _currentArea = null;

            _currentArea = Store.GetAreaByName(areaName);

            if(_currentArea != null)
            {
                Store.SetActiveAreaName(areaName);

                if (ActiveArea != null)
                {
                    ActiveArea = _currentArea.BoundingBox;
                }
                else
                {
                    ActiveArea = new Rectangle((int)_currentArea.Position.X, (int)_currentArea.Position.Y, (int)_currentArea.BoundingBox.Width, (int)_currentArea.BoundingBox.Height);
                }
            }
            else
            {
                return;
            }
        }
        public static void IntializeAreaTransition()
        {


            State = MapHandlingState.AreaTransition;
        }
    }
}
