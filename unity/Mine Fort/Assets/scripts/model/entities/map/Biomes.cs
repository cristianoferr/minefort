using Rimworld.model.entities.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rimworld.model.entities.map
{
   public class Biomes
    {
        Dictionary<string, Biome> biomes;
        private DataHolder dataHolder;

        public Biomes()
        {
            biomes = new Dictionary<string, Biome>();

        }

        public Biomes(DataHolder dataHolder)
        {
            this.dataHolder = dataHolder;
        }

        public void AddBiome(string name, Biome biome)
        {
            if (biomes.ContainsKey(name)) biomes.Remove(name);
            biomes.Add(name, biome);
        }

        public Biome GetBiome(string name)
        {
            if (!biomes.ContainsKey(name)) return null;
            return biomes[name];
        }
    }
}
