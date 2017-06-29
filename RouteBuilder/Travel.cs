using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class Travel
    {
        //Class elements
        public List<Detection> detections;
        public List<int> passingNodes;

        //Constructor
        public Travel()
        {
            detections = new List<Detection>();
            passingNodes = new List<int>();
        }

        //Method 1: Add a detection
        public void add_detection(Detection d)
        {
            detections.Add(d);
        }

        //Method 2: Obtain the time of the last detection
        public double get_last_time()
        {
            return detections[detections.Count - 1].time;
        }

        //Method 3: Generate the passing nodes
        public void generate_passingNodes()
        {
            passingNodes.Add(detections[0].BSID);

            for (int i = 1; i < detections.Count;i++)
            {
                if(passingNodes[passingNodes.Count-1]==detections[i].BSID)
                {
                    continue;
                }
                else
                {
                    passingNodes.Add(detections[i].BSID);
                }
            }
        }
    }
}
