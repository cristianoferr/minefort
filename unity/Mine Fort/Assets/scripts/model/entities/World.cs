/*
using System;
using System.Collections.Generic;
using UnityEngine;
using MineFort.model.entities.map;
using System.Xml;
using System.IO;
using MineFort.model.furniture;
using MineFort.model.inventory;
using MoonSharp.Interpreter;

namespace MineFort.model.entities
{
    [MoonSharpUserData]
    public class World_Old : GameEntity
    {
        public Dictionary<string, Furniture> furniturePrototypes;

        private World_Old()
        {
            
            SetupWorld(GameConsts.WORLD_WIDTH, GameConsts.WORLD_HEIGHT);

        }

       public override  void Start()
        {
            CreateFurniturePrototypes();
            CreateCharacter(GetTileAt(width / 2, height / 2));
        }


        private void SetupWorld(int width, int height)
        {
            mapData.SetupWorld(width, height);
            jobQueue = new JobQueue(); 
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
            set
            {
                world_ = value;
            }
        }
       

        #region properties
        public Path_TileGraph tileGraph;
        public Dictionary<string, Job> furnitureJobPrototypes;


        #endregion properties

        public GameCharacter CreateCharacter(Tile t)
        {
            Debug.Log("CreateCharacter");
            GameCharacter c = new GameCharacter();
            c.CurrTile = t;
            AddEntity(c);

            if (cbCharacterCreated != null)
                cbCharacterCreated(c);

            return c;
        }

        public bool ContainsEntity(PhysicalEntity pawn)
        {
            return entities.Contains(pawn);
        }

        public void OnInventoryCreated(GameInventory inv)
        {
            if (cbInventoryCreated != null)
                cbInventoryCreated(inv);
        }

        public void OnFurnitureRemoved(PhysicalEntity furn)
        {
            AddEntity(furn);
        }

        internal PhysicalEntity AddEntity(PhysicalEntity entity)
        {
            if (entities.Contains(entity))
            {
                Utils.LogError("Entities already contains "+entity);
            }
            entities.Add(entity);
            return entity;
        }

       
        #region Callbacks

        Action<Furniture> cbFurnitureCreated;
        Action<MovableEntity> cbCharacterCreated;
        Action<GameInventory> cbInventoryCreated;
        

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

        public void RegisterInventoryCreated(Action<GameInventory> callbackfunc)
        {
            cbInventoryCreated += callbackfunc;
        }

        public void UnregisterInventoryCreated(Action<GameInventory> callbackfunc)
        {
            cbInventoryCreated -= callbackfunc;
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

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            foreach (PhysicalEntity c in entities)
            {
                c.Update(deltaTime);
            }

        }

        public Tile GetTileAt(float x, float y)
        {
            return mapData.GetTileAt(x, y);
        }

        public Tile GetTileAt(Vector3 pos)
        {
            return mapData.GetTileAt(pos.x,pos.y);
        }


        // TODO: Most likely this will be replaced with a dedicated
        // class for managing job queues (plural!) that might also
        // be semi-static or self initializing or some damn thing.
        // For now, this is just a PUBLIC member of World
        public JobQueue jobQueue;
        public Biomes biomes;

        #region Furniture
        


        public void SetFurnitureJobPrototype(Job j, Furniture f)
        {
            furnitureJobPrototypes[f.objectType] = j;
        }

        void LoadFurnitureLua()
        {
            string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "LUA");
            filePath = System.IO.Path.Combine(filePath, "Furniture.lua");
            string myLuaCode = System.IO.File.ReadAllText(filePath);

            //Debug.Log("My LUA Code");
            //Debug.Log(myLuaCode);

            // Instantiate the singleton
            new FurnitureActions(myLuaCode);

        }

        void CreateFurniturePrototypes()
        {
            LoadFurnitureLua();


            furniturePrototypes = new Dictionary<string, Furniture>();
            furnitureJobPrototypes = new Dictionary<string, Job>();

            // READ FURNITURE PROTOTYPE XML FILE HERE
            // TODO:  Probably we should be getting past a StreamIO handle or the raw
            // text here, rather than opening the file ourselves.

            string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "Data");
            filePath = System.IO.Path.Combine(filePath, "Furniture.xml");
            string furnitureXmlText = System.IO.File.ReadAllText(filePath);

            XmlTextReader reader = new XmlTextReader(new StringReader(furnitureXmlText));

            int furnCount = 0;
            if (reader.ReadToDescendant("Furnitures"))
            {
                if (reader.ReadToDescendant("Furniture"))
                {
                    do
                    {
                        furnCount++;

                        Furniture furn = new Furniture();
                        furn.ReadXmlPrototype(reader);

                        furniturePrototypes[furn.objectType] = furn;



                    } while (reader.ReadToNextSibling("Furniture"));
                }
                else
                {
                    Debug.LogError("The furniture prototype definition file doesn't have any 'Furniture' elements.");
                }
            }
            else
            {
                Debug.LogError("Did not find a 'Furnitures' element in the prototype definition file.");
            }

            Debug.Log("Furniture prototypes read: " + furnCount.ToString());

            // This bit will come from parsing a LUA file later, but for now we still need to
            // implement furniture behaviour directly in C# code.
            //furniturePrototypes["Door"].RegisterUpdateAction( FurnitureActions.Door_UpdateAction );
            //furniturePrototypes["Door"].IsEnterable = FurnitureActions.Door_IsEnterable;

        }

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
            AddEntity(furn);

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

      

    }
}
*/