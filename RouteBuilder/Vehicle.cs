using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class Vehicle
    {
        public int MAC;
        List<Detection> allDetections;
        List<Travel> travels;

        public Vehicle(int MAC)
        {
            this.MAC = MAC;
            this.allDetections = new List<Detection>();
            this.travels = new List<Travel>();
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


        }

    }
}
