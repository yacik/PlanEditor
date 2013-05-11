using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanEditor.Graph
{
    public class Edge
    {
        public Edge(Vertex a, Vertex b)
        {
            VerA = a;
            VerB = b;
        }
        public Vertex VerA { get; private set; }
        public Vertex VerB { get; private set; }
    }
}
