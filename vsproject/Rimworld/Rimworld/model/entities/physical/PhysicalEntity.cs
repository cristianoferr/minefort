
namespace Rimworld.model.entities
{
    public class PhysicalEntity : GameEntity
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
        }

        public int width
        {
            get
            {
                return dimension.width;
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
