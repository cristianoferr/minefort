
using Rimworld.logic;
using System;
using UnityEngine;

namespace Rimworld.model.entities
{
    public class PhysicalEntity : GameEntity, ISelectableInterface
    {
        public PhysicalEntity()
            : base()
        {
          //  position = new Vector3(0, 0, 0);
            dimension = new Dimension(1, 1);
        }

        public string name { get; set; }
        //If true then cbOnChanged is called on update
        protected bool hasChanged = false;
        // public Vector3 position;

        Tile _currTile=null;
        /// <summary>
        /// The tile the Character is considered to still be standing in.
        /// </summary>
        public Tile CurrTile
        {
            get
            {
                return CurrTile;
            }

            set
            {
                if (CurrTile != null)
                {
                    CurrTile.Characters.Remove(this);
                }

                CurrTile = value;
                CurrTile.Characters.Add(this);

                TileOffset = Vector3.zero;
            }
        }

        /// Tile offset for animation
        public Vector3 TileOffset { get; set; }


        public Dimension dimension { get; private set; }

        #region callbacks
        public Action<PhysicalEntity> cbOnChanged;
        public Action<PhysicalEntity> cbOnRemoved;


        public void RegisterOnChangedCallback(Action<PhysicalEntity> callbackFunc)
        {
            cbOnChanged += callbackFunc;
        }

        public void UnregisterOnChangedCallback(Action<PhysicalEntity> callbackFunc)
        {
            cbOnChanged -= callbackFunc;
        }

        public void RegisterOnRemovedCallback(Action<PhysicalEntity> callbackFunc)
        {
            cbOnRemoved += callbackFunc;
        }

        public void UnregisterOnRemovedCallback(Action<PhysicalEntity> callbackFunc)
        {
            cbOnRemoved -= callbackFunc;
        }
        #endregion callbacks



        internal void PlaceNear(Vector3 pos)
        {
            if (pos == null)
            {
                Utils.LogError("PlaceNear pos is null!");
                return;
            }
            CurrTile = World.Current.GetTileAt(pos);
        }

        
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            if (cbOnChanged != null && hasChanged)
            {
                cbOnChanged(this);
                hasChanged = false;
            }
        }


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

        /// <summary>
        /// Returns a float representing the Character's X position, which can
        /// be part-way between two tiles during movement.
        /// </summary>
        public float X
        {
            get
            {
                return CurrTile.X + TileOffset.x;
            }
        }

        /// <summary>
        /// Returns a float representing the Character's Y position, which can
        /// be part-way between two tiles during movement.
        /// </summary>
        public float Y
        {
            get
            {
                return CurrTile.Y + TileOffset.y;
            }
        }

        /// <summary>
        /// Returns a float representing the Character's Z position, which can
        /// be part-way between two tiles during movement.
        /// </summary>
        public float Z
        {
            get
            {
                return CurrTile.Z + TileOffset.z;
            }
        }

        public int height
        {
            get
            {
                return dimension.height;
            }
            set
            {
                dimension.height = value;
                hasChanged = true;
            }
        }

        public int width
        {
            get
            {
                return dimension.width;
            }
            set
            {
                dimension.width = value;
                hasChanged = true;
            }
        }

        


    }
}
