
using Rimworld.logic;

namespace Rimworld.model.entities
{
    public class PhysicalEntity : GameEntity, ISelectableInterface
    {
        public PhysicalEntity()
            : base()
        {
            position = new Position(0, 0, 0);
            dimension = new Dimension(1, 1);
        }

        public string name { get; set; }
        public Position position { get; private set; }
        public Dimension dimension { get; private set; }

        internal void PlaceNear(Position pos)
        {
            if (pos == null)
            {
                Utils.LogError("PlaceNear pos is null!");
                return;
            }
            position.x = pos.x;
            position.y = pos.y;
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
                return position.x;
            }
        }

        public virtual float Y
        {
            get
            {
                return position.y;
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
            }
        }

        public Tile currTile
        {
            get { return World.map.GetTileAt(position); }

        }



        internal void Update(float deltaTime)
        {
        }
    }
}
