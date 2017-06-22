using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class Vehicle
    {
        public int MAC;
        List<Detection> allDetections;
        List<Travel> travels;

        public Vehicle(int MAC)
        {
            this.MAC = MAC;
            this.allDetections = new List<Detection>();
            this.travels = new List<Travel>();
        }

        public void add_new_travel(Travel t)
        {
            travels.Add(t);
        }

		public void add_new_detection(Detection d)
		{
            allDetections.Add(d);
		}

        public void sort_Detections()
        {
            allDetections.Sort();
        }
    }
}
