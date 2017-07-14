
using Rimworld.logic.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Rimworld.model.entities
{
    public class Furniture : PhysicalEntity
    {
        #region constructors
        public Furniture()
            : base()
        {
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

        #region properties
        List<Job> jobs;
        /// <summary>
        /// Custom parameter for this particular piece of furniture.  We are
        /// using a dictionary because later, custom LUA function will be
        /// able to use whatever parameters the user/modder would like.
        /// Basically, the LUA code will bind to this dictionary.
        /// </summary>
        protected Dictionary<string, float> furnParameters;

        /// <summary>
        /// These actions are called every update. They get passed the furniture
        /// they belong to, plus a deltaTime.
        /// </summary>
        //protected Action<Furniture, float> updateActions;
        protected List<string> updateActions;

        //public Func<Furniture, ENTERABILITY> IsEnterable;
        protected string isEnterableAction;
        // This "objectType" will be queried by the visual system to know what sprite to render for this object
        public string objectType
        {
            get;
            protected set;
        }

        public bool roomEnclosure { get; protected set; }
        public Color tint = Color.white;

        // If this furniture gets worked by a person,
        // where is the correct spot for them to stand,
        // relative to the bottom-left tile of the sprite.
        // NOTE: This could even be something outside of the actual
        // furniture tile itself!  (In fact, this will probably be common).
        public Vector2 jobSpotOffset = Vector2.zero;

        // If the job causes some kind of object to be spawned, where will it appear?
        public Vector2 jobSpawnSpotOffset = Vector2.zero;
        public Action<Furniture> cbOnChanged;
        public Action<Furniture> cbOnRemoved;

        Func<Tile, bool> funcPositionValidation;

        public bool linksToNeighbour
        {
            get;
            protected set;
        }

        #endregion properties

        #region callbacks
        public void RegisterOnChangedCallback(Action<Furniture> callbackFunc)
        {
            cbOnChanged += callbackFunc;
        }

        public void UnregisterOnChangedCallback(Action<Furniture> callbackFunc)
        {
            cbOnChanged -= callbackFunc;
        }

        public void RegisterOnRemovedCallback(Action<Furniture> callbackFunc)
        {
            cbOnRemoved += callbackFunc;
        }

        public void UnregisterOnRemovedCallback(Action<Furniture> callbackFunc)
        {
            cbOnRemoved -= callbackFunc;
        }

        #endregion callbacks


        internal bool IsValidPosition(Tile t)
        {
            for (int x_off = t.X; x_off < (t.X + width); x_off++)
            {
                for (int y_off = t.Y; y_off < (t.Y + height); y_off++)
                {
                    Tile t2 = World.current.GetTileAt(x_off, y_off);

                    // Make sure tile is FLOOR
                    if (t2.Type != GameConsts.TileType.Floor)
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
            obj.position.getFrom(proto.position);

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
                int x = tile.X;
                int y = tile.Y;

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
