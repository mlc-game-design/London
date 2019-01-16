using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

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

            foreach(MapEntity _entity in entities)
            {
                Entities.Add(_entity.Clone() as MapEntity);
            }
        }

        /* METHODS */
        public void PlayBGM()
        {

        }
        public void PlayBGM(float volume)
        {

        }

        public void SetPlayerSpawnPosition(Vector2 position)
        {
            PlayerStartPosition = position;
        }
        public void SetPlayerSpawnPosition(int x, int y)
        {
            PlayerStartPosition = new Vector2(x, y);
        }
    }
}
