using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLondon
{
    public class MapManagerStore
    {
        List<MapObject> MapObjects { get; set; }
        List<MapEntityArea> Areas { get; set; }
        List<MapEntity> Entities { get; set; }
        public List<MapCollisionSolidStatic> CollisionObjects { get; protected set; }

        
    }
}
