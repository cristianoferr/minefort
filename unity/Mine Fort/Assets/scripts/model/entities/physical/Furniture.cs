
using Rimworld.logic;
using Rimworld.logic.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using MoonSharp.Interpreter;
using System.Xml.Serialization;
using Rimworld.model.furniture;
using System.Xml.Schema;
using Rimworld.model.inventory;

namespace Rimworld.model.entities
{
    [MoonSharpUserData]
    public class Furniture : PhysicalEntity, IXmlSerializable, ISelectableInterface
    {
        #region constructors
        public Furniture()
            : base()
        {
            furnParameters = new Dictionary<string, float>();
            updateActions = new List<string>();

            jobs = new List<Job>();
            this.funcPositionValidation = this.DEFAULT__IsValidPosition;
            this.height = 1;
            this.width = 1;

        }
        virtual public Furniture Clone()
        {
            return new Furniture(this);
        }




        protected Furniture(Furniture other)
        {
            this.objectType = other.objectType;
            this.name = other.name;
            this.movementCost = other.movementCost;
            this.roomEnclosure = other.roomEnclosure;
            this.dimension.GetFrom(other.dimension);
            this.tint = other.tint;
            this.linksToNeighbour = other.linksToNeighbour;

            this.jobSpotOffset = other.jobSpotOffset;
            this.jobSpawnSpotOffset = other.jobSpawnSpotOffset;

            this.furnParameters = new Dictionary<string, float>(other.furnParameters);
            jobs = new List<Job>();

            if (other.updateActions != null)
                this.updateActions = new List<string>(other.updateActions);

            this.isEnterableAction = other.isEnterableAction;

            if (other.funcPositionValidation != null)
                this.funcPositionValidation = (Func<Tile, bool>)other.funcPositionValidation.Clone();

        }
        #endregion constructors

        #region getValidPosition

        Func<Tile, bool> funcPositionValidation;

        public bool IsValidPosition(Tile t)
        {
            return funcPositionValidation(t);
        }
        // FIXME: These functions should never be called directly,
        // so they probably shouldn't be public functions of Furniture
        // This will be replaced by validation checks fed to use from 
        // LUA files that will be customizable for each piece of furniture.
        // For example, a door might specific that it needs two walls to
        // connect to.
        protected bool DEFAULT__IsValidPosition(Tile t)
        {
            for (int x_off = (int)t.X; x_off < (t.X + width); x_off++)
            {
                for (int y_off = (int)t.Y; y_off < (t.Y + height); y_off++)
                {
                    Tile t2 = World.current.GetTileAt(x_off, y_off);

                    // Make sure tile is FLOOR
                    if (t2.Type ==null )//GameConsts.TileType.Floor)
                    {
                        return false;
                    }

                    // Make sure tile doesn't already have furniture
                    if (t2.furniture != null)
                    {
                        return false;
                    }

                }
            }


            return true;
        }


        #endregion getValidPosition

        #region Parameters
        //TODO: mudar isso para o parameter value

        /// <summary>
        /// Custom parameter for this particular piece of furniture.  We are
        /// using a dictionary because later, custom LUA function will be
        /// able to use whatever parameters the user/modder would like.
        /// Basically, the LUA code will bind to this dictionary.
        /// </summary>
        protected Dictionary<string, float> furnParameters;

        /// <summary>
        /// Gets the custom furniture parameter from a string key.
        /// </summary>
        /// <returns>The parameter value (float).</returns>
        /// <param name="key">Key string.</param>
        /// <param name="default_value">Default value.</param>
        public float GetParameter(string key, float default_value)
        {
            if (furnParameters.ContainsKey(key) == false)
            {
                return default_value;
            }

            return furnParameters[key];
        }

        public float GetParameter(string key)
        {
            return GetParameter(key, 0);
        }


        public void SetParameter(string key, float value)
        {
            furnParameters[key] = value;
        }

        public void ChangeParameter(string key, float value)
        {
            if (furnParameters.ContainsKey(key) == false)
            {
                furnParameters[key] = value;
            }

            furnParameters[key] += value;
        }

        #endregion Parameters

        /// <summary>
        /// Registers a function that will be called every Update.
        /// (Later this implementation might change a bit as we support LUA.)
        /// </summary>
        public void RegisterUpdateAction(string luaFunctionName)
        {
            updateActions.Add(luaFunctionName);
        }

        public void UnregisterUpdateAction(string luaFunctionName)
        {
            updateActions.Remove(luaFunctionName);
        }

        #region properties
        List<Job> jobs;

        public void Deconstruct()
        {
            Debug.Log("Deconstruct");

            tile.UnplaceFurniture();

            if (cbOnRemoved != null)
                cbOnRemoved(this);

            // Do we need to recalculate our rooms?
            if (roomEnclosure)
            {
                Room.DoRoomFloodFill(this.tile);
            }

            World.current.InvalidateTileGraph();

            // At this point, no DATA structures should be pointing to us, so we
            // should get garbage-collected.

        }
        

        public bool IsStockpile()
        {
            return objectType == "Stockpile";
        }


        	public GameConsts.ENTERABILITY IsEnterable() {
		if(isEnterableAction == null || isEnterableAction.Length == 0) {
			return GameConsts.ENTERABILITY.Yes;
		}

		//FurnitureActions.CallFunctionsWithFurniture( isEnterableActions.ToArray(), this );

		DynValue ret = FurnitureActions.CallFunction( isEnterableAction, this );

		return (GameConsts.ENTERABILITY)ret.Number;

	}

	// This represents the BASE tile of the object -- but in practice, large objects may actually occupy
	// multile tiles.
	public Tile tile {
		get; protected set;
	}

	// This "objectType" will be queried by the visual system to know what sprite to render for this object
	public string objectType {
		get; protected set;
	}

        /// <summary>
        /// These actions are called every update. They get passed the furniture
        /// they belong to, plus a deltaTime.
        /// </summary>
        //protected Action<Furniture, float> updateActions;
        protected List<string> updateActions;

        //public Func<Furniture, ENTERABILITY> IsEnterable;
        protected string isEnterableAction;
        // This "objectType" will be queried by the visual system to know what sprite to render for this object
      

        public bool roomEnclosure { get; protected set; }
        public Color tint = Color.white;

        // If this furniture gets worked by a person,
        // where is the correct spot for them to stand,
        // relative to the bottom-left tile of the sprite.
        // NOTE: This could even be something outside of the actual
        // furniture tile itself!  (In fact, this will probably be common).
        public Vector2 jobSpotOffset = Vector2.zero;

        #region readXML
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("X", tile.X.ToString());
            writer.WriteAttributeString("Y", tile.Y.ToString());
            writer.WriteAttributeString("objectType", objectType);
            //writer.WriteAttributeString( "movementCost", movementCost.ToString() );

            foreach (string k in furnParameters.Keys)
            {
                writer.WriteStartElement("Param");
                writer.WriteAttributeString("name", k);
                writer.WriteAttributeString("value", furnParameters[k].ToString());
                writer.WriteEndElement();
            }

        }

        public void ReadXmlPrototype(XmlReader reader_parent)
        {
            //Debug.Log("ReadXmlPrototype");

            objectType = reader_parent.GetAttribute("objectType");

            XmlReader reader = reader_parent.ReadSubtree();


            while (reader.Read())
            {
                switch (reader.Name)
                {
                    case "Name":
                        reader.Read();
                        name = reader.ReadContentAsString();
                        break;
                    case "MovementCost":
                        reader.Read();
                        movementCost = reader.ReadContentAsFloat();
                        break;
                    case "Width":
                        reader.Read();
                        width = reader.ReadContentAsInt();
                        break;
                    case "Height":
                        reader.Read();
                        height = reader.ReadContentAsInt();
                        break;
                    case "LinksToNeighbours":
                        reader.Read();
                        linksToNeighbour = reader.ReadContentAsBoolean();
                        break;
                    case "EnclosesRooms":
                        reader.Read();
                        roomEnclosure = reader.ReadContentAsBoolean();
                        break;
                    case "BuildingJob":
                        float jobTime = float.Parse(reader.GetAttribute("jobTime"));

                        List<GameInventory> invs = new List<GameInventory>();

                        XmlReader invs_reader = reader.ReadSubtree();

                        while (invs_reader.Read())
                        {
                            if (invs_reader.Name == "Inventory")
                            {
                                // Found an inventory requirement, so add it to the list!
                                invs.Add(new GameInventory(
                                    invs_reader.GetAttribute("objectType"),
                                    int.Parse(invs_reader.GetAttribute("amount")),
                                    0
                                ));
                            }
                        }

                        Job j = new Job(null,
                            objectType,
                            FurnitureActions.JobComplete_FurnitureBuilding, jobTime,
                            invs.ToArray()
                        );

                        World.current.SetFurnitureJobPrototype(j, this);

                        break;
                    case "OnUpdate":

                        string functionName = reader.GetAttribute("FunctionName");
                        RegisterUpdateAction(functionName);

                        break;
                    case "IsEnterable":

                        isEnterableAction = reader.GetAttribute("FunctionName");

                        break;

                    case "JobSpotOffset":
                        jobSpotOffset = new Vector2(
                            int.Parse(reader.GetAttribute("X")),
                            int.Parse(reader.GetAttribute("Y"))
                        );

                        break;
                    case "JobSpawnSpotOffset":
                        jobSpawnSpotOffset = new Vector2(
                            int.Parse(reader.GetAttribute("X")),
                            int.Parse(reader.GetAttribute("Y"))
                        );

                        break;
                    case "Params":
                        ReadXmlParams(reader);  // Read in the Param tag
                        break;
                }
            }


        }

        public void ReadXml(XmlReader reader)
        {
            // X, Y, and objectType have already been set, and we should already
            // be assigned to a tile.  So just read extra data.

            //movementCost = int.Parse( reader.GetAttribute("movementCost") );

            ReadXmlParams(reader);
        }

        public void ReadXmlParams(XmlReader reader)
        {
            // X, Y, and objectType have already been set, and we should already
            // be assigned to a tile.  So just read extra data.

            //movementCost = int.Parse( reader.GetAttribute("movementCost") );

            if (reader.ReadToDescendant("Param"))
            {
                do
                {
                    string k = reader.GetAttribute("name");
                    float v = float.Parse(reader.GetAttribute("value"));
                    furnParameters[k] = v;
                } while (reader.ReadToNextSibling("Param"));
            }
        }
        #endregion readXML

        // If the job causes some kind of object to be spawned, where will it appear?
        public Vector2 jobSpawnSpotOffset = Vector2.zero;

        
        public bool linksToNeighbour
        {
            get;
            protected set;
        }

        #endregion properties

        

        #region ISelectableInterface
        public string GetName()
        {
            return this.name;
        }

        public string GetDescription()
        {
            return "This is a piece of furniture."; // TODO: Add "Description" property and matching XML field.
        }

        public string GetHitPointString()
        {
            return "18/18"; // TODO: Add a hitpoint system to...well...everything
        }
        #endregion ISelectableInterface




        // This is a multipler. So a value of "2" here, means you move twice as slowly (i.e. at half speed)
        // Tile types and other environmental effects may be combined.
        // For example, a "rough" tile (cost of 2) with a table (cost of 3) that is on fire (cost of 3)
        // would have a total movement cost of (2+3+3 = 8), so you'd move through this tile at 1/8th normal speed.
        // SPECIAL: If movementCost = 0, then this tile is impassible. (e.g. a wall).
        public float movementCost { get; protected set; }

        static public Furniture PlaceInstance(Furniture proto, Tile tile)
        {
            if (proto.funcPositionValidation(tile) == false)
            {
                Utils.LogError("PlaceInstance -- Position Validity Function returned FALSE.");
                return null;
            }

            // We know our placement destination is valid.
            Furniture obj = proto.Clone();
            obj.tile = proto.tile;

            // FIXME: This assumes we are 1x1!
            if (tile.PlaceFurniture(obj) == false)
            {
                // For some reason, we weren't able to place our object in this tile.
                // (Probably it was already occupied.)

                // Do NOT return our newly instantiated object.
                // (It will be garbage collected.)
                return null;
            }

            if (obj.linksToNeighbour)
            {
                // This type of furniture links itself to its neighbours,
                // so we should inform our neighbours that they have a new
                // buddy.  Just trigger their OnChangedCallback.

                Tile t;
                float x = tile.X;
                float y = tile.Y;

                t = World.current.GetTileAt(x, y + 1);
                if (t != null && t.furniture != null && t.furniture.cbOnChanged != null && t.furniture.objectType == obj.objectType)
                {
                    // We have a Northern Neighbour with the same object type as us, so
                    // tell it that it has changed by firing is callback.
                    t.furniture.cbOnChanged(t.furniture);
                }
                t = World.current.GetTileAt(x + 1, y);
                if (t != null && t.furniture != null && t.furniture.cbOnChanged != null && t.furniture.objectType == obj.objectType)
                {
                    t.furniture.cbOnChanged(t.furniture);
                }
                t = World.current.GetTileAt(x, y - 1);
                if (t != null && t.furniture != null && t.furniture.cbOnChanged != null && t.furniture.objectType == obj.objectType)
                {
                    t.furniture.cbOnChanged(t.furniture);
                }
                t = World.current.GetTileAt(x - 1, y);
                if (t != null && t.furniture != null && t.furniture.cbOnChanged != null && t.furniture.objectType == obj.objectType)
                {
                    t.furniture.cbOnChanged(t.furniture);
                }

            }

            return obj;
        }
    }
}
