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

        public void RegenerateMap(float seed, Biome biome)
        {
            float maxHeight = biome.maxHeight;
            for (int x = 0; x < world.Width; x++)
            {
                for (int y = 0; y < world.Height; y++)
                {
                    float noise = Mathf.PerlinNoise((seed + x) / biome.scale, (seed + y) / biome.scale);

                    if (noise < 0) noise = 0;
                    if (noise > 1) noise = 1;
                    int height = (int)(noise* maxHeight);
                    Tile tile = world.GetTileAt(x, y, height);
                    //tile.height = noise*height;
                    tile.height = 0;
                    TileType tileType = biome.GetTileForHeight(noise);
                    tile.SetTileType(tileType,false);
                    for (int i = 0; i < height; i++)
                    {
                        Tile belowTile = world.GetTileAt(x, y, i);
                        belowTile.SetTileType(biome.GetTileWithTag(tileType.BelowTileTag), false);
                        //belowTile.SetTileType(TileType.Empty, false);
                    }
                    for (int i = height + 1; i < maxHeight; i++)
                    {
                         world.GetTileAt(x, y, i).SetTileType(TileType.Empty, false);
                        Tile belowTile = world.GetTileAt(x, y, i);
                        //belowTile.SetTileType(biome.GetTileWithTag(tileType.BelowTileTag), false);
                    }
                }

            }
        }
    }
}
