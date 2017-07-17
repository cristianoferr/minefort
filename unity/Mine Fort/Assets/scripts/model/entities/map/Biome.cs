using Rimworld.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rimworld.model.entities.map.tiles;

namespace Rimworld.model.entities.map
{
    public class Biome : TagObject
    {

        public Biome()
        {
            tileData = new List<TileData>();
        }
        public IList<TileData> tileData;

        internal TileData RandomTile(string tag=null)
        {
            if (tag == null)
            {
                return tileData[Utils.Random(0, tileData.Count)];
            }
            return tileData.Where(x => x.ContainsTags(tag)).OrderBy(x => Utils.Random(0, 100)).FirstOrDefault();
        }

        internal void LoadFromCSV(string[] lineData)
        {
            TileData td = new TileData();
            int i = 0;
            td.name = lineData[i++];
            td.fileName = lineData[i++];
            td.MovementCost= float.Parse(lineData[i++]);
            td.MinHeight = float.Parse(lineData[i++]);
            td.MaxHeight = float.Parse(lineData[i++]);
            string[] tags = lineData[i++].Split(',');
            foreach(string tag in tags)
            {
                td.AddTag(tag);
            }
            tileData.Add(td);
        }

        public TileData GetTileWithTag(string tag)
        {
            try
            {
                return tileData.Where(x => x.ContainsTags(tag)).OrderBy(x => Utils.Random(0, 100)).FirstOrDefault();
            }
            catch (Exception)
            {

                return null;
            }
            
        }
    }
}
