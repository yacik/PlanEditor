using PlanEditor.Entities;

namespace PlanEditor.Graph
{
    public class Edge
    {
        public Edge(Vertex a, Vertex b, Portal portal)
        {
            VerA = a;
            VerB = b;
            Portal = portal;
        }
        public Vertex VerA { get; private set; }
        public Vertex VerB { get; private set; }
        public Portal Portal { get; private set; }

        public Vertex GetOppositeVertex(Vertex vertex)
        {
            if (VerA != null && VerB != null)
            {
                return (VerA.Equals(vertex)) ? VerB : VerA;
            }
            return null;
        }
    }
}
