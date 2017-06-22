using System;
namespace RouteBuilder
{
    public class Detection : IComparable<Detection>
    {
        public int BSID;
        public int MAC;
        public double time;

        public Detection(int BSID, int MAC, double time)
        {
            this.BSID = BSID;
            this.MAC = MAC;
            this.time = time;
        }

        public int CompareTo(Detection other)
        {
            if (this.time > other.time)
                return 1;

            else if (this.time < other.time)
                return -1;

            else
                return 0;
            
        }
    }
}
