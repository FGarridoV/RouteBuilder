using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class TravelTimes
    {
        public int ID_period;
        public List<double> times;

        public TravelTimes(int ID_period)
        {
			this.ID_period = ID_period;
			this.times = new List<double>();
		}

		public void add_tTime(double time)
		{
			this.times.Add(time);
		}
    }
}
