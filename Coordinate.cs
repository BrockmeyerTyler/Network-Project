

namespace Network_Project
{
    class Coordinate
    {
        public float x, y;

        public Coordinate(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public Coordinate(Coordinate coord)
        {
            x = coord.x;
            y = coord.y;
        }
    }
}
