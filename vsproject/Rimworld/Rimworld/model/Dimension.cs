
namespace Rimworld.model
{
    public class Dimension
    {
        public int width, height;

        public Dimension(int p1, int p2)
        {
            this.width = p1;
            this.height = p2;
        }

        public void GetFrom(Dimension dimension)
        {
            this.width = dimension.width;
            this.height = dimension.height;
        }
    }
}
