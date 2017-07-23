using MineFort.model.entities.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MineFort.model.entities.map
{
   public class Biomes
    {
        Dictionary<string, Biome> biomeList;
        private DataHolder dataHolder;

        public Biomes()
        {
            biomeList = new Dictionary<string, Biome>();
            TileType.Empty.AddTag(GameConsts.TAG_EMPTY);
            tileTypes = new List<TileType>();
        }

        public Biomes(DataHolder dataHolder)
        {
            this.dataHolder = dataHolder;
        }

        public IList<TileType> tileTypes { get; private set; }

        public void AddBiome(string name, Biome biome)
        {
            if (biomeList.ContainsKey(name)) biomeList.Remove(name);
            biome.name = name;
            biomeList.Add(name, biome);
        }

        public Biome GetBiome(string name)
        {
            if (!biomeList.ContainsKey(name)) {
                Biome biome = new Biome(this);
                biomeList.Add(name,biome);
            }
            return biomeList[name];
        }

        public Biome currBiome;
        internal void RandomBiome()
        {
            currBiome = biomeList.Values.OrderBy(x => Utils.Random(0, 100)).FirstOrDefault();
        }

        public TileType GetTileTypeWithName(string name)
        {
            TileType td= tileTypes.Where(x => x.name==name).FirstOrDefault();
            if (td == null)
            {
                td = new TileType();
                td.name = name;
                tileTypes.Add(td);
            }
            return td;
        }

        public TileType GetTileTypeWithTag(string tags)
        {
            return tileTypes.Where(x => x.ContainsTags(tags)).OrderBy(x=>Utils.Random(0,100)).FirstOrDefault();
        }

        internal void LoadTileTypeFromCSV(string[] lineData)
        {
            string name = lineData[0];
            GetTileTypeWithName(name).LoadFromCSV(lineData);
        }
    }
}
