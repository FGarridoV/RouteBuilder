using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class DetectionsDB
    {

        public List<Detection> detections;

        public DetectionsDB(List<double[]> BTData)
        {
            detections = new List<Detection>();

            foreach(double[] i in BTData)
            {
                Detection aux = new Detection((int)i[0], (int)i[1], i[2]);
                detections.Add(aux);
            }
        }

    }
}
