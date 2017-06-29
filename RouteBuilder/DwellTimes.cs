using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class DwellTimes
    {
		//Class elements
        public int ID_period;
		public List<double> times;

        //Constructor
        public DwellTimes(int ID_period)
        {
            this.ID_period = ID_period;
            this.times = new List<double>();
        }

        //Method 1: Add a new time to the list
        public void add_dTime(double time)
        {
            this.times.Add(time);
        }
    }
}
