using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rimworld.model.entities.map.tiles
{
    public class TileData:TagObject
    {

        public float MovementCost = 1;

        public float MinHeight = 0;
        public float MaxHeight = 0.2f;
        public string fileName="undef";
        public string name;
    }
}
