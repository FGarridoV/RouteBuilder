using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class Scenario
    {

        List<Vehicle> vehicles;

        public Scenario(DetectionsDB dets)
        {
            vehicles = new List<Vehicle>();

            foreach (Detection d in dets.detections)
            {
                if (new_MAC(d.MAC))
                {
                    Vehicle aux = new Vehicle(d.MAC);
                    aux.add_new_detection(d);
                    vehicles.Add(aux);
                }
                else
                {
                    add_detection_by_mac(d);
                }
            }
        }

        public bool new_MAC(int mac)
        {
            foreach(Vehicle v in vehicles)
            {
                if(v.MAC == mac)
                {
                    return false;
                }
            }
            return true;
        }

        public void add_detection_by_mac(Detection d)
        {
            foreach(Vehicle v in vehicles)
            {
                if(d.MAC == v.MAC)
                {
                    v.add_new_detection(d);
                }
            }
        }



    }
}
