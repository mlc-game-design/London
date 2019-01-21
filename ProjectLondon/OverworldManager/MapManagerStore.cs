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
        public List<MapObject> MapObjects { get; protected set; }
        public List<MapEntityArea> Areas { get; protected set; }
        public List<MapEntity> Entities { get; protected set; }
        public List<MapEntityStatic> CollisionObjects { get; protected set; }

        public Vector2 PlayerStartPosition { get; protected set; }
        private ContentManager Content { get; set; }
        public Song BackgroundMusic { get; private set; }

        public string ActiveAreaName { get; protected set; }

        /* CONSTRUCTOR */
        public MapManagerStore(ContentManager content)
        {
            MapObjects = new List<MapObject>();
            Areas = new List<MapEntityArea>();
            Entities = new List<MapEntity>();
            CollisionObjects = new List<MapEntityStatic>();

            PlayerStartPosition = new Vector2();
        }

        public void CloneAssetLists(List<MapEntityArea> areas, List<MapEntityStatic> collisionObjects, List<MapEntity> entities)
        {
            foreach (MapEntityArea _area in areas)
            {
                Areas.Add(_area.Clone() as MapEntityArea);
            }

            foreach (MapEntityStatic _solidObject in collisionObjects)
            {
                CollisionObjects.Add(_solidObject.Clone() as MapEntityStatic);
            }

            //foreach(MapEntity _entity in entities)
            //{
            //    Entities.Add(_entity.Clone() as MapEntity);
            //}
        }
        public void SetPlayerDefaultSpawn(Vector2 playerStartPosition)
        {
            PlayerStartPosition = new Vector2(playerStartPosition.X, playerStartPosition.Y);
        }
        public void SetActiveAreaName(string areaName)
        {
            ActiveAreaName = areaName;
        }

        /* METHODS */
        public void Destroy()
        {
            // Empty out All Lists
            for(int i = MapObjects.Count - 1; i >= 0; i--)
            {
                MapObjects.RemoveAt(i);
            }
            for (int i = CollisionObjects.Count - 1; i >= 0; i--)
            {
                CollisionObjects.RemoveAt(i);
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

        public MapEntityArea GetAreaByName(string name)
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

        public void PlayBGM(Song song)
        {
            BackgroundMusic = song;
            
            MediaPlayer.Volume = 1.0f;
            MediaPlayer.Play(BackgroundMusic);
        }
        public void PlayBGM(Song song, float volume)
        {
            BackgroundMusic = song;

            MediaPlayer.Volume = volume;
            MediaPlayer.Play(BackgroundMusic);
        }
        public void SetBGM(Song song)
        {
            BackgroundMusic = song;
        }
    }
}
