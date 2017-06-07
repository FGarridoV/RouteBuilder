using System;
using System.IO;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class NetworkReader
    {
        public int nNodes; 
        public int nLinks;
        public List<int[]> nodesInfo;
        public List<int[]> linksInfo;

        public NetworkReader(string nodeFileName, string linkFileName)
        {
            try
            {
                StreamReader sr1 = new StreamReader(nodeFileName);
                StreamReader sr2 = new StreamReader(linkFileName);
                this.nNodes = int.Parse(sr1.ReadLine());
                this.nLinks = int.Parse(sr2.ReadLine());
                this.nodesInfo = new List<int[]>(nodeStr_to_list(sr1.ReadToEnd()));
                this.linksInfo = new List<int[]>(LinkStr_to_list(sr2.ReadToEnd()));
                sr1.Close();
                sr2.Close();
            }
            catch
            {
                Console.WriteLine("Error 0001: reading network file");
                System.Environment.Exit(0);
            }
            
        }

        public List<int[]> nodeStr_to_list(string str)
        {
            string[] nodesStr = str.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            List<int[]> nodesData = new List<int[]>();

			foreach (string node in nodesStr)
			{
				string[] auxStr = node.Split(',');
                int[] aux = new int[] { int.Parse(auxStr[0]), int.Parse(auxStr[1]), int.Parse(auxStr[2]), int.Parse(auxStr[3])};
				nodesData.Add(aux);
			}

            return nodesData;
        }

		public List<int[]> LinkStr_to_list(string str)
		{
            string[] linksStr = str.Split(new string[] { "\r\n", "\n" },StringSplitOptions.RemoveEmptyEntries);
			List<int[]> linksData = new List<int[]>();

			foreach (string link in linksStr)
			{
                string[] auxStr = link.Split(',');
                int[] aux = new int[] {int.Parse(auxStr[0]),int.Parse(auxStr[1]),int.Parse(auxStr[2]) };
                linksData.Add(aux);
			}
			return linksData;
		}
    }
}
