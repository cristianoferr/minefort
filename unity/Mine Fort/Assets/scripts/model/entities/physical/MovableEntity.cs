
using Rimworld.model.Pathfinding;
using UnityEngine;
namespace Rimworld.model.entities
{
    //C vvvxxxx
    public class MovableEntity : PhysicalEntity
    {
        public MovableEntity()
            : base()
        {
        }

        public Tile nextTile { get; set; }
        public override float X
        {
            get
            {
                if (nextTile == null)
                {
                    return position.x;
                }
                return Mathf.Lerp(currTile.X, nextTile.X, movementPercentage);
            }
        }

        public override float Y
        {
            get
            {
                if (nextTile == null)
                {
                    return position.y;
                }
                return Mathf.Lerp(currTile.Y, nextTile.Y, movementPercentage);

            }
        }

        public void SetDestination(Tile tile)
        {
            if (currTile.IsNeighbour(tile, true) == false)
            {
                Utils.Log("Character::SetDestination -- Our destination tile isn't actually our neighbour.");
            }

            DestTile = tile;
        }

        // If we aren't moving, then destTile = currTile
        Tile _destTile;
        Tile DestTile
        {
            get { return _destTile; }
            set
            {
                if (_destTile != value)
                {
                    _destTile = value;
                    pathAStar = null;	// If this is a new destination, then we need to invalidate pathfinding.
                }
            }
        }

        Path_AStar pathAStar;
        float movementPercentage; // Goes from 0 to 1 as we move from currTile to destTile

        float speed = 5f;	// Tiles per second

    }
}
