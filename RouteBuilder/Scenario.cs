using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class Scenario
    {
        //Class elements
        List<Vehicle> vehicles;
        //Network network;

        //Constructor
        public Scenario(DetectionsDB dets)
        {
            vehicles = new List<Vehicle>();

			int i = 0;
			int j = 10;
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
                if (i == (int)((dets.detections.Count - 1) * j / 100))
				{
					Console.WriteLine("Creating vehicle entities ... " + j + "%\t" + System.DateTime.Now.ToString());
					j += 10;
				}
				i++;
            }
            Console.WriteLine(vehicles.Count + " Vehicles created\t\t\t" + System.DateTime.Now.ToString());
        }

        //Method 1: Determine if is it a new mac
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

        //Method 2: Add a detection by MAC
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

        //Method 3: Add travels to all vehicle
        public void add_travels_all_vehicles(double NewTripTime)
        {
            foreach(Vehicle v in vehicles)
            {
                v.generate_trips(NewTripTime);
            }
        }

		//Method 4: Add options to all vehicles
		public void add_options(Network net, int k, double T)
		{
			foreach (Vehicle v in vehicles)
			{
                v.add_tripOptions(net,k, T);
			}
		}

        //Method 5: Add times to the network
        public void add_times_to_nodes_and_links(Network net, double T, double timeNewVisit)
        {
            foreach(Vehicle v in vehicles)
            {
                foreach(Trip t in v.trips)
                {
                    for(int i = 0; i < t.detections.Count-1;i++)
                    {
                        if(t.detections[i].BSID==t.detections[i+1].BSID)
                        {
                            int end = i + 1;
                            for(int j = i + 2; j < t.detections.Count;j++)
                            {
                                if(t.detections[i].BSID == t.detections[j].BSID && t.detections[j].time - t.detections[j-1].time < timeNewVisit)
                                {
                                    end = j;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            double dTimeAux = t.detections[end].time - t.detections[i].time;
                            int period = (int)Math.Ceiling(t.detections[i].time / T);
                            net.nodeByID(t.detections[i].BSID).set_dwell_time_at_period(period,dTimeAux);
                            i = end-1;
                        }

                        else
                        {
                            if(net.Can_I_go_in_one_link(t.detections[i].BSID,t.detections[i + 1].BSID))
                            {
                                double tTimeAux = t.detections[i + 1].time - t.detections[i].time;
                                int period = (int)Math.Ceiling(t.detections[i].time / T);
                                net.LinkByNodesID(t.detections[i].BSID,t.detections[i+1].BSID).set_travel_time_at_period(period,tTimeAux);
                            }
                        }
                    }
                }
            }
        }
    }
}
