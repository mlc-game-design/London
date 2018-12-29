using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;

namespace ProjectLondon
{
    public class OverworldManager
    {
        ContentManager contentManager;
        GraphicsDevice graphicsDevice;

        TiledMap mapCurrent;
        TiledMapRenderer mapRenderer;
        Camera2D mapCamera;

        List<TiledMap> adjacentMaps;

        public OverworldManager(GraphicsDevice gfxDevice, ContentManager content)
        {
            graphicsDevice = gfxDevice;
            contentManager = content;

            mapCurrent = null;
            mapRenderer = new TiledMapRenderer(graphicsDevice);
            mapCamera = new Camera2D(graphicsDevice);
            mapCamera.Origin = new Vector2(0, 0);
            mapCamera.Position = new Vector2(0, 0);
            mapCamera.Zoom = 4.0f;

            adjacentMaps = new List<TiledMap>();
        }

        public void LoadMap(string mapName)
        {
            mapCurrent = contentManager.Load<TiledMap>(mapName);
        } 

        public void Update(GameTime gameTime)
        {
            mapRenderer.Update(mapCurrent, gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(transformMatrix: mapCamera.GetViewMatrix(), samplerState: SamplerState.PointClamp);

            mapRenderer.Draw(mapCurrent, mapCamera.GetViewMatrix());

            spriteBatch.End();
        }
    }
}
