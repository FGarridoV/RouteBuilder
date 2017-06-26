using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class Scenario
    {

        List<Vehicle> vehicles;

        public Scenario(DetectionsDB dets, Network net, double timeNewTravel, double timePeriod)
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
            add_travels_all_vehicles(timeNewTravel);
            add_times_to_nodes_and_links(net,timePeriod);
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

        public void add_travels_all_vehicles(double timeNewTravel)
        {
            foreach(Vehicle v in vehicles)
            {
                v.generate_travels(timeNewTravel);
            }
        }

        public void add_times_to_nodes_and_links(Network net, double timePeriod)
        {
            foreach(Vehicle v in vehicles)
            {
                foreach(Travel t in v.travels)
                {
                    for(int i = 0; i < t.detections.Count-1;i++)
                    {
                        if(t.detections[i].BSID==t.detections[i+1].BSID)
                        {
                            int end = i + 1;
                            for(int j = i + 2; j < t.detections.Count;j++)
                            {
                                if(t.detections[i].BSID == t.detections[j].BSID)
                                {
                                    end = j;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            double dTimeAux = t.detections[end].time - t.detections[i].time;
                            int period = (int)Math.Ceiling(t.detections[i].time / timePeriod);
                            net.nodeByID(t.detections[i].BSID).set_dwell_time_at_period(period,dTimeAux);
                            i = end-1;
                        }

                        else
                        {
                            if(net.Can_I_go_in_one_link(t.detections[i].BSID,t.detections[i + 1].BSID))
                            {
                                double tTimeAux = t.detections[i + 1].time - t.detections[i].time;
                                int period = (int)Math.Ceiling(t.detections[i].time / timePeriod);
                                net.LinkByNodesID(t.detections[i].BSID,t.detections[i+1].BSID).set_travel_time_at_period(period,tTimeAux);
                            }
                        }
                    }
                }
            }
        }



    }
}
