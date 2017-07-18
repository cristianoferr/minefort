using Rimworld.model.entities;
using Rimworld.model.entities.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Rimworld.logic.MapGen
{
   public class MapGenerator
    {
        private World world;

        public MapGenerator(World world)
        {
            this.world = world;
        }

        public void RegenerateMap(float seed,Biome biome)
        {
            float height = biome.maxHeight;
            for (int x = 0; x < world.width; x++)
            {
                for (int y = 0; y < world.height; y++)
                {
                    float noise= Mathf.PerlinNoise((seed+x)/ biome.scale, (seed+ y) /biome.scale) ;
                    if (noise < 0) noise = 0;
                    if (noise > 1) noise = 1;
                    Tile tile = world.GetTileAt(x, y);
                    tile.height = biome.minHeight+noise*height;
                    tile.Type = biome.GetTileForHeight(noise);
                }
            }
            
        }
    }
}
