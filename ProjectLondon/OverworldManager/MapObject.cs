using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLondon
{
    public abstract class MapObject
    {
        public string Type { get; protected set; }

        public void SetType(string type)
        {
            Type = type;
        }
    }
}
