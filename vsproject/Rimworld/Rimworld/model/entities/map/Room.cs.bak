
using System.Collections.Generic;
namespace Rimworld.model.entities
{
    public class Room : PhysicalEntity
    {
        IList<Tile> tiles;
        private map.Chunk chunk;

        public Room(map.Chunk chunk)
        {
            tiles = new List<Tile>();
            this.chunk = chunk;
        }

        public void AssignTile(Tile t)
        {
            if (tiles.Contains(t))
            {
                // This tile already in this room.
                return;
            }

            if (t.room != null)
            {
                // Belongs to some other room
                t.room.tiles.Remove(t);
            }

            t.room = this;
            tiles.Add(t);
        }

        public void ReturnTilesToOutsideRoom()
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                tiles[i].room = World.map.GetOutsideRoom(position);	// Assign to outside
            }
            tiles = new List<Tile>();
        }

        public bool IsOutsideRoom()
        {
            return this == World.map.GetOutsideRoom(position);
        }
    }
}
