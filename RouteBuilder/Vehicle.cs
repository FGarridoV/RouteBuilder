using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class Vehicle
    {
        public int MAC;
        public List<Detection> allDetections;
        public List<Travel> travels;
        public List<List<int>> inferedTravels;

        public Vehicle(int MAC)
        {
            this.MAC = MAC;
            this.allDetections = new List<Detection>();
            this.travels = new List<Travel>();
            this.inferedTravels = new List<List<int>>();
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

        public void generate_travels(double timeNewTravel)
        {
            sort_Detections();
            Travel t0 = new Travel();
            t0.add_detection(allDetections[0]);

            for (int i = 0; i < allDetections.Count;i++)
            {
                if(i==0)
                {
                    Travel t = new Travel();
                    travels.Add(t);
                    travels[travels.Count-1].add_detection(allDetections[i]);
                }

                else
                {
                    if(allDetections[i].time - travels[travels.Count-1].get_last_time() < timeNewTravel)
                    {
                        travels[travels.Count - 1].add_detection(allDetections[i]);
                    }

                    else
                    {
                        Travel t = new Travel();
                        travels.Add(t);
                        travels[travels.Count - 1].add_detection(allDetections[i]);
                    }
                }
            }

            foreach(Travel t in travels)
            {
                t.generate_passingNodes();
            }
        }

        public void add_inferedTravels(Network net, RealNetwork mn, int k)
        {
            foreach(Travel t in travels)
            {
                List<int> infTravel = new List<int>();
                infTravel.Add(t.detections[0].BSID);
                for (int i = 1; i < t.detections.Count - 1; i++)
                {
                    if (infTravel[infTravel.Count-1] == t.detections[i].BSID)
                    {
                        continue;
                    }

                    else
                    {
                        if (net.Can_I_go_in_one_link(infTravel[infTravel.Count - 1], t.detections[i].BSID))
                        {
                            infTravel.Add(t.detections[i].BSID);
                        }

                        else
                        {
                            //net.set_angularCosts(t.detections[i].BSID);
                            Options paths = new Options(net,mn,infTravel[infTravel.Count - 1],t.detections[i].BSID,k);
                            //Generar set de rutas entre los nodos
                        }
                    }
                }
            }
            
        }
    }
}
