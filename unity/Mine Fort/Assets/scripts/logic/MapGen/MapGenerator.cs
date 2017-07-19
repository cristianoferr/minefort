using MineFort.model.entities;
using MineFort.model.entities.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MineFort.logic.MapGen
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
            for (int x = 0; x < world.Width; x++)
            {
                for (int y = 0; y < world.Height; y++)
                {
                        float noise= Mathf.PerlinNoise((seed+x)/ biome.scale, (seed+ y) /biome.scale) ;
                    if (noise < 0) noise = 0;
                    if (noise > 1) noise = 1;
                    Tile tile = world.GetTileAt(x, y,0);
                    tile.height = biome.minHeight+noise*height;
                    tile.SetTileType(biome.GetTileForHeight(noise));
                }
            }
            
        }
    }
}
