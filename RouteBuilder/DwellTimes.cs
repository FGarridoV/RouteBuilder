using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class DwellTimes
    {
		public int ID_period;
		public List<double> times;

        public DwellTimes(int ID_period)
        {
            this.ID_period = ID_period;
            this.times = new List<double>();
        }

        public void add_dTime(double time)
        {
            this.times.Add(time);
        }
    }
}
