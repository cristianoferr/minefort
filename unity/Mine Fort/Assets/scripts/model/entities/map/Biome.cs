using Rimworld.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rimworld.model.entities.map.tiles;

namespace Rimworld.model.entities.map
{
    //specific data about the tiledata for this biome
    class TileBiome
    {
        public string name;
        public string tags;
        public float minHeight;
        public float maxHeight;
    }
    public class Biome : TagObject
    {

        public Biome(Biomes biomes)
        {
            tileData = new List<TileBiome>();
            this.biomes = biomes;
            maxHeight = 1;
            minHeight = 0;
            scale = 100;
        }
        IList<TileBiome> tileData;
        private Biomes biomes;
        internal float scale;

        public float maxHeight { get; internal set; }
        public float minHeight { get; internal set; }
        

        internal TileData RandomTile(string tag=null)
        {
            if (tag == null)
            {
                return biomes.GetTileDataWithName(tileData[Utils.Random(0, tileData.Count)].name);
            }
            return GetTileWithTag(tag);
        }

        internal TileData GetTileForHeight(float height)
        {

            TileBiome tb = tileData.Where(x => x.minHeight <= height && x.maxHeight >= height).OrderBy(x => Utils.Random(0, 100)).FirstOrDefault();
            if (tb == null)
            {
                Utils.LogError("No tileBiome found for height: "+height);
                return null;
            }
            return biomes.GetTileDataWithTag(tb.tags);
        }

        internal void LoadFromCSV(string[] lineData)
        {

            TileBiome tb = new TileBiome();
            int i = 0;//0=name
            tb.name = lineData[i++];
            tb.minHeight = float.Parse(lineData[i++]);
            tb.maxHeight = float.Parse(lineData[i++]);
            tb.tags = lineData[i++];
           
            tileData.Add(tb);
        }

        public TileData GetTileWithTag(string tag)
        {
            return biomes.GetTileDataWithTag(tileData[Utils.Random(0, tileData.Count)].tags);
        }
    }
}
