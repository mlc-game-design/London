using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectLondon
{
    public class MapStoreHandler
    {
        private MapManagerStore Store { get; set; }

        private MapEntityTransition ActiveMapTransition { get; set; }

        public MapStoreHandler()
        {
            Store = null;
        }

        public void SetStore(MapManagerStore store)
        {
            Store = store;
        }

        public void HandleCollisions(PlayerActor player)
        {
            foreach (MapEntityArea _area in Store.Areas)
            {
                if (player.BoundingBox.Intersects(_area.BoundingBox) == true)
                {
                    if (_area.Name != Store.ActiveAreaName)
                    {
                        MapManager.SetAreaBoundaries(_area.Name);
                        MapManager.IntializeAreaTransition();
                        AreaTransitionCollisionRectangle = Rectangle.Intersect(MainPlayer.BoundingBox, ActiveArea);
                        AreaTransitionTimer = 0f;
                        return;
                    }
                }
            }
            foreach (MapEntityStatic _collisionObject in Store.CollisionObjects)
            {
                if (player.SolidBoundingBox.Intersects(_collisionObject.BoundingBox))
                {
                    switch (_collisionObject.Type)
                    {
                        case "MapCollisionStatic":
                            {
                                Rectangle collisionRectangle = Rectangle.Intersect(player.SolidBoundingBox, _collisionObject.BoundingBox);

                                player.HasCollided(collisionRectangle);

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
            foreach (MapEntityTransition _mapTransition in Store.CollisionObjects)
            {
                if (_mapTransition.Type == "mapTransition")
                {
                    MapEntityTransition transitionObject = _mapTransition;

                    if (transitionObject.BoundingBox.Intersects(player.SolidBoundingBox))
                    {
                        ActiveMapTransition = transitionObject;
                        transitionObject.InitializeTransition(MapCurrent, (Rectangle)MapCamera.BoundingRectangle,
                            TransitionSound);
                        IsTransitionActive = true;
                    }
                }
            }
            foreach (MapEntity _mapEntity in Store.Entities)
            {
                if (player.BoundingBox.Intersects(_mapEntity.BoundingBox) && _mapEntity.IsSolid == true)
                {
                    // Run Uncollide Code in Player
                    Rectangle collisionRectangle = Rectangle.Intersect(player.BoundingBox, _mapEntity.BoundingBox);

                    player.HasCollided(collisionRectangle);
                }
            }
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
