
using System;
using System.Collections.Generic;

namespace Rimworld.model.entities.map
{
    public class Chunk
    {
        public Tile[,] tiles { get; private set; }
        public IList<Room> rooms;
        public Chunk(Map map, int cx, int cy)

        {
            rooms = new List<Room>();
            this.map = map;
            this.cx = cx;
            this.cy = cy;
            outsideRoom = new Room(this);
            tiles = new Tile[GameConsts.CHUNK_SIZE, GameConsts.CHUNK_SIZE];
            for (int i = 0; i < GameConsts.CHUNK_SIZE; i++)
            {
                for (int j = 0; j < GameConsts.CHUNK_SIZE; j++)
                {
                    tiles[i, j] = new Tile(this, i, j);
                }
            }
        }

        public Room outsideRoom { get; set; }

        public Tile GetTileAt(int px, int py)
        {
            //a posição não fica nesse chunk...
            if (px < 0 || py < 0 || px >= GameConsts.CHUNK_SIZE || px >= GameConsts.CHUNK_SIZE)
            {
                return map.GetTileAt(cx * GameConsts.CHUNK_SIZE + px, cy * GameConsts.CHUNK_SIZE + py);
            }
            return tiles[px, py];
        }

        public Map map { get; set; }

        public int cx { get; set; }

        public int cy { get; set; }

        public override string ToString()
        {
            return "Chunk(" + cx + "," + cy + ")";
        }

        public void AddRoom(Room newRoom)
        {
            rooms.Add(newRoom);
        }

        public void DeleteRoom(Room r)
        {
            if (r == outsideRoom)
            {
                Utils.LogError("Tried to delete the outside room.");
                return;
            }

            // Remove this room from our rooms list.
            rooms.Remove(r);

            // All tiles that belonged to this room should be re-assigned to
            // the outside.
            r.ReturnTilesToOutsideRoom();
        }
    }
}
