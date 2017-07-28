using System;
namespace RouteBuilder
{
    public class Detection : IComparable<Detection>
    {
        //Class elements
        public int BSID;
        public int MAC;
        public double time;

        //Constructor
        public Detection(int BSID, int MAC, double time)
        {
            this.BSID = BSID;
            this.MAC = MAC;
            this.time = time;
        }

        //Method 1: IComparable method to sort a list of those elements
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
