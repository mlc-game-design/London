using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace ProjectLondon
{
    public class MapManagerStore
    {
        /* MEMBERS */
        private List<MapObject> MapObjects { get; set; }
        private List<MapEntityArea> Areas { get; set; }
        private List<MapEntity> Entities { get; set; }
        public List<MapEntityStatic> SolidObjects { get; protected set; }

        public Vector2 PlayerStartPosition { get; protected set; }
        private ContentManager Content { get; set; }
        private string BackgroundMusicAssetName { get; set; }
        public Song BackgroundMusic { get; private set; }

        public string ActiveAreaName { get; protected set; }

        /* CONSTRUCTOR */
        public MapManagerStore(List<MapEntityArea> areas, List<MapEntityStatic> solidObjects, List<MapEntity> entities)
        {
            foreach(MapEntityArea _area in areas)
            {
                Areas.Add(_area.Clone() as MapEntityArea);
            }

            foreach(MapEntityStatic _solidObject in solidObjects)
            {
                SolidObjects.Add(_solidObject.Clone() as MapEntityStatic);
            }

            //foreach(MapEntity _entity in entities)
            //{
            //    Entities.Add(_entity.Clone() as MapEntity);
            //}
        }

        public void Initialize(ContentManager content, string bgmAssetName)
        {
            Content = content;
            BackgroundMusicAssetName = bgmAssetName;

            PlayerStartPosition = new Vector2(0, 0);
        }
        public void Initialize(ContentManager content, string bgmAssetName, Vector2 playerStartPosition)
        {
            Content = content;
            BackgroundMusicAssetName = bgmAssetName;
            PlayerStartPosition = new Vector2(playerStartPosition.X, playerStartPosition.Y);
        }

        /* METHODS */
        public void Destroy()
        {
            // Empty out All Lists
            for(int i = MapObjects.Count - 1; i >= 0; i--)
            {
                MapObjects.RemoveAt(i);
            }
            for (int i = SolidObjects.Count - 1; i >= 0; i--)
            {
                SolidObjects.RemoveAt(i);
            }
            for (int i = Entities.Count - 1; i >= 0; i--)
            {
                Entities.RemoveAt(i);
            }
            for (int i = Areas.Count - 1; i >= 0; i--)
            {
                Areas.RemoveAt(i);
            }

            // Remove Instanced Information
            BackgroundMusic = null;
            PlayerStartPosition = new Vector2();
        }

        public Rectangle GetAreaByName(string name)
        {
            MapEntityArea _loadedArea = null;

            foreach (MapEntityArea _area in Areas)
            {
                if (_area.Name == name)
                {
                    _loadedArea = _area;
                    ActiveAreaName = _area.Name;
                }
            }

            return _loadedArea;
        }
    }
}
