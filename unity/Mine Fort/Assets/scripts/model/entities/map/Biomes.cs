using Rimworld.model.entities.map;
using Rimworld.model.entities.map.tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rimworld.model.entities.map
{
   public class Biomes
    {
        Dictionary<string, Biome> biomeList;
        private DataHolder dataHolder;

        public Biomes()
        {
            biomeList = new Dictionary<string, Biome>();
            tileData = new List<TileData>();
        }

        public Biomes(DataHolder dataHolder)
        {
            this.dataHolder = dataHolder;
        }

        public IList<TileData> tileData { get; private set; }

        public void AddBiome(string name, Biome biome)
        {
            if (biomeList.ContainsKey(name)) biomeList.Remove(name);
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

        public TileData GetTileDataWithName(string name)
        {
            TileData td= tileData.Where(x => x.name==name).FirstOrDefault();
            if (td == null)
            {
                td = new TileData(name);
                tileData.Add(td);
            }
            return td;
        }

        public TileData GetTileDataWithTag(string tags)
        {
            return tileData.Where(x => x.ContainsTags(tags)).OrderBy(x=>Utils.Random(0,100)).FirstOrDefault();
        }

        internal void LoadTileDataFromCSV(string[] lineData)
        {
            string name = lineData[0];
            GetTileDataWithName(name).LoadFromCSV(lineData);
        }
    }
}
