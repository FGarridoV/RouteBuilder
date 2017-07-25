﻿using System;

namespace RouteBuilder
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            //Definitions (in seconds)
            double newTripTime = 30 * 60;
            double newVisitTime = 3 * 60;
            double T = 20 * 60;
            int k = 2;

            Console.WriteLine("Welcome to Route Builder v0.0 by TyggerSoftware Inc.");

            NetworkReader nr = new NetworkReader("nodes.txt","links.txt");
            Console.WriteLine("Network loaded" +"\t\t\t\t"+ System.DateTime.Now.ToString());

            DataBaseReader dbr = new DataBaseReader("DetBT.txt");
            Console.WriteLine("Database loaded" +"\t\t\t\t" + System.DateTime.Now.ToString());

            RealNetwork rn = new RealNetwork(nr.nodesInfo,nr.linksInfo);
            Console.WriteLine("Real network created" +"\t\t\t" + System.DateTime.Now.ToString());

			/////BORRAR//////
			DataBaseReader dbrREAL = new DataBaseReader("AllBT.txt");
			Console.WriteLine("Database REAL loaded" + "\t\t\t\t" + System.DateTime.Now.ToString());
			RealNetwork rnREAL = new RealNetwork(nr.nodesInfo, nr.linksInfo);
			Console.WriteLine("Real network created" + "\t\t\t" + System.DateTime.Now.ToString());
            /////////////////

            if (rn.BTS_id().Count>0)
            {
                Console.WriteLine("Starting a single experiment" +"\t\t" + System.DateTime.Now.ToString());
                Experiment exp = new Experiment(rn, dbr, newTripTime, newVisitTime, T, k);
                ////BORRAR/////
				Console.WriteLine("Starting a single experiment" + "\t\t" + System.DateTime.Now.ToString());
				Experiment expREAL = new Experiment(rnREAL, dbrREAL, newTripTime, newVisitTime, T, k);
                ///////////////
                /// 
                ///

                exp.scenarios[0].export_inference_vehicles(expREAL);
            }

            else
            {
                //Acá se prueban todos los expériments
            }

			//TRUCHERIA
			

        }
    }
}
