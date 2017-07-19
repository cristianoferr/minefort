
using Boo.Lang;
using Rimworld.model.inventory;
using Rimworld.model.Inventory;
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
        internal Path_AStar pathAStar;

        public Tile nextTile { get; set; }
        public GameInventory inventory;
        
        public override void Update(float deltaTime)
        {
            //Debug.Log("Character Update");
            base.Update(deltaTime);

            //Update_DoMovement(deltaTime);

        }

        public virtual void AbandonJob()
        {
            nextTile = DestTile = CurrTile;
        }




        




        public void SetDestination(Tile tile)
        {
            if (CurrTile.IsNeighbour(tile, true) == false)
            {
                Utils.Log("Character::SetDestination -- Our destination tile isn't actually our neighbour.");
            }

            DestTile = tile;
        }

        // If we aren't moving, then destTile = currTile
        internal Tile _destTile;
        internal Tile DestTile
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

        
        float movementPercentage; // Goes from 0 to 1 as we move from currTile to destTile

        float speed = 5f;   // Tiles per second
                            /// Tiles per second.
        public float MovementSpeed
        {
            get
            {
                return speed;
            }
        }

        /// Holds the path to reach DestTile.
        private List<Tile> movementPath;
    }
}
