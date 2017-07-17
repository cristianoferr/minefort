
using Rimworld.logic;
using Rimworld.logic.Jobs;
using Rimworld.model.entities.map.tiles;
using Rimworld.model.inventory;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Rimworld.model.entities
{

    public class Tile:ISelectableInterface
    {

        IList<PhysicalEntity> entitiesHere;
        public List<PhysicalEntity> characters;
        public float X { get; private set; }
        public float Y { get; private set; }
        public Tile(map.Chunk chunk, int i, int j)
        {
            
            this.chunk = chunk;
            position = new Vector3(i, j, 0);
            room = chunk.outsideRoom;
            X = (chunk.cx * GameConsts.CHUNK_SIZE) + position.x;
            Y = (chunk.cy * GameConsts.CHUNK_SIZE) + position.y;
        }


        public map.Chunk chunk { get; private set; }
        public Vector3 position;
        public Room room { get; set; }
        public GameInventory inventory;

        public override string ToString()
        {
            return "Tile[" + X + "," + Y + "] @room: " + room;
        }

        public void RemoveEntity(PhysicalEntity physicalEntity)
        {
            if (entitiesHere == null) return;
            entitiesHere.Remove(physicalEntity);
        }

        public void AddEntity(PhysicalEntity physicalEntity)
        {
            if (entitiesHere == null)
            {
                entitiesHere = new List<PhysicalEntity>();
            }
            entitiesHere.Add(physicalEntity);
        }

        internal void RandomizeHeight()
        {
            if (_type == null) return;
            height = UnityEngine.Random.Range(_type.MinHeight, _type.MaxHeight);
            if (cbTileChanged != null)
            {
                cbTileChanged(this);
            }
        }

        #region ISelectableInterface implementation

        public string GetName()
        {
            return this._type.ToString();
        }

        public string GetDescription()
        {
            return "The tile.";
        }

        public string GetHitPointString()
        {
            return "";  // Do tiles have hitpoints? Can flooring be damaged? Obviously "empty" is indestructible.
        }

        #endregion

        /// <summary>
        /// Gets the neighbours.
        /// </summary>
        /// <returns>The neighbours.</returns>
        /// <param name="diagOkay">Is diagonal movement okay?.</param>
        public Tile[] GetNeighbours(bool diagOkay = false)
        {
            Tile[] ns;

            if (diagOkay == false)
            {
                ns = new Tile[4];	// Tile order: N E S W
            }
            else
            {
                ns = new Tile[8];	// Tile order : N E S W NE SE SW NW
            }

            Tile n;

            n = World.map.GetTileAt(X, Y + 1);
            ns[0] = n;	// Could be null, but that's okay.
            n = World.map.GetTileAt(X + 1, Y);
            ns[1] = n;	// Could be null, but that's okay.
            n = World.map.GetTileAt(X, Y - 1);
            ns[2] = n;	// Could be null, but that's okay.
            n = World.map.GetTileAt(X - 1, Y);
            ns[3] = n;	// Could be null, but that's okay.

            if (diagOkay == true)
            {
                n = World.map.GetTileAt(X + 1, Y + 1);
                ns[4] = n;	// Could be null, but that's okay.
                n = World.map.GetTileAt(X + 1, Y - 1);
                ns[5] = n;	// Could be null, but that's okay.
                n = World.map.GetTileAt(X - 1, Y - 1);
                ns[6] = n;	// Could be null, but that's okay.
                n = World.map.GetTileAt(X - 1, Y + 1);
                ns[7] = n;	// Could be null, but that's okay.
            }

            return ns;
        }

        public GameConsts.ENTERABILITY IsEnterable()
        {
            // This returns true if you can enter this tile right this moment.
            if (movementCost == 0)
                return GameConsts.ENTERABILITY.Never;

            // Check out furniture to see if it has a special block on enterability
            if (furniture != null)
            {
                return furniture.IsEnterable();
            }

            return GameConsts.ENTERABILITY.Yes;
        }

        public float movementCost
        {
            get
            {

                if (Type == null)
                    return 0;	// 0 is unwalkable

                if (furniture == null)
                    return _type.MovementCost;

                return _type.MovementCost * furniture.movementCost;
            }
        }

        private TileData _type = null;
        public TileData Type
        {
            get { return _type; }
            set
            {
                TileData oldType = _type;
                _type = value;
                if (oldType == null)
                {
                    RandomizeHeight();
                }
                // Call the callback and let things know we've changed.

                if (cbTileChanged != null && oldType != _type)
                {
                    cbTileChanged(this);
                }
            }
        }

        // The function we callback any time our tile's data changes
        Action<Tile> cbTileChanged;

       

        public bool IsNeighbour(Tile tile, bool diagOkay = false)
        {
            // Check to see if we have a difference of exactly ONE between the two
            // tile coordinates.  Is so, then we are vertical or horizontal neighbours.
            return
                Mathf.Abs(this.X - tile.X) + Mathf.Abs(this.Y - tile.Y) == 1 ||  // Check hori/vert adjacency
                (diagOkay && (Mathf.Abs(this.X - tile.X) == 1 && Mathf.Abs(this.Y - tile.Y) == 1)) // Check diag adjacency
                ;
        }

        /// <summary>
        /// Register a function to be called back when our tile type changes.
        /// </summary>
        public void RegisterTileTypeChangedCallback(Action<Tile> callback)
        {
            cbTileChanged += callback;
        }

        /// <summary>
        /// Unregister a callback.
        /// </summary>
        public void UnregisterTileTypeChangedCallback(Action<Tile> callback)
        {
            cbTileChanged -= callback;
        }


        // Furniture is something like a wall, door, or sofa.
        public Furniture furniture
        {
            get;
            protected set;
        }

        

        public bool UnplaceFurniture()
        {
            // Just uninstalling.  FIXME:  What if we have a multi-tile furniture?

            if (furniture == null)
                return false;

            Furniture f = furniture;

            for (int x_off = (int)X; x_off < (X + f.width); x_off++)
            {
                for (int y_off = (int)Y; y_off < (Y + f.height); y_off++)
                {

                    Tile t = World.current.GetTileAt(x_off, y_off);
                    t.furniture = null;
                }
            }

            return true;
        }


        // TODO: This seems like a terrible way to flag if a job is pending
        // on a tile.  This is going to be prone to errors in set/clear.
        public Job pendingFurnitureJob;
        internal float height
        {
            get
            {
                return position.z;
            }
            set { position.z = value; }
        }

        public int charactersCount
        {
            get
            {
                if (characters == null) return 0;
                return characters.Count;
            }
        }

        public bool PlaceFurniture(Furniture objInstance)
        {

            if (objInstance == null)
            {
                return UnplaceFurniture();
            }

            if (objInstance.IsValidPosition(this) == false)
            {
                Debug.LogError("Trying to assign a furniture to a tile that isn't valid!");
                return false;
            }

            for (int x_off = (int)X; x_off < (X + objInstance.width); x_off++)
            {
                for (int y_off = (int)Y; y_off < (Y + objInstance.height); y_off++)
                {
                    Tile t = World.current.GetTileAt(x_off, y_off);
                    t.furniture = objInstance;
                }
            }

            return true;
        }

        public bool PlaceInventory(GameInventory inv)
        {
            if (inv == null)
            {
                inventory = null;
                return true;
            }

            if (inventory != null)
            {
                // There's already inventory here. Maybe we can combine a stack?

                if (inventory.objectType != inv.objectType)
                {
                    Debug.LogError("Trying to assign inventory to a tile that already has some of a different type.");
                    return false;
                }

                int numToMove = inv.stackSize;
                if (inventory.stackSize + numToMove > inventory.maxStackSize)
                {
                    numToMove = inventory.maxStackSize - inventory.stackSize;
                }

                inventory.stackSize += numToMove;
                inv.stackSize -= numToMove;

                return true;
            }

            // At this point, we know that our current inventory is actually
            // null.  Now we can't just do a direct assignment, because
            // the inventory manager needs to know that the old stack is now
            // empty and has to be removed from the previous lists.

            inventory = inv.Clone();
            inventory.tile = this;
            inv.stackSize = 0;

            return true;
        }
    }
}
