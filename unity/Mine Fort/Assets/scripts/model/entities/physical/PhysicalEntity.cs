
using MineFort.Entities;
using MineFort.Localization;
using MineFort.logic;
using MineFort.model;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MineFort.Entities
{
    public class PhysicalEntity : GameEntity, ISelectable
    {
        /// Unique ID of the character.
        public readonly int ID;
        /// What ID we currently are sitting at
        private static int currentID = 0;

        public PhysicalEntity()
            : base()
        {
          //  position = new Vector3(0, 0, 0);
            dimension = new Dimension(1, 1);
            ID = currentID++;
        }

        public string Name { get; set; }
        //If true then cbOnChanged is called on update
        protected bool hasChanged = false;

        /// Stats, for character.
        public Dictionary<string, Stat> Stats { get; protected set; }

        internal bool selected = false;
        public bool IsSelected
        {
            get
            {
                return selected;
            }

            set
            {
                if (value == false)
                {
                    VisualPath.Instance.RemoveVisualPoints(ID);
                }

                selected = value;
            }
        }

        public Tile CurrTile
        {
            get
            {
                return _currTile;
            }
        }

            Tile _currTile =null;
        /// <summary>
        /// The tile the Character is considered to still be standing in.
        /// </summary>
        public Tile Tile
        {
            get
            {
                return _currTile;
            }

            set
            {
                if (_currTile != null)
                {
                    _currTile.Characters.Remove(this);
                }


                _currTile = value.GetValidGroundTile();
                _currTile.Characters.Add(this);

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
            Tile = World.Current.GetTileAt(pos);
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

        public virtual string GetJobDescription()
        {
            return "";
        }
            public virtual string GetName()
        {
            return this.Name;
        }

        public virtual string GetDescription()
        {
            return "This is a physical entity"; // TODO: Add "Description" property and matching XML field.
        }

        public virtual IEnumerable<string> GetAdditionalInfo()
        {
            foreach (string stat in Stats.Keys)
            {
                yield return LocalizationTable.GetLocalization("stat_" + stat.ToLower(), Stats[stat].Value);
            }
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
                return Tile.X + TileOffset.x;
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
                return Tile.Y + TileOffset.y;
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
                return Tile.Z + TileOffset.z;
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
