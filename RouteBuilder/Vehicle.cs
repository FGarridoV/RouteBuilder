using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class Vehicle
    {
        //Class elements
        public int MAC;
        public List<Detection> allDetections;
        public List<Trip> trips;

        //Constructor
        public Vehicle(int MAC)
        {
            this.MAC = MAC;
            this.allDetections = new List<Detection>();
            this.trips = new List<Trip>();
        }

        //Method 1: Add a new trip to this vehicle
        public void add_new_trip(Trip t)
        {
            trips.Add(t);
        }

		//Method 2: Add a new detection to this vehicle
        public void add_new_detection(Detection d)
		{
            allDetections.Add(d);
		}

        //Method 3: Sort detections by occur (time)
        public void sort_Detections()
        {
            allDetections.Sort();
        }

        //Method 4: Create all trips of a vehicle
        public void generate_trips(double timeNewtrip)
        {
            sort_Detections();

            for (int i = 0; i < allDetections.Count;i++)
            {
                if(i==0)
                {
                    Trip t = new Trip();
                    trips.Add(t);
                    trips[trips.Count-1].add_detection(allDetections[i]);
                }

                else
                {
                    if(allDetections[i].time - trips[trips.Count-1].get_last_time() <= timeNewtrip)
                    {
                        trips[trips.Count - 1].add_detection(allDetections[i]);
                    }

                    else
                    {
                        Trip t = new Trip();
                        trips.Add(t);
                        trips[trips.Count - 1].add_detection(allDetections[i]);
                    }
                }
            }

            foreach(Trip t in trips)
            {
                t.generate_passingNodes();
            }
        }

        //Method 5: Add all options to each trip
        public void add_tripOptions(Network net, int k, double T)
        {
            foreach(Trip t in trips)
            {
                t.add_sections(net,k, T);
            }
            
        }

        public  bool has_loop(List<int> pass)
        {
            for (int i = 0; i < pass.Count;i++)
            {
                for(int j = 0; j < pass.Count; j++)
                {
                    if (pass[i] == pass[j] && i != j)
                        return true; 
                }
            }
            return false;
        }

        public bool tiene_inter_loops(Vehicle virVehicle)
        {
            
            List<int> passingReal = this.trips[0].passingNodes;
            List<int> passingVirtual = virVehicle.trips[0].passingNodes;
            if (passingVirtual.Count == 1)
                return true;

            List<int> actualSection = new List<int>();
            int k = 0;
            actualSection.Add(passingVirtual[k]);

            for (int i = 1; i < passingReal.Count; i++)
            {
                if(passingReal[i] == passingVirtual[k+1])
                {
                    actualSection.Add(passingReal[i]);
                    if(has_loop(actualSection))
                    {
                        return true;
                    }

                    actualSection = new List<int>();

                    if (k <= passingVirtual.Count - 3)
                    {
                        k++;
                        //i = 0;
                        actualSection.Add(passingVirtual[k]);
                    }
                    else
                    {
                        return false;
                    }
                }

                else
                {
                    actualSection.Add(passingReal[i]);
                }
            }
            return false;
        }

        public void set_proporcion_equals(Network net)
        {
            foreach(Trip t in trips)
            {
                t.set_percSimil_Dif(net);
            }
        }

        public void set_seedCat()
        {
            foreach(Trip t in trips)
            {
                foreach(Section s in t.sections)
                {
                    s.set_speedCat();
                }
            }
        }
    }

}
