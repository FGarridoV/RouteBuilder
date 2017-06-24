﻿using System;
using System.Collections.Generic;
namespace RouteBuilder
{
    public class Link
    {
		public int ID;
		public Node tailNode;
		public Node headNode;
		public double distanceCost;
		public double edgesCost;
        public double angularCost;
        public double mainCost;
        public List<TravelTimes> tTimes;


        public Link(RealLink l)
        {
            this.ID = l.ID;
            this.distanceCost = l.dijkstraDistance;
            this.edgesCost = l.dijkstraNodes;
        }

        public void add_tailNode(Node n)
        {
            this.tailNode = n;
        }

		public void add_headNode(Node n)
		{
            this.headNode = n;
		}


    }
}
