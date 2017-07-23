
using MineFort.Rooms;
using MineFort.model.entities.map;
using System;
using UnityEngine;

namespace MineFort.model.entities
{
    public class Map
    {
        private Chunk[,] chunks { get; set; }
        public AutomatoCelular automato;

        public Map(World world)
        {
            this.world = world;
            automato = new AutomatoCelular(this);
        }
        public Biome biome
        {
            get
            {
                return world.biome;
            }
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
        internal void SetupWorld(int width, int height,int depth)
        {
            GameConsts.CHUNK_SIZE = (width / GameConsts.CHUNK_QTD);
            //This way I get the closest int value for width/height.
            width = (width / GameConsts.CHUNK_SIZE) * GameConsts.CHUNK_SIZE;
            height = (height / GameConsts.CHUNK_SIZE) * GameConsts.CHUNK_SIZE;

            this.width = width;
            this.height = height;
            this.depth = depth;
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


        public Tile GetTileAt(float x, float y, float z =0)
        {
            if (x < 0||x>=width||y<0||y>=height||z<0||z>=depth) return null;
            Chunk chunk = GetChunkAt(x, y);

            int px = (int)(x % GameConsts.CHUNK_SIZE);
            int py = (int)(y % GameConsts.CHUNK_SIZE);
            return chunk.GetTileAt((int)px, (int)py,(int)z);
        }
        public Tile GetTileAt(Vector3 position)
        {
            return GetTileAt(position.x, position.y, position.z);

        }

        public int height { get; private set; }

        public int width { get; private set; }
        public int depth { get; set; }
    }
}
