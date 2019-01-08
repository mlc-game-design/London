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
        Rectangle SceneBoundaries { get; set; }
        Rectangle ActiveArea { get; set; }
        string ActiveAreaName { get; set; }

        List<TiledMap> AdjacentMaps;
        List<MapObject> MapObjects { get; set; }
        List<MapAreaDefinition> Areas { get; set; }
        List<MapEntity> Entities { get; set; }
        public List<MapObjectSolid> SolidObjects { get; protected set; }

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

        Texture2D DebugTexture { get; set; }

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
            ActiveAreaName = "";

            AdjacentMaps = new List<TiledMap>();
            MapObjects = new List<MapObject>();
            SolidObjects = new List<MapObjectSolid>();
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
            MainPlayer = null;

            DebugTexture = content.Load<Texture2D>("SinglePixel");
        }

        /// <summary>
        /// Loads TiledMap into memory by MapName string
        /// </summary>
        /// <param name="mapName"></param>
        public void LoadMap(string mapName)
        {
            MapCurrent = contentManager.Load<TiledMap>(mapName);
            LoadMapObjectLayers();

            if (MapCurrent.Properties.ContainsKey("startZone") == true)
            {
                ActiveAreaName = MapCurrent.Properties["startZone"];
                SetAreaBoundaries(MapCurrent.Properties["startZone"]);
            }

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

                MediaPlayer.Volume = 0.4f;
                MediaPlayer.Play(MapBackgroundMusic);
            }

            MapBackgroundMusicAssetName = MapCurrent.Properties["mapBackgroundMusic"];
        }
        /// <summary>
        /// Loads MapObjects for the Active Map
        /// </summary>
        private void LoadMapObjectLayers()
        {
            TiledMapObjectLayer mapEntityObjectLayer = (TiledMapObjectLayer)MapCurrent.GetLayer("Entity Layer");
            TiledMapObjectLayer mapCollisionLayer = (TiledMapObjectLayer)MapCurrent.GetLayer("Collision Layer");
            TiledMapObjectLayer mapAreaBoundaryLayer = (TiledMapObjectLayer)MapCurrent.GetLayer("Area Definitions");

            foreach (TiledMapObject mapObject in mapEntityObjectLayer.Objects)
            {
                switch (mapObject.Name)
                {
                    case "mapPlayerSpawn":
                        {
                            if(MainPlayer == null)
                            {
                                // Run Player Spawn Code Here
                                PlayerSpawnX = Convert.ToInt32(mapObject.Position.X);
                                PlayerSpawnY = Convert.ToInt32(mapObject.Position.Y);
                                IsPlayerSpawn = true;

                                MainPlayer = new PlayerActor(contentManager, new Vector2(PlayerSpawnX, PlayerSpawnY), 100);
                            }
                            
                            break;
                        }
                    case "mapTransition":
                        {
                            // Create Transition Objects
                            float destinationX, destinationY;
                            
                            destinationX = Convert.ToInt32(mapObject.Properties["mapDestinationX"]);
                            destinationY = Convert.ToInt32(mapObject.Properties["mapDestinationY"]);

                            MapTransitionHandler mapTransition = new MapTransitionHandler(contentManager, mapObject.Properties["mapDestination"],
                                new Vector2((float)destinationX, (float)destinationY), new Rectangle((int)mapObject.Position.X, (int)mapObject.Position.Y, (int)mapObject.Size.Width, (int)mapObject.Size.Height),
                                mapObject.Properties["mapDestinationArea"], mapObject.Properties["mapDestinationFacing"]);

                            mapTransition.SetType("mapTransition");

                            MapObjects.Add(mapTransition);
                            break;
                        }
                    case "mapEntitySpawn":
                        {
                            // Spawn the Entity
                            bool isSolid = false;

                            if(mapObject.Properties["isSolid"] == "true")
                            {
                                isSolid = true;
                            }

                            MapEntityInteractive mapEntity = new MapEntityInteractive(isSolid, new Vector2(mapObject.Position.X, mapObject.Position.Y), (int)mapObject.Size.Width, (int)mapObject.Size.Height);
                            mapEntity.CreateAnimationDictionary(AssetManager.AnimationLibraries[mapObject.Properties["animationSetName"]], mapObject.Properties["currentAnimation"]);

                            Entities.Add(mapEntity);
                            break;
                        }
                }
            }
            foreach (TiledMapObject solidObject in mapCollisionLayer.Objects)
            {
                switch (solidObject.Name)
                {
                    case "solidStatic":
                        {
                            MapObjectSolid solid = new MapObjectSolid(solidObject.Position, (int)solidObject.Size.Width, (int)solidObject.Size.Height);
                            solid.SetType("mapObjectSolid");
                            SolidObjects.Add(solid);
                            break;
                        }
                }
            }
            foreach (TiledMapObject areaBoundary in mapAreaBoundaryLayer.Objects)
            {
                MapAreaDefinition area = new MapAreaDefinition(areaBoundary.Name, new Vector2((int)areaBoundary.Position.X, (int)areaBoundary.Position.Y),
                    new Rectangle((int)areaBoundary.Position.X, (int)areaBoundary.Position.Y, (int)areaBoundary.Size.Width, (int)areaBoundary.Size.Height));

                Areas.Add(area);
            }
        }
        private void SetAreaBoundaries(string mapAreaName)
        {
            MapAreaDefinition currentArea = null;

            foreach(MapAreaDefinition area in Areas)
            {
                if(area.Name == mapAreaName)
                {
                    currentArea = area;
                    ActiveAreaName = area.Name;
                }
            }

            if(ActiveArea != null)
            {
                ActiveArea = currentArea.BoundingBox;
            }
            else
            {
                ActiveArea = new Rectangle((int)currentArea.BoundingBox.X, (int)currentArea.BoundingBox.Y, (int)currentArea.BoundingBox.Width, (int)currentArea.BoundingBox.Height);
            }
        }
        private void UpdateCameraPosition(Rectangle sceneBoundary)
        {
            Vector2 _cameraMovePosition = new Vector2(0);

            if(MapCamera.BoundingRectangle.Bottom > sceneBoundary.Bottom)
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
        private void AreaTransitionCameraUpdate(PlayerActor.AreaTransitionMoveState moveState, GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float cameraMoveSpeed = 320f;

            switch (moveState)
            {
                case PlayerActor.AreaTransitionMoveState.SlideDown:
                    {
                        if(MapCamera.BoundingRectangle.Bottom < ActiveArea.Bottom)
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
                        if(MapCamera.BoundingRectangle.Left > ActiveArea.Left)
                        {
                            MapCamera.Position = MapCamera.Position - new Vector2(cameraMoveSpeed * deltaTime, 0);
                        }
                        
                        break;
                    }
                case PlayerActor.AreaTransitionMoveState.SlideRight:
                    {
                        if(MapCamera.BoundingRectangle.Right < ActiveArea.Right)
                        {
                            MapCamera.Position = MapCamera.Position + new Vector2(cameraMoveSpeed * deltaTime, 0);
                        }
                        
                        break;
                    }
            }
        }
        private void UnloadMap()
        {
            MapObjects.Clear();
            SolidObjects.Clear();
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
        public void Update(GameTime gameTime)
        {
            MapRenderer.Update(MapCurrent, gameTime);

            if(IsTransitionActive == false && IsAreaTransitionActive == false)
            {
                MainPlayer.Update(gameTime);
                MapCamera.Position = MainPlayer.Position - new Vector2(MapCamera.BoundingRectangle.Width / 2, MapCamera.BoundingRectangle.Height / 2);
                UpdateCameraPosition(ActiveArea);

                foreach (MapAreaDefinition area in Areas)
                {
                    if (MainPlayer.BoundingBox.Intersects(area.BoundingBox) == true)
                    {
                        if (area.Name != ActiveAreaName)
                        {
                            PreviousArea = new Rectangle(ActiveArea.X, ActiveArea.Y, ActiveArea.Width, ActiveArea.Height);
                            ActiveAreaName = area.Name;
                            SetAreaBoundaries(area.Name);
                            AreaTransitionCollisionRectangle = Rectangle.Intersect(MainPlayer.BoundingBox, ActiveArea);
                            IsAreaTransitionActive = true;
                            AreaTransitionTimer = 0f;
                            return;
                        }
                    }
                }

                foreach (MapObjectSolid solid in SolidObjects)
                {
                    if (MainPlayer.BoundingBox.Intersects(solid.BoundingBox))
                    {
                        // Run Uncollide Code in Player
                        Rectangle collisionRectangle = Rectangle.Intersect(MainPlayer.SolidBoundingBox, solid.BoundingBox);

                        MainPlayer.Uncollide(collisionRectangle);
                    }
                }

                foreach (MapObject mapObject in MapObjects)
                {
                    if (mapObject.Type == "mapTransition")
                    {
                        MapTransitionHandler transitionObject = (MapTransitionHandler)mapObject;

                        if (transitionObject.BoundingBox.Intersects(MainPlayer.SolidBoundingBox))
                        {
                            MapTransition = transitionObject;
                            transitionObject.InitializeTransition(MapCurrent, (Rectangle)MapCamera.BoundingRectangle, 
                                TransitionSound, graphicsDevice);
                            IsTransitionActive = true;
                        }
                    }
                }

                foreach (MapEntity entity in Entities)
                {
                    entity.Update(gameTime);

                    if (MainPlayer.BoundingBox.Intersects(entity.BoundingBox) && entity.IsSolid == true)
                    {
                        // Run Uncollide Code in Player
                        Rectangle collisionRectangle = Rectangle.Intersect(MainPlayer.SolidBoundingBox, entity.BoundingBox);

                        MainPlayer.Uncollide(collisionRectangle);
                    }

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

                if (IsPlayerSpawn == true)
                {
                    MainPlayer.Draw(spriteBatch);
                    //spriteBatch.Draw(DebugTexture, MainPlayer.BoundingBox, Color.Red * 0.4f);
                    //spriteBatch.Draw(DebugTexture, MainPlayer.SolidBoundingBox, Color.DarkRed * 0.6f);
                }

                if (IsAreaTransitionActive == true)
                {
                    //spriteBatch.Draw(DebugTexture, AreaTransitionCollisionRectangle, Color.Yellow * 0.4f);
                }

                foreach (MapObjectSolid solid in SolidObjects)
                {
                    //spriteBatch.Draw(DebugTexture, solid.BoundingBox, Color.Red * 0.4f);
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
