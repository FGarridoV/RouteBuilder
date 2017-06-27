using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class Travel
    {
        public List<Detection> detections;
        public List<int> passingNodes;

        public Travel()
        {
            detections = new List<Detection>();
            passingNodes = new List<int>();
        }

        public void add_detection(Detection d)
        {
            detections.Add(d);
        }

        public double get_last_time()
        {
            return detections[detections.Count - 1].time;
        }

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
