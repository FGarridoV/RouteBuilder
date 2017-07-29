using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class DwellTimes
    {
        //Class elements
        public int ID_period;
        public double optimalA;
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

        //Method 4: Determines the optimal binRange
        public void set_optimalA()
        {
            double dataRange = this.max_value() - this.min_value();
            int nbins = (int)Math.Floor(Math.Sqrt(dataRange));
            this.optimalA = dataRange / nbins;

            if(Math.Abs(dataRange) < 0.0000001)
            {
                this.optimalA = -1;
            }
        }

    }

}
