
using Rimworld.logic;
using Rimworld.logic.Jobs;
using Rimworld.Utilities.Pathfinding;
using System;
using System.Collections.Generic;
namespace Rimworld.model.entities
{
    public class World : GameEntity
    {


        private World()
        {
            mapData = new Map();
            SetupWorld(GameConsts.WORLD_WIDTH, GameConsts.WORLD_HEIGHT);
            entities = new List<PhysicalEntity>();

        }

        

        //singleton
        static World world_;
        public static World current
        {
            get
            {
                //mundo será instanciado vazio.
                if (world_ == null)
                {
                    world_ = new World();
                }
                return world_;
            }
        }
        public override void Initialize()
        {
            base.Initialize();
        }

        #region properties
        public Map mapData { get; private set; }
        public Path_TileGraph tileGraph;
        public Dictionary<string, Job> furnitureJobPrototypes;
        public static Map map
        {
            get { return world_.mapData; }
        }
        public IList<PhysicalEntity> entities { get; private set; }


        #endregion properties

        public bool ContainsEntity(PhysicalEntity pawn)
        {
            return entities.Contains(pawn);
        }


        internal PhysicalEntity AddEntity(PhysicalEntity entity)
        {
            entities.Add(entity);
            return entity;
        }

        private void SetupWorld(int width, int height)
        {
            mapData.SetupWorld(width, height);
        }

        #region Callbacks

        Action<Furniture> cbFurnitureCreated;
        Action<MovableEntity> cbCharacterCreated;
        Action<Inventory> cbInventoryCreated;
        Action<Tile> cbTileChanged;

        public void RegisterFurnitureCreated(Action<Furniture> callbackfunc)
        {
            cbFurnitureCreated += callbackfunc;
        }

        public void UnregisterFurnitureCreated(Action<Furniture> callbackfunc)
        {
            cbFurnitureCreated -= callbackfunc;
        }

        public void RegisterCharacterCreated(Action<MovableEntity> callbackfunc)
        {
            cbCharacterCreated += callbackfunc;
        }

        public void UnregisterCharacterCreated(Action<MovableEntity> callbackfunc)
        {
            cbCharacterCreated -= callbackfunc;
        }

        public void RegisterInventoryCreated(Action<Inventory> callbackfunc)
        {
            cbInventoryCreated += callbackfunc;
        }

        public void UnregisterInventoryCreated(Action<Inventory> callbackfunc)
        {
            cbInventoryCreated -= callbackfunc;
        }

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

            InvalidateTileGraph();
        }

        public void DeleteRoom(Room r)
        {
            r.chunk.DeleteRoom(r);
        }

        #endregion Callbacks

        public void InvalidateTileGraph()
        {
            tileGraph = null;
        }

        internal void Update(float deltaTime)
        {
            foreach (PhysicalEntity c in entities)
            {
                c.Update(deltaTime);
            }

            /*foreach (Furniture f in furnitures)
            {
                f.Update(deltaTime);
            }*/
        }

        public Tile GetTileAt(int x, int y)
        {
            return mapData.GetTileAt(x, y);
        }

        public int width
        {
            get
            {
                return mapData.width;
            }
        }

        public int height
        {
            get
            {
                return mapData.height;
            }
        }

        #region Furniture
        public Dictionary<string, Furniture> furniturePrototypes;
        public List<Furniture> furnitures;

        // TODO: Most likely this will be replaced with a dedicated
        // class for managing job queues (plural!) that might also
        // be semi-static or self initializing or some damn thing.
        // For now, this is just a PUBLIC member of World
        public JobQueue jobQueue;

        public bool IsFurniturePlacementValid(string furnitureType, Tile t)
        {
            return furniturePrototypes[furnitureType].IsValidPosition(t);
        }
        #endregion Furniture

        internal Furniture PlaceFurniture(string objectType, Tile t, bool doRoomFloodFill = true)
        {
            //Debug.Log("PlaceInstalledObject");
            // TODO: This function assumes 1x1 tiles -- change this later!

            if (furniturePrototypes.ContainsKey(objectType) == false)
            {
                Utils.LogError("furniturePrototypes doesn't contain a proto for key: " + objectType);
                return null;
            }

            Furniture furn = Furniture.PlaceInstance(furniturePrototypes[objectType], t);

            if (furn == null)
            {
                // Failed to place object -- most likely there was already something there.
                return null;
            }

            furn.RegisterOnRemovedCallback(OnFurnitureRemoved);
            furnitures.Add(furn);

            // Do we need to recalculate our rooms?
            if (doRoomFloodFill && furn.roomEnclosure)
            {
                Room.DoRoomFloodFill(furn.tile);
            }



            if (cbFurnitureCreated != null)
            {
                cbFurnitureCreated(furn);

                if (furn.movementCost != 1)
                {
                    // Since tiles return movement cost as their base cost multiplied
                    // buy the furniture's movement cost, a furniture movement cost
                    // of exactly 1 doesn't impact our pathfinding system, so we can
                    // occasionally avoid invalidating pathfinding graphs
                    InvalidateTileGraph();	// Reset the pathfinding system
                }
            }

            return furn;
        }

        public void OnFurnitureRemoved(Furniture furn)
        {
            furnitures.Remove(furn);
        }

        internal void AddRoom(Room newRoom)
        {
            newRoom.chunk.AddRoom(newRoom);
        }
    }
}
