using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rimworld.model.entities.map.tiles
{
    public class TileData:TagObject
    {

        public float MovementCost = 1;

        public string fileName="undef";
        public string name;

        public TileData(string name)
        {
            this.name = name;
        }

        internal void LoadFromCSV(string[] lineData)
        {
            TileData td = this;
            int i = 0;
            td.name = lineData[i++];
            td.fileName = lineData[i++];
            td.MovementCost = float.Parse(lineData[i++]);
            string[] tags = lineData[i++].Split(',');
            foreach (string tag in tags)
            {
                td.AddTag(tag);
            }
        }
    }
}
