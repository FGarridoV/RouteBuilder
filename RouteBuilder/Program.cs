using System;

namespace RouteBuilder
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Route Builder by Tygger Inc.");

            NetworkReader nr = new NetworkReader("nodes.txt","links.txt");
            Console.WriteLine("Network charged");

            DataBaseReader dbr = new DataBaseReader("db.txt");
            Console.WriteLine("Data base charged");

            RealNetwork rn = new RealNetwork(nr.nodesInfo,nr.linksInfo);
            Console.WriteLine("Real network created");

            RealNetwork mn = rn.real_to_model();
            mn.set_DijkstraData(rn);
            Network modelNet = new Network(mn);
            Console.WriteLine("Model network created");


            //Crear red de tipo NETWORK para trabajar

            //Asignar tiempos de permanencia a los nodos

            //Asignar tiempos de viaje a los arcos

            //Para cada par de nodos calcular el set de rutas posibles quizás antes

            //Para cdada dato determinar P

            //generar ruta

            //convertir a red real

            //mostrar


        }
    }
}
