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
        public bool IsActive { get; protected set; }

        TiledMap MapCurrent;
        TiledMapRenderer MapRenderer;
        public Camera2D MapCamera { get; private set; }
        Rectangle SceneBoundaries { get; set; }
        Rectangle ActiveArea { get; set; }
        string ActiveAreaName { get; set; }

        List<TiledMap> AdjacentMaps;
        List<MapObject> MapObjects { get; set; }
        List<MapAreaDefinition> Areas { get; set; }
        List<MapEntity> Entities { get; set; }
        public List<MapCollisionSolidStatic> CollisionObjects { get; protected set; }

        MapTransitionHandler MapTransition { get; set; }
        bool IsTransitionActive { get; set; }
        bool IsAreaTransitionActive { get; set; }
        float AreaTransitionTimer { get; set; }
        Rectangle AreaTransitionCollisionRectangle { get; set; }
        Rectangle PreviousArea { get; set; }
        SoundEffect TransitionSound { get; set; }

        Song MapBackgroundMusic;
        string MapBackgroundMusicAssetName;

        public bool IsPlayerSpawn { get; private set; }
        public int PlayerSpawnX, PlayerSpawnY;
        private PlayerActor MainPlayer { get; set; }

        public OverworldState State { get; protected set; }
        public enum OverworldState
        {
            Normal,
            AreaTransition,
            MapTransition
        }

        Texture2D DebugTexture { get; set; }

        public OverworldManager(GraphicsDevice gfxDevice, ContentManager content, PlayerActor mainPlayer)
        {
            graphicsDevice = gfxDevice;
            contentManager = content;
            MainPlayer = mainPlayer;
            IsActive = false;

            MapCurrent = null;
            MapRenderer = new TiledMapRenderer(graphicsDevice);
            MapCamera = new Camera2D(graphicsDevice);
            MapCamera.Origin = new Vector2(0, 0);
            MapCamera.Position = new Vector2(0, 0);
            MapCamera.Zoom = 4.0f;
            ActiveAreaName = "";

            AdjacentMaps = new List<TiledMap>();
            MapObjects = new List<MapObject>();
            CollisionObjects = new List<MapCollisionSolidStatic>();
            Entities = new List<MapEntity>();
            Areas = new List<MapAreaDefinition>();

            MapTransition = null;
            IsTransitionActive = false;
            IsAreaTransitionActive = false;
            AreaTransitionTimer = 0f;
            TransitionSound = contentManager.Load<SoundEffect>("sfx/Oracle_Stairs");

            MapBackgroundMusic = null;
            MapBackgroundMusicAssetName = "";

            IsPlayerSpawn = false;

            DebugTexture = content.Load<Texture2D>("SinglePixel");
        }

        /// <summary>
        /// Loads TiledMap into memory by mapName string, Analyzes TiledMap object layers, instantiates 
        /// all objects into memory, sets Background Music
        /// </summary>
        /// <param name="mapName">Asset Name from ContentPipeline</param>
        public void LoadMap(string mapName)
        {
            MapCurrent = contentManager.Load<TiledMap>(mapName);
            LoadMapObjectLayers();

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
        }
        /// <summary>
        /// Loads MapObjects for the Active Map, iterates through them, and instantiates objects
        /// based on the information found in the Map ObjectLayers
        /// </summary>
        private void LoadMapObjectLayers()
        {
            TiledMapObjectLayer _mapEntityObjectLayer = (TiledMapObjectLayer)MapCurrent.GetLayer("Entity Layer");
            TiledMapObjectLayer _mapCollisionLayer = (TiledMapObjectLayer)MapCurrent.GetLayer("Collision Layer");
            TiledMapObjectLayer _mapAreaDefinitionLayer = (TiledMapObjectLayer)MapCurrent.GetLayer("Area Definitions");

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
                            // Spawn the Entity
                            bool isSolid = false;

                            if(_entityObject.Properties["isSolid"] == "true")
                            {
                                isSolid = true;
                            }

                            MapEntityStatic mapEntity = new MapEntityStatic(isSolid, new Vector2(_entityObject.Position.X, _entityObject.Position.Y), (int)_entityObject.Size.Width, (int)_entityObject.Size.Height);
                            mapEntity.CreateAnimationDictionary(AssetManager.AnimationLibraries[_entityObject.Properties["animationSetName"]], _entityObject.Properties["currentAnimation"]);

                            Entities.Add(mapEntity);
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
            foreach (TiledMapObject _areaDefintion in _mapAreaDefinitionLayer.Objects)
            {
                MapAreaDefinition area = new MapAreaDefinition(_areaDefintion.Name, new Vector2((int)_areaDefintion.Position.X, (int)_areaDefintion.Position.Y),
                    new Rectangle((int)_areaDefintion.Position.X, (int)_areaDefintion.Position.Y, (int)_areaDefintion.Size.Width, (int)_areaDefintion.Size.Height));

                Areas.Add(area);
            }
        }
        /// <summary>
        /// Configures the Area's Defined Total space, as well as MapCamera operating restriction space
        /// </summary>
        /// <param name="mapAreaName">Reference name for Area being used to set the boundaries</param>
        private void SetAreaBoundaries(string mapAreaName)
        {
            MapAreaDefinition _currentArea = null;

            foreach(MapAreaDefinition _area in Areas)
            {
                if(_area.Name == mapAreaName)
                {
                    _currentArea = _area;
                    ActiveAreaName = _area.Name;
                }
            }

            if(ActiveArea != null)
            {
                ActiveArea = _currentArea.BoundingBox;
            }
            else
            {
                ActiveArea = new Rectangle((int)_currentArea.BoundingBox.X, (int)_currentArea.BoundingBox.Y, (int)_currentArea.BoundingBox.Width, (int)_currentArea.BoundingBox.Height);
            }
        }
        /// <summary>
        /// Pulls camera back inside the defined boundary if it strays outside the space
        /// </summary>
        /// <param name="sceneBoundary">The Rectangle in which the camera is allowed to operate</param>
        private void UpdateCameraPosition(Rectangle sceneBoundary)
        {
            Vector2 _cameraMovePosition = new Vector2(0);

            if (MapCamera.BoundingRectangle.Bottom > sceneBoundary.Bottom)
            {
                float _diff = MapCamera.BoundingRectangle.Bottom - sceneBoundary.Bottom;
                _cameraMovePosition = _cameraMovePosition + new Vector2(0, -_diff);
            }

            if (MapCamera.BoundingRectangle.Top < sceneBoundary.Top)
            {
                float _diff = MapCamera.BoundingRectangle.Top - sceneBoundary.Top;
                _cameraMovePosition = _cameraMovePosition + new Vector2(0, -_diff);
            }

            if (MapCamera.BoundingRectangle.Right > sceneBoundary.Right)
            {
                float _diff = MapCamera.BoundingRectangle.Right - sceneBoundary.Right;
                _cameraMovePosition = _cameraMovePosition + new Vector2(-_diff, 0);

            }

            if (MapCamera.BoundingRectangle.Left < sceneBoundary.Left)
            {
                float _diff = MapCamera.BoundingRectangle.Left - sceneBoundary.Left;
                _cameraMovePosition = _cameraMovePosition + new Vector2(-_diff, 0);
            }

            MapCamera.Position = MapCamera.Position + _cameraMovePosition;
        }
        /// <summary>
        /// Slides the Camera between two Areas during transitions
        /// </summary>
        /// <param name="moveState">The MoveState of the MainPlayer, used to determine which way the camera should move</param>
        /// <param name="gameTime">Elapsed Gametime used to move the camera at a steady pace</param>
        private void AreaTransitionCameraUpdate(PlayerActor.AreaTransitionMoveState moveState, GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float cameraMoveSpeed = 320f;

            switch (moveState)
            {
                case PlayerActor.AreaTransitionMoveState.SlideDown:
                    {
                        if (MapCamera.BoundingRectangle.Bottom < ActiveArea.Bottom)
                        {
                            MapCamera.Position = MapCamera.Position + new Vector2(0, cameraMoveSpeed * deltaTime);
                        }

                        break;
                    }
                case PlayerActor.AreaTransitionMoveState.SlideUp:
                    {
                        if (MapCamera.BoundingRectangle.Top > ActiveArea.Top)
                        {
                            MapCamera.Position = MapCamera.Position - new Vector2(0, cameraMoveSpeed * deltaTime);
                        }

                        break;
                    }
                case PlayerActor.AreaTransitionMoveState.SlideLeft:
                    {
                        if (MapCamera.BoundingRectangle.Left > ActiveArea.Left)
                        {
                            MapCamera.Position = MapCamera.Position - new Vector2(cameraMoveSpeed * deltaTime, 0);
                        }

                        break;
                    }
                case PlayerActor.AreaTransitionMoveState.SlideRight:
                    {
                        if (MapCamera.BoundingRectangle.Right < ActiveArea.Right)
                        {
                            MapCamera.Position = MapCamera.Position + new Vector2(cameraMoveSpeed * deltaTime, 0);
                        }

                        break;
                    }
            }
        }
        /// <summary>
        /// Clears the Current map's Lists from memory, Loads New TiledMap into memory,
        /// positions and faces the Player accordingly, snaps the MapCamera into place,
        /// then signals the MapTransition object to continue it's transition
        /// </summary>
        private void UnloadMap()
        {
            MapObjects.Clear();
            CollisionObjects.Clear();
            Areas.Clear();
            AdjacentMaps.Clear();

            LoadMap("maps/" + MapTransition.DestinationMapName);

            MainPlayer.SetPosition(MapTransition.DestinationPosition);
            MainPlayer.SetFacing(MapTransition.DestinationFacing);

            SetAreaBoundaries(MapTransition.DestinationAreaName);
            MapCamera.Position = MainPlayer.Position - new Vector2(MapCamera.BoundingRectangle.Width / 2, MapCamera.BoundingRectangle.Height / 2);
            UpdateCameraPosition(ActiveArea);

            MapTransition.MapChangeComplete((Rectangle)MapCamera.BoundingRectangle);
        }
        /// <summary>
        /// Iterates through the current Map's lists and checks their BoundingBox's against
        /// the MainPlayer's to determine if a Collision has occurred. Begins or handles
        /// collision processes as they occur.
        /// </summary>
        public void HandleCollisions()
        {
            foreach (MapAreaDefinition _area in Areas)
            {
                if (MainPlayer.BoundingBox.Intersects(_area.BoundingBox) == true)
                {
                    if (_area.Name != ActiveAreaName)
                    {
                        PreviousArea = new Rectangle(ActiveArea.X, ActiveArea.Y, ActiveArea.Width, ActiveArea.Height);
                        ActiveAreaName = _area.Name;
                        SetAreaBoundaries(_area.Name);
                        AreaTransitionCollisionRectangle = Rectangle.Intersect(MainPlayer.BoundingBox, ActiveArea);
                        IsAreaTransitionActive = true;
                        AreaTransitionTimer = 0f;
                        return;
                    }
                }
            }
            foreach (MapCollisionSolidStatic _collisionObject in CollisionObjects)
            {
                if (MainPlayer.SolidBoundingBox.Intersects(_collisionObject.BoundingBox))
                {
                    switch (_collisionObject.Type)
                    {
                        case "MapCollisionStatic":
                            {
                                Rectangle collisionRectangle = Rectangle.Intersect(MainPlayer.SolidBoundingBox, _collisionObject.BoundingBox);

                                MainPlayer.HasCollided(collisionRectangle);

                                break;
                            }
                        case "MapCollisionWater":
                            {

                                break;
                            }
                        case "MapCollisionPitTrap":
                            {

                                break;
                            }
                    }
                }
            }
            foreach (MapObject _mapTransition in MapObjects)
            {
                if (_mapTransition.Type == "mapTransition")
                {
                    MapTransitionHandler transitionObject = (MapTransitionHandler)_mapTransition;

                    if (transitionObject.BoundingBox.Intersects(MainPlayer.SolidBoundingBox))
                    {
                        MapTransition = transitionObject;
                        transitionObject.InitializeTransition(MapCurrent, (Rectangle)MapCamera.BoundingRectangle,
                            TransitionSound);
                        IsTransitionActive = true;
                    }
                }
            }
            foreach (MapEntity _mapEntity in Entities)
            {
                if (MainPlayer.BoundingBox.Intersects(_mapEntity.BoundingBox) && _mapEntity.IsSolid == true)
                {
                    // Run Uncollide Code in Player
                    Rectangle collisionRectangle = Rectangle.Intersect(MainPlayer.SolidBoundingBox, _mapEntity.BoundingBox);

                    MainPlayer.Uncollide(collisionRectangle);
                }

            }
        }
        public void Activate()
        {
            IsActive = true;
        }
        public void Deactivate()
        {
            IsActive = false;
        }
        public void Update(GameTime gameTime)
        {
            MapRenderer.Update(MapCurrent, gameTime);

            if (IsTransitionActive == false && IsAreaTransitionActive == false)
            {
                MainPlayer.Update(gameTime);
                MapCamera.Position = MainPlayer.Position - new Vector2(MapCamera.BoundingRectangle.Width / 2, MapCamera.BoundingRectangle.Height / 2);
                UpdateCameraPosition(ActiveArea);
                HandleCollisions();

                foreach (MapEntity _mapEntity in Entities)
                {
                    _mapEntity.Update(gameTime);
                }
                
            }
            else if(IsAreaTransitionActive == true)
            {
                float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

                AreaTransitionTimer += deltaTime;

                if(AreaTransitionTimer > 0.15f)
                {
                    // Move Camera and Player into New Area
                    if (MainPlayer.TransitionMoveState == PlayerActor.AreaTransitionMoveState.Complete)
                    {
                        IsAreaTransitionActive = false;
                        AreaTransitionTimer = 0f;
                        MainPlayer.ResetAreaTransitionState();
                    }
                    else
                    {
                        MainPlayer.MoveToNewArea(AreaTransitionCollisionRectangle, ActiveArea, gameTime);
                        AreaTransitionCameraUpdate(MainPlayer.TransitionMoveState, gameTime);
                    }
                }
            }
            else
            {
                if(MapTransition.State == MapTransitionHandler.TransitionState.MapChange)
                {
                    UnloadMap();
                }
                else if(MapTransition.State == MapTransitionHandler.TransitionState.Complete) {
                    IsTransitionActive = false;
                    MapTransition = null;
                }
                else
                {
                    MapTransition.Update(gameTime);
                }
            }
            
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if(MapTransition != null && MapTransition.State == MapTransitionHandler.TransitionState.MapChange)
            {
                MapTransition.Draw(spriteBatch);
            }
            else
            {
                MapRenderer.Draw(MapCurrent, MapCamera.GetViewMatrix());

                if (MainPlayer.IsActive == true)
                {
                    MainPlayer.Draw(spriteBatch);
                }

                foreach (MapEntity entity in Entities)
                {
                    entity.Draw(spriteBatch);
                }

                if (IsTransitionActive == true)
                {
                    MapTransition.Draw(spriteBatch);
                }
            }
            

            
        }
    }
}
