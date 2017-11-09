using System;

namespace RouteBuilder
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            //Definitions (in seconds, in meters)
            double newTripTime = 90 * 60;
            double newVisitTime = 3 * 60;
            double T = 15 * 60;
            int k = 10;
            double radious = 25;
            int version = 2;

            Console.WriteLine("Welcome to Route Builder v1.0 by TyggerSoftware Inc.\n");

            Console.WriteLine("Starting the program" + "\t\t\t\t" + System.DateTime.Now.ToString());
            NetworkReader nr = new NetworkReader("nodes.ty","links.ty");
            Console.WriteLine("Network loaded" +"\t\t\t\t\t"+ System.DateTime.Now.ToString());

            DataBaseReader dbr = new DataBaseReader("DetBT.ty");
            Console.WriteLine("BT database loaded" +"\t\t\t\t" + System.DateTime.Now.ToString());
            Console.WriteLine("");

			DataBaseReader realDbr = new DataBaseReader("AllBT.ty");
			Console.WriteLine("Real database loaded" + "\t\t\t\t" + System.DateTime.Now.ToString());
            Console.WriteLine("");

            RealNetwork rn = new RealNetwork(nr.nodesInfo,nr.linksInfo);
            Console.WriteLine("Real network created" +"\t\t\t\t" + System.DateTime.Now.ToString());


            if (rn.BTS_id().Count>0)
            {
                Console.WriteLine("Starting a single experiment" +"\t\t\t" + System.DateTime.Now.ToString());
                Experiment exp = new Experiment(rn, dbr, realDbr, newTripTime, newVisitTime, T, k, radious, version);
                Console.WriteLine("\a\a\a\a\a\a\a\a\a\a\a\a\a\a\a\a\a\a");                                          
                //exp.scenarios[0].export_inference_vehicles();
            }

            else
            {
                Console.WriteLine("Starting a set of experiments" + "\t\t\t" + System.DateTime.Now.ToString());
                //Acá se prueban todos los experiments
            }
        }
    }

}
