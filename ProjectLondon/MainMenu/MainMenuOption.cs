using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectLondon
{
    public class MainMenuOption
    {
        public string Name { get; protected set; }
        public Vector2 Position { get; protected set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        
        public MainMenuOption(string name, Vector2 startPosition)
        {
            Name = name;
            Position = startPosition;
            Width = 0;
            Height = 0;
        }

        public void AdjustPosition(SpriteFont stringFont)
        {
            Vector2 _stringMeasure = stringFont.MeasureString(Name);
            int _adjustedX, _adjustedY;

            Width = (int)_stringMeasure.X;
            Height = (int)_stringMeasure.Y;

            _adjustedX = (int)Position.X - ((int)_stringMeasure.X / 2);
            _adjustedY = (int)Position.Y - ((int)_stringMeasure.Y / 2);

            Position = new Vector2(_adjustedX, _adjustedY);
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont stringFont)
        {
            spriteBatch.DrawString(stringFont, Name, Position, Color.White);
        }
    }
}
