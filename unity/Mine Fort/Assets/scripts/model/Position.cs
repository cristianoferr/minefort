
namespace Rimworld.model
{
    public class Position
    {
        public int x, y, facing;

        public Position(int x, int y, int facing = 0)
        {
            this.x = x;
            this.y = y;
            this.facing = facing;
        }

        public override string ToString()
        {
            return "P(" + x + "," + y + ")";
        }


        internal void getFrom(Position position)
        {
            x = position.x;
            y = position.y;
            facing = position.facing;
        }
    }
}
