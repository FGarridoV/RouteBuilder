using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class Experiment
    {
		int nBTsensor;
        List<Scenario> scenarios;

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

			Scenario sc = new Scenario(DB);
			Console.WriteLine("New scenario created" + "\t\t\t" + System.DateTime.Now.ToString());
			sc.add_travels_all_vehicles(newTravelTime);
			Console.WriteLine("Vehicle trips assigned" + "\t\t\t" + System.DateTime.Now.ToString());
			sc.add_times_to_nodes_and_links(modelNet, T, timeNewVisit);
			Console.WriteLine("Dwell and travel times loaded" + "\t\t" + System.DateTime.Now.ToString());
			sc.add_options(modelNet, k, T);
			Console.WriteLine("Sections of all vehicle determinated" + "\t" +System.DateTime.Now.ToString());
            //modelNet.export_data();
            Console.WriteLine("\a");


        }
    }
}
