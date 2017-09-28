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

        public Experiment(RealNetwork rn, DataBaseReader ddb, DataBaseReader realDdb, double newTripTime, double newVisitTime, double T, int K, double radious, int version)
        {
            nBTsensor = rn.BTS_id().Count;
            scenarios = new List<Scenario>();

            DetectionsDB DB = new DetectionsDB(ddb.BTData, rn.BTS_id());
            Console.WriteLine("BT database filtered" +"\t\t\t\t"+ System.DateTime.Now.ToString());

			DetectionsDB rDB = new DetectionsDB(realDdb.BTData, rn.BTS_id());
			Console.WriteLine("Real database filtered" + "\t\t\t\t" + System.DateTime.Now.ToString());

            RealNetwork mn = rn.real_to_model();
			mn.set_DijkstraData(rn);
			Network modelNet = new Network(mn);
			Console.WriteLine("Model network created" +"\t\t\t\t"+ System.DateTime.Now.ToString());

            Scenario sc = new Scenario(modelNet,DB,rDB,T,newTripTime,newVisitTime,K);
            Console.WriteLine("New scenario created" + "\t\t\t\t" + System.DateTime.Now.ToString());
			
            sc.add_travels_all_vehicles();
            sc.add_travels_all_rVehicles();
			Console.WriteLine("Vehicle trips assigned" + "\t\t\t\t" + System.DateTime.Now.ToString());
			
            sc.add_times_to_nodes_and_links();
			Console.WriteLine("Dwell and travel times loaded" + "\t\t\t" + System.DateTime.Now.ToString());

            sc.times_corrector(radious);
            Console.WriteLine("Dwell and travel times corrected" + "\t\t" + System.DateTime.Now.ToString());
			
            sc.add_options();
            sc.add_rOptions();
			Console.WriteLine("Sections of all vehicle determinated" + "\t\t" +System.DateTime.Now.ToString());

            sc.apply_methodology(version);
            sc.apply_directions();
			Console.WriteLine("Bayesian probabilities calculated" + "\t\t" + System.DateTime.Now.ToString());
			
            sc.assing_routes();
            sc.assing_rRoutes();
            sc.trimrRoutes();
			Console.WriteLine("All vehicle routes infered" + "\t\t\t" + System.DateTime.Now.ToString());

            sc.set_choices();
			Console.WriteLine("Choices assigned to vehicles" + "\t\t\t" + System.DateTime.Now.ToString());

            sc.calculate_statistics();
            sc.estimating_flows();
            sc.print_statistics();
            sc.write_statistics();
            sc.export_inference_vehicles();
            sc.correctr();

			scenarios.Add(sc);
        }
    }

}