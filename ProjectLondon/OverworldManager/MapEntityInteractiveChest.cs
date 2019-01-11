using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace ProjectLondon
{
    public class MapEntityInteractiveChest : MapEntityInteractive
    {
        public Vector2 OriginPosition { get; protected set; }
        public string UniqueIdentifier { get; protected set; }
        public string ItemName { get; protected set; }
        public PlayerActorItem Item { get; protected set; }

        public MapEntityInteractiveChest(string uniqueIdentifier, Vector2 position, Rectangle collisionBox, string itemName)
        {
            SubType = "Chest";
            UniqueIdentifier = uniqueIdentifier;
            Position = position;
            OriginPosition = new Vector2(Position.X + 8, Position.Y + 8);
            BoundingBox = collisionBox;
            ItemName = itemName;
            Item = null;
        }

        public void Update(GameTime gameTime, PlayerActor mainPlayer)
        {
            if(IsActive == false)
            {
                return;
            }
            else
            {
                int distanceHorizontal, distanceVertical;

                distanceHorizontal = Math.Abs((int)OriginPosition.X - (int)mainPlayer.OriginPoint.X);
                distanceVertical = Math.Abs((int)OriginPosition.Y - (int)mainPlayer.OriginPoint.Y);

                if(distanceHorizontal <= 24 || distanceVertical <= 24)
                {
                    if (mainPlayer.ControlKeys["A_Key"] == true)
                    {
                        bool _isPlayerFacingMe = false;

                        if(distanceHorizontal > distanceVertical)
                        {
                            _isPlayerFacingMe = IsPlayerFacingMeFromAboveOrBelow(mainPlayer);
                        }
                        else
                        {
                            _isPlayerFacingMe = IsPlayerFacingMeFromLeftOrRight(mainPlayer);
                        }

                        if(_isPlayerFacingMe == true)
                        {
                            // Open the Damn Chest!

                        }
                    }
                }
            }
        }

        public bool IsPlayerFacingMeFromAboveOrBelow(PlayerActor mainPlayer)
        {
            bool _isPlayerFacingMe = false;

            if(mainPlayer.OriginPoint.Y > OriginPosition.Y)
            {
                // Player is Below us, needs to be facing up
                if (mainPlayer.Facing == PlayerActor.PlayerFacing.Up) _isPlayerFacingMe = true;
            }
            else
            {
                // Player is Above us, needs to be facing down
                if (mainPlayer.Facing == PlayerActor.PlayerFacing.Down) _isPlayerFacingMe = true;
            }

            return _isPlayerFacingMe;
        }
        public bool IsPlayerFacingMeFromLeftOrRight(PlayerActor mainPlayer)
        {
            bool _isPlayerFacingMe = false;

            if (mainPlayer.OriginPoint.X > OriginPosition.X)
            {
                // Player is to the Right, needs to be facing left
                if (mainPlayer.Facing == PlayerActor.PlayerFacing.Left) _isPlayerFacingMe = true;
            }
            else
            {
                // Player is to the Left, needs to be facing right
                if (mainPlayer.Facing == PlayerActor.PlayerFacing.Right) _isPlayerFacingMe = true;
            }

            return _isPlayerFacingMe;
        }
        public void OpenChest()
        {

        }
    }
}
