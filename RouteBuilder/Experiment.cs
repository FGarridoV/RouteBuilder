using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class Experiment
    {
		int nBTsensor;
        public List<Scenario> scenarios;

        public Experiment(int nBTsensor, RealNetwork rn, DataBaseReader ddb)
        {
            this.nBTsensor = nBTsensor;
            //probar todas las combinaciones
        }

        public Experiment(RealNetwork rn, DataBaseReader ddb, double newTravelTime, double timeNewVisit, double T, int k)
        {
            nBTsensor = rn.BTS_id().Count;
            scenarios = new List<Scenario>();

            DetectionsDB DB = new DetectionsDB(ddb.BTData, rn.BTS_id());
            Console.WriteLine("Database filtered" +"\t\t\t"+ System.DateTime.Now.ToString());

            RealNetwork mn = rn.real_to_model();
			mn.set_DijkstraData(rn);
			Network modelNet = new Network(mn);
			Console.WriteLine("Model network created" +"\t\t\t"+ System.DateTime.Now.ToString());

			Scenario sc = new Scenario(DB,T);
			Console.WriteLine("New scenario created" + "\t\t\t" + System.DateTime.Now.ToString());
			sc.add_travels_all_vehicles(newTravelTime);
			Console.WriteLine("Vehicle trips assigned" + "\t\t\t" + System.DateTime.Now.ToString());
			sc.add_times_to_nodes_and_links(modelNet, T, timeNewVisit);
			Console.WriteLine("Dwell and travel times loaded" + "\t\t" + System.DateTime.Now.ToString());
			sc.add_options(modelNet, k, T);
			Console.WriteLine("Sections of all vehicle determinated" + "\t" +System.DateTime.Now.ToString());
            sc.apply_methodology();
			Console.WriteLine("Bayesian probabilities calculated" + "\t" + System.DateTime.Now.ToString());
			sc.assing_routes();
			Console.WriteLine("All vehicle routes infered" + "\t\t" + System.DateTime.Now.ToString());
            scenarios.Add(sc);
            //sc.export_inference_vehicles(this);
            Console.WriteLine("\a");
        }

        public string get_realRoute(int veh_mac)
        {
            foreach(Vehicle v in this.scenarios[0].vehicles)
            {
                if(v.MAC == veh_mac)
                {
                    return v.trips[0].routes[0].export_route();
                }
            }
            return null;
        }

        public Route export_in_route(int veh_mac)
        {
			foreach (Vehicle v in this.scenarios[0].vehicles)
			{
				if (v.MAC == veh_mac)
				{
					return v.trips[0].routes[0];
				}
			}
			return null;
        }
    }
}