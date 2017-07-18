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
    }
}
