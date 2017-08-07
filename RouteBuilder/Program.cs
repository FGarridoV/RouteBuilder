﻿using System;

namespace RouteBuilder
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            //Definitions (in seconds, in meters)
            double newTripTime = 30 * 60;
            double newVisitTime = 3 * 60;
            double T = 15 * 60;
            int k = 10;
            double radious = 25;
            int version = 3;

            Console.WriteLine("Welcome to Route Builder v1.0 by TyggerSoftware Inc.\n");

            NetworkReader nr = new NetworkReader("nodes.ty","links.ty");
            Console.WriteLine("Network loaded" +"\t\t\t\t\t"+ System.DateTime.Now.ToString());

            DataBaseReader dbr = new DataBaseReader("DetBT.ty");
            Console.WriteLine("BT database loaded" +"\t\t\t\t" + System.DateTime.Now.ToString());

			DataBaseReader realDbr = new DataBaseReader("AllBT.ty");
			Console.WriteLine("Real database loaded" + "\t\t\t\t" + System.DateTime.Now.ToString());

            RealNetwork rn = new RealNetwork(nr.nodesInfo,nr.linksInfo);
            Console.WriteLine("Real network created" +"\t\t\t\t" + System.DateTime.Now.ToString());


            if (rn.BTS_id().Count>0)
            {
                Console.WriteLine("Starting a single experiment" +"\t\t\t" + System.DateTime.Now.ToString());
                Experiment exp = new Experiment(rn, dbr, realDbr, newTripTime, newVisitTime, T, k, radious, version);

                exp.scenarios[0].export_inference_vehicles();
            }

            else
            {
                //Acá se prueban todos los experiments
            }
        }
    }

}
