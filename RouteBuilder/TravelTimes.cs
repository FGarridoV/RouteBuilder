using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class TravelTimes
    {
        //Class elements
        public int ID_period;
        public double optimalA;
        public List<double> times;


        //Constructor
        public TravelTimes(int ID_period)
        {
			this.ID_period = ID_period;
			this.times = new List<double>();
		}

        //Method 1: Add a new time
		public void add_tTime(double time)
		{
			this.times.Add(time);
		}

		//Method 2: Resturn the minimal value
		public double min_value()
		{
			times.Sort();
			return times[0];
		}

		//Method 3: Resturn the maximal value
		public double max_value()
		{
			times.Sort();
			times.Reverse();
			return times[0];
		}

		//Method 4: Dtermines the optimal binRange
		public void set_optimalA()
		{
			double dataRange = this.max_value() - this.min_value();
			int nbins = (int)Math.Floor(Math.Sqrt(dataRange));
			this.optimalA = dataRange / nbins;
		}
    }
}
