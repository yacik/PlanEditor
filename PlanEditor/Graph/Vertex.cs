using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanEditor.Graph
{
    public class Vertex
    {
        public Vertex(int id, double x, double y)
        {
            ID = id;
            Edges = new List<Edge>();
        }

        public int ID { get; private set; }
        public double X { get; private set; }
        public double Y { get; private set; }
        public List<Edge> Edges { get; set; }
        public int Weight = 1;
    }
}
