using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class Scenario
    {
        //Class elements
        public Network network;
        public List<Vehicle> vehicles;
        public double T;
        public double newTripTime;
        public double newVisitTime;
        public int K;
        public List<Vehicle> rVehicles;

        //Constructor
        public Scenario(Network net, DetectionsDB dets, DetectionsDB rDets, double T, double newTripTime, double newVisitTime, int K)
        {
            this.T = T;
            this.newTripTime = newTripTime;
            this.newVisitTime = newVisitTime;
            this.K = K;

            network = new Network(net);

            vehicles = new List<Vehicle>();
            create_vehicles(dets, vehicles);

            rVehicles = new List<Vehicle>();
            create_vehicles(rDets, rVehicles);

            Console.WriteLine("Vehicles created: " + vehicles.Count + "\t\t\t\t" + System.DateTime.Now.ToString());
            Console.WriteLine("Real vehicles: " + rVehicles.Count + "\t\t\t\t" + System.DateTime.Now.ToString());
        }

        public void create_vehicles(DetectionsDB dets, List<Vehicle> vehicles)
        {
            int i = 0;
            int j = 5;
            Console.WriteLine("\t\t\t0%\t50%\t100%");
            Console.Write("Creating vehicles...\t");
            foreach (Detection d in dets.detections)
            {
                if (new_MAC(d.MAC,vehicles))
                {
                    Vehicle aux = new Vehicle(d.MAC);
                    aux.add_new_detection(d);
                    vehicles.Add(aux);
                }
                else
                {
                    add_detection_by_mac(d, vehicles);
                }
                if (i == (int)((dets.detections.Count - 1) * j / 100))
                {
                    if (j == 100)
                        Console.WriteLine("\u2588\t" + System.DateTime.Now.ToString());

                    else
                    {
                        Console.Write("\u2588");
                        j += 5;
                    }
                }
                i++;
            }
        }

        //Method 1: Determine if is it a new mac
        public bool new_MAC(int mac, List<Vehicle> vehicles)
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
        public void add_detection_by_mac(Detection d, List<Vehicle> vehicles)
        {
            foreach (Vehicle v in vehicles)
            {
                if (d.MAC == v.MAC)
                {
                    v.add_new_detection(d);
                }
            }
        }

        //Method 3: Add travels to all vehicle
        public void add_travels_all_vehicles()
        {
            foreach(Vehicle v in vehicles)
            {
                v.generate_trips(newTripTime);
            }
        }

		//Method 3a: Add travels to all real vehicle
		public void add_travels_all_rVehicles()
		{
			foreach (Vehicle v in rVehicles)
			{
				v.generate_trips(newTripTime);
			}
		}

		//Method 4: Add options to all vehicles
		public void add_options()
		{
			foreach (Vehicle v in vehicles)
			{
                v.add_tripOptions(network ,K, T);
			}
		}

		//Method 4a: Add options to all vehicles
		public void add_rOptions()
		{
			foreach (Vehicle v in rVehicles)
			{
				v.add_tripOptions(network, 1, T);
			}
		}

        //Method 5: Add times to the network
        public void add_times_to_nodes_and_links()
        {
            foreach(Vehicle v in vehicles)
            {
                foreach(Trip t in v.trips)
                {
                    for(int i = 0; i < t.detections.Count-1;i++)
                    {
                        if(t.detections[i].BSID==t.detections[i+1].BSID)
                        {
                            if(t.detections[i+1].time - t.detections[i].time>newVisitTime)
                            {
                                continue;
                            }
                            int end = i + 1;
                            for(int j = i + 2; j < t.detections.Count;j++)
                            {
                                if(t.detections[i].BSID == t.detections[j].BSID && t.detections[j].time - t.detections[j-1].time <= newVisitTime)
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
                            network.nodeByID(t.detections[i].BSID).set_dwell_time_at_period(period,dTimeAux);
                            i = end-1;
                        }

                        else
                        {
                            if(network.Can_I_go_in_one_link(t.detections[i].BSID,t.detections[i + 1].BSID))
                            {
                                double tTimeAux = t.detections[i + 1].time - t.detections[i].time;
                                int period = (int)Math.Ceiling(t.detections[i].time / T);
                                network.LinkByNodesID(t.detections[i].BSID,t.detections[i+1].BSID).set_travel_time_at_period(period,tTimeAux);
                            }
                        }
                    }
                }
            }
			network.set_BinsRange();
        }

        public void NumberVehicles()
        {
            int total_inferences = 0;
            int totalSections = 0;
            int sectionToInference = 0;
            foreach (Vehicle v in vehicles)
            {
                totalSections = 0;
                sectionToInference = 0;
                foreach (Trip t in v.trips)
                {
                    totalSections = t.sections.Count;
                    foreach (Section s in t.sections)
                    {
                        if (s.paths.Count > 1)
                        {
                            sectionToInference++;
                            total_inferences++;
                        }
                    }
                }

				if (v.MAC == 364)
                {
                    Console.WriteLine("The vehicle " + v.MAC + " has " + sectionToInference + "/" + totalSections + " sections  to make inference");

                    Console.WriteLine("The detections are:");
                    foreach (Detection d in v.allDetections)
                    {
                        Console.WriteLine(d.BSID + " --> " + d.time);
                    }
                    Console.WriteLine("The sections are:");
                    foreach (Trip t in v.trips)
                    {
                        foreach (Section s in t.sections)
                        {
                            Console.WriteLine("----------Section---------- Period: " + s.period);
                            Console.WriteLine("start time: " + s.timeStart);
                            Console.WriteLine("end time: " + s.timeEnd);
                            foreach (Path p in s.paths)
                            {
                                Console.WriteLine("-----Paths-----");
                                double[] a = get_DB_Full_and_v(p, 23);
                                Console.WriteLine("existen " + a[0] + "a una vel " + a[1]*3.6);
                                foreach (int n in p.nodesIDs)
                                {
                                    Console.WriteLine(n);
                                }

                            }

                        }
                    }
                }
            }

        }

        public double[] get_DB_node_and_v(Path p,int node_ID, int period)
        {
            int total = 0;
            double sumSpeeds = 0;
            foreach(Vehicle v in vehicles)
            {
                foreach(Trip t in v.trips)
                {
                    for (int i = 0; i < t.passingNodes.Count; i++)
                    {
                        if (period == (int)Math.Ceiling(t.enterTimePassingNodes[i] / T) && t.passingNodes.Count>i+2)
                        {
                            if (t.passingNodes[i] == p.nodesIDs[0] && t.passingNodes[i+1]==node_ID && t.passingNodes[i+2]==p.nodesIDs[p.nodesIDs.Count-1])
                            {
                                total++;
                                sumSpeeds += p.distance / (t.enterTimePassingNodes[i+2] - t.exitTimePassingNodes[i]);
                            }
                        }
                    }
                }
            }
			if (total == 0)
				return new double[] { 0, 0 };

			double[] data = new double[] { total, sumSpeeds / total };
			return data;
        }

        public double[] get_DB_Full_and_v(Path p, int period)
        {
            int total = 0;
            double sumSpeeds = 0;
            int ver = 0;
            int start = 0;
            int end = 0;

            foreach(Vehicle v in vehicles)
            {
                foreach(Trip t in v.trips)
                {
                    for (int i = 0; i < t.passingNodes.Count; i++)
                    {
                        if (period == (int)Math.Ceiling(t.enterTimePassingNodes[i] / T))
                        {
                            if(t.passingNodes[i]==p.nodesIDs[0])
                            {
                                ver++;
                                start = i;
                                for (int j = i + 1; j < t.passingNodes.Count && j-i<p.nodesIDs.Count;j++)
                                {
                                    if (t.passingNodes[j] == p.nodesIDs[j - i])
                                    {
                                        ver++;
                                        end = j;
                                    }
                                }
                                if (ver == p.nodesIDs.Count)
                                {
                                    total++;
                                    sumSpeeds += p.distance / (t.enterTimePassingNodes[end] - t.exitTimePassingNodes[start]);
                                }
                                ver = 0;
                            }
                        }
                    }                    
                }
            }
            if (total == 0)
                return new double[] { 0, 0};
                
			double[] data = new double[] { total, sumSpeeds / total };
			return data;
        }

        public double[] get_DB_k_and_v(Path p, int r, int period)
        {
            int total = 0;
            double sumSpeeds = 0;

            foreach(Vehicle v in vehicles)
            {
                int detections = 0;
                int start;
                int end;
                foreach(Trip t in v.trips)
                {
                    for (int i = 0; i < t.passingNodes.Count;i++)
                    {
                        if (period == (int)Math.Ceiling(t.enterTimePassingNodes[i]/ T))
                        {
                            if (t.passingNodes[i] == p.nodesIDs[0])
                            {
                                start = i;
                                end = 0;
                                detections = 0;
                                for (int j = i + 1; j < t.passingNodes.Count; j++)
                                {
                                    if (t.passingNodes[j] == p.nodesIDs[p.nodesIDs.Count - 1])
                                    {
                                        end = j;
                                        break;
                                    }
                                }
                                if (end == 0)
                                    break;
                                if (end == start + 1)
                                    continue;

                                int startPoint = 1;
                                for (int s = start + 1; s < end; s++)
                                {
                                    for (int q = startPoint; q < p.nodesIDs.Count - 1; q++)
                                    {
                                        if (t.passingNodes[s] == p.nodesIDs[q])
                                        {
                                            detections++;
                                            startPoint = q + 1;
                                            break;
                                        }
                                    }
                                }
                                if (detections == r)
                                {
                                    total++;
                                    sumSpeeds += p.distance/(t.enterTimePassingNodes[end]-t.exitTimePassingNodes[start]);
                                }
                            }
                        }
                    }
                }
            }

            double[] data = new double[] { total, sumSpeeds / total };
            return data;
        }

        public void apply_methodology(int version)
        {
            foreach(Vehicle v in vehicles)
            {
                foreach(Trip t in v.trips)
                {
                    foreach(Section s in t.sections)
                    {
                        if (s.paths.Count >= 2)
                        {
                            s.apply_BayesianInference(this, version);
                        }

                        else if (s.paths.Count == 1)
                            s.apply_ObiouslyInference();
                    }
                }
            }
        }

		public void apply_directions()
		{
			foreach (Vehicle v in rVehicles)
			{
				foreach (Trip t in v.trips)
				{
					foreach (Section s in t.sections)
					{
							s.apply_ObiouslyInference();
					}
				}
			}
		}

        public void assing_routes()
        {
            foreach (Vehicle v in vehicles)
            {
                foreach (Trip t in v.trips)
                {
                    t.create_Routes(v.MAC);
                }
            }
        }

		public void assing_rRoutes()
		{
			foreach (Vehicle v in rVehicles)
			{
				foreach (Trip t in v.trips)
				{
					t.create_Routes(v.MAC);
				}
			}
		}

        public double[] min_max_period_ID()
        {
            double minPeriodID = double.PositiveInfinity;
            double maxPeriodID = 0;

            foreach(Link l in network.links)
            {
                foreach(TravelTimes tt in l.tTimes)
                {
                    if (tt.ID_period > maxPeriodID)
                        maxPeriodID = tt.ID_period;
                    if (tt.ID_period < minPeriodID)
                        minPeriodID = tt.ID_period;
                }
            }
			foreach (Node n in network.nodes)
			{
				foreach (DwellTimes dt in n.dTimes)
				{
					if (dt.ID_period > maxPeriodID)
						maxPeriodID = dt.ID_period;
					if (dt.ID_period < minPeriodID)
						minPeriodID = dt.ID_period;
				}
			}

            return new double[] { minPeriodID, maxPeriodID };
        }

        public void times_corrector(double radious)
        {
            double[] minMax = min_max_period_ID();
            int count = (int)(minMax[1] - minMax[0]+1);

            foreach(Link l in network.links)
            {
                if(l.tTimes.Count<count)
                {
                    List<int> missingPeriods = new List<int>(periods_dont_present(l, minMax));
                    foreach(int period in missingPeriods)
                    {
                        TravelTimes ttAux = new TravelTimes(period);

                        double vProm = 0;
                        double cProm = 0;
                        int counts = 0;
                        foreach(Link l_v in l.tailNode.innerLinks)
                        {
                            if (l_v.tt_at_specific_period(period)!=null)
                            {
                                vProm += l_v.get_Vprom_at_period(period);
                                cProm += l_v.get_Count_at_period(period);
                                counts++;
                            }
                        }
                        foreach (Link l_v in l.headNode.outerLinks)
						{
                            if (l_v.tt_at_specific_period(period)!=null)
                            {
                                vProm += l_v.get_Vprom_at_period(period);
                                cProm += l_v.get_Count_at_period(period);
                                counts++;
                            }
						}
                        vProm = vProm / counts;
                        cProm = cProm / counts;

                        if(Math.Abs(vProm) < 0.00001 || counts == 0)
                        {
                            vProm = 5;
                            cProm = 1;
                        }
                        double tProm = l.distanceCost / vProm;
                        double tMax = tProm * 1.5;
                            
                        ttAux.add_tTime(tProm);

                        for (int i = 0; i < cProm;i++)
                        {
                            ttAux.add_tTime(tMax);
                        }
                        ttAux.set_optimalA();
                        l.add_travelTimes(ttAux);
                    }
                }
            }

			foreach(Node n in network.nodes)
			{
				if (n.dTimes.Count < count)
				{
					List<int> missingPeriods = new List<int>(periods_dont_present(n, minMax));
					foreach (int period in missingPeriods)
					{
                        DwellTimes dtAux = new DwellTimes(period);

						double vProm = 0;
						double cProm = 0;
                        foreach (Link l_in in n.innerLinks)
						{
                            vProm += l_in.tailNode.get_Vprom_at_period(period,radious);
                            cProm += l_in.tailNode.get_Count_at_period(period);
						}
                        foreach (Link l_out in n.outerLinks)
						{
                            vProm += l_out.headNode.get_Vprom_at_period(period,radious);
                            cProm += l_out.headNode.get_Count_at_period(period);
						}
                        vProm = vProm / (n.innerLinks.Count + n.outerLinks.Count);
						cProm = cProm / (n.innerLinks.Count + n.outerLinks.Count);
                        double tProm = 2*radious / vProm;
						double tMax = tProm * 1.5;

						dtAux.add_dTime(tProm);

						for (int i = 0; i < cProm; i++)
						{
							dtAux.add_dTime(tMax);
						}
						dtAux.set_optimalA();
                        n.add_dwellTimes(dtAux);
					}
				}
			}
        }

        public List<int> periods_dont_present(Link l, double[] minMax)
        {
            List<int> all = new List<int>();
            for (int i = (int)minMax[0]; i <= (int)minMax[1];i++)
            {
                all.Add(i);
            }

            foreach(TravelTimes tt in l.tTimes)
            {
                all.Remove(tt.ID_period);
            }

            return all;
        }

		public List<int> periods_dont_present(Node n, double[] minMax)
		{
			List<int> all = new List<int>();
			for (int i = (int)minMax[0]; i <= (int)minMax[1]; i++)
			{
				all.Add(i);
			}

			foreach (DwellTimes dt in n.dTimes)
			{
				all.Remove(dt.ID_period);
			}

			return all;
		}

		public string get_realRoute(int veh_mac)
		{
			foreach (Vehicle v in this.rVehicles)
			{
				if (v.MAC == veh_mac)
				{
					return v.trips[0].routes[0].export_route();
				}
			}
			return null;
		}

		public Route export_in_route(int veh_mac)
		{
			foreach (Vehicle v in this.rVehicles)
			{
				if (v.MAC == veh_mac)
				{
					return v.trips[0].routes[0];
				}
			}
			return null;
		}

        public void export_inference_vehicles()
        {
            int exitos = 0;
            int fracasos = 0;
            foreach(Vehicle v in vehicles)
            {
                foreach(Trip t in v.trips)
                {
                    if(t.routes.Count>1)
                    {
                        Console.WriteLine("---------- " + v.MAC + " ----------");
                        foreach(Route r in t.routes)
                        {
                            Console.WriteLine("Route: " + r.export_route());
                        }
                        Console.WriteLine(get_realRoute(v.MAC));
                        if (Route.Comparer(t.get_mostProbably(), export_in_route(v.MAC)))
                        {
                            Console.WriteLine("SUCCESS");
                            exitos++;
                        }
                        else
                        {
                            Console.WriteLine("FAIL");
                            fracasos++;
                        }
                        Console.WriteLine("----------" + "---" + "----------");
                    }
                }
            }
            Console.WriteLine("Program ended, RATIO: " + (exitos/(double)(exitos + fracasos)*100) + "%   Total is: "+(exitos + fracasos));
        }
    }

}
