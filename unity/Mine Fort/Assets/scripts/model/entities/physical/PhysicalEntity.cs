
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
        public Tile currTile { get {
                return _currTile;
            }
            set
            {
                if (_currTile != value)
                {
                    _currTile = value;
                    hasChanged = true;
                }
            }

        }

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
            currTile = World.current.GetTileAt(pos);
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

        //X e Y serão usados visualmente
        public virtual float X
        {
            get
            {
                return currTile.X;
            }
        }

        public virtual float Y
        {
            get
            {
                return currTile.Y;
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
