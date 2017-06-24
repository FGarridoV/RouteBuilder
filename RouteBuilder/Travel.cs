using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class Travel
    {
		public List<Detection> detections;

        public Travel()
        {
            detections = new List<Detection>(); 
        }

        public void add_detection(Detection d)
        {
            detections.Add(d);
        }

        public double get_last_time()
        {
            return detections[detections.Count - 1].time;
        }
    }
}
