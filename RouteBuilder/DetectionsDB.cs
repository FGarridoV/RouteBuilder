using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class DetectionsDB
    {
        //Class elements
        public List<Detection> detections;

        //Constructor
        public DetectionsDB(List<double[]> BTData)
        {
            detections = new List<Detection>();

            foreach(double[] i in BTData)
            {
                Detection aux = new Detection((int)i[0], (int)i[1], i[2]);
                detections.Add(aux);
            }
        }

        public DetectionsDB(List<double[]> BTData, List<int> BTS)
        {
			detections = new List<Detection>();

            foreach (double[] i in BTData)
            {
                if (BTS.Contains((int)i[0]))
                {
                    Detection aux = new Detection((int)i[0], (int)i[1], i[2]);
                    detections.Add(aux);
                }
            }
        }

    }

}
