using MineFort.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MineFort.model.entities.map
{
    //specific data about the TileType for this biome
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
            TileType = new List<TileBiome>();
            this.biomes = biomes;
            maxHeight = 1;
            scale = 100;
        }
        IList<TileBiome> TileType;
        public Biomes biomes;
        internal float scale;
        internal string name;

        public int maxHeight { get; internal set; }


        public string GetName()
        {
            return name; 
        }

        public override string ToString()
        {
            return "Biome:"+name + "["+maxHeight+","+scale+"] with " + TileType.Count + " tiles. ";
        }

        internal TileType RandomTile(string tag=null)
        {
            if (tag == null)
            {
                return biomes.GetTileTypeWithName(TileType[Utils.Random(0, TileType.Count)].name);
            }
            return GetTileWithTag(tag);
        }

        internal TileType GetTileForHeight(float height)
        {

            TileBiome tb = TileType.Where(x => x.minHeight <= height && x.maxHeight >= height).OrderBy(x => Utils.Random(0, 100)).FirstOrDefault();
            if (tb == null)
            {
                Utils.LogError("No tileBiome found for height: "+height);
                return null;
            }
            return biomes.GetTileTypeWithTag(tb.tags);
        }

        internal void LoadFromCSV(string[] lineData)
        {

            TileBiome tb = new TileBiome();
            int i = 0;//0=name
            tb.name = lineData[i++];
            tb.minHeight = float.Parse(lineData[i++]);
            tb.maxHeight = float.Parse(lineData[i++]);
            tb.tags = lineData[i++];
           
            TileType.Add(tb);
        }

        //TODO: rever esse metodo
        public TileType GetTileWithTag(string tag)
        {
            //return biomes.GetTileTypeWithTag(TileType[Utils.Random(0, TileType.Count)].tags);
            return biomes.GetTileTypeWithTag(tag);
        }
    }
}
