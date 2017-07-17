
using Rimworld.model.entities.map;
using System;
using UnityEngine;

namespace Rimworld.model.entities
{
    public class Map
    {
        private Chunk[,] chunks { get; set; }

        public Map(World world)
        {
            this.world = world;
        }
        public Room GetOutsideRoom(Vector3 position)
        {
            return GetChunkAt(position).outsideRoom;
        }
        public Room GetOutsideRoom(float x, float y)
        {
            return GetChunkAt(x,y).outsideRoom;
        }

        public Chunk GetChunkAt(Vector3 position)
        {
            return GetChunkAt(position.x, position.y);
        }
        public Chunk GetChunkAt(float x, float y)
        {
            while (x < 0) { x = GameConsts.WORLD_WIDTH + x; }
            while (y < 0) { y = GameConsts.WORLD_HEIGHT + y; }
            while (x >= GameConsts.WORLD_WIDTH) { x = x-GameConsts.WORLD_WIDTH ; }
            while (y >= GameConsts.WORLD_HEIGHT) { y = y-GameConsts.WORLD_HEIGHT ; }
            float cx = (int)(x / GameConsts.CHUNK_SIZE);
            float cy = (int)(y / GameConsts.CHUNK_SIZE);
            
            return chunks[(int)cx, (int)cy];
        }


        int chunkWidth = 0;
        int chunkHeight = 0;
        internal void SetupWorld(int width, int height)
        {
            this.width = width;
            this.height = height;
            chunkWidth = width / GameConsts.CHUNK_SIZE;
            chunkHeight = height / GameConsts.CHUNK_SIZE;
            chunks = new Chunk[chunkWidth, chunkHeight];
            for (int i = 0; i < chunkWidth; i++)
            {
                for (int j = 0; j < chunkHeight; j++)
                {
                    chunks[i, j] = new Chunk(this, i, j,OnTileChanged);
                }
            }

        }

        #region CallBacks
        Action<Tile> cbTileChanged;
        private World world;

        public void RegisterTileChanged(Action<Tile> callbackfunc)
        {
            cbTileChanged += callbackfunc;
        }

        public void UnregisterTileChanged(Action<Tile> callbackfunc)
        {
            cbTileChanged -= callbackfunc;
        }

        // Gets called whenever ANY tile changes
        void OnTileChanged(Tile t)
        {
            if (cbTileChanged == null)
                return;

            cbTileChanged(t);

            world.InvalidateTileGraph();
        }
        #endregion CallBacks


        public Tile GetTileAt(float x, float y)
        {
            Chunk chunk = GetChunkAt(x, y);

            int px = (int)(x % GameConsts.CHUNK_SIZE);
            int py = (int)(y % GameConsts.CHUNK_SIZE);
            return chunk.GetTileAt(px, py);
        }
        public Tile GetTileAt(Vector3 position)
        {
            return GetTileAt(position.x, position.y);

        }

        public int height { get; private set; }

        public int width { get; private set; }
    }
}
