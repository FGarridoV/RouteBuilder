using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class Vehicle
    {
        //Class elements
        public int MAC;
        public List<Detection> allDetections;
        public List<Travel> travels;
        public List<List<int>> inferedTravels;

        //Constructor
        public Vehicle(int MAC)
        {
            this.MAC = MAC;
            this.allDetections = new List<Detection>();
            this.travels = new List<Travel>();
            this.inferedTravels = new List<List<int>>();
        }

        //Method 1: Add a new travel to this vehicle
        public void add_new_travel(Travel t)
        {
            travels.Add(t);
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

        //Method 4: Create all travels of a vehicle
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

        //Method 5: Add all options to each travel
        public void add_TravelOptions(Network net, int k)
        {
            foreach(Travel t in travels)
            {
                t.add_sections(net,k);
            }
            
        }
    }
}
