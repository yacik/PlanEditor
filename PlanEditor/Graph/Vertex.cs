using System.Collections.Generic;
using PlanEditor.Entities;

namespace PlanEditor.Graph
{
    public class Vertex
    {
        public Vertex(Place place)
        {
            //ID = id;
            Edges = new List<Edge>();
            Place = place;
        }

        //public int ID { get; private set; }
        //public double X { get; private set; }
        //public double Y { get; private set; }
        public List<Edge> Edges { get; set; }
        public Place Place { get; private set; }
        //public int Weight = 1;
    }
}
