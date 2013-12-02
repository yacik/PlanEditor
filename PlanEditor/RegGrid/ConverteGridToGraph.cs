using PlanEditor.Graph;

namespace PlanEditor.RegGrid
{
    public class ConverteGridToGraph
    {
        public ConverteGridToGraph()
        {

        }

        private SimpleGraph graph = new SimpleGraph();
        /*

        // a - Кол-во ячеек сетки на этаже
        public SimpleGraph FromGraph(int a)
        {
            graph.Edgies.Clear();
            graph.Vertices.Clear();

            DefineVertices(a);
            DefineEdges(a);

            return graph;
        }

        private void DefineVertices(int a)
        {
            for (int i = 0; i < Constants.Stages; ++i)
            {
                foreach (var place in Constants.Places[i])
                {
                    //if (place.Type != Place.PlaceType.Halfway) continue;
                    foreach (Cell c in place.Cells)
                    {
                        int id = c.ID + i * a;
                        Vertex ver = new Vertex(id, c.CenterX, c.CenterY);
                        graph.Vertices.Add(ver);
                    }
                }
            }
        }

        private void DefineEdges(int a)
        {
            for (int i = 0; i < Constants.Stages; ++i)
            {
                foreach (var place in Constants.Places[i])
                {
                    //if (place.Type != Place.PlaceType.Halfway) continue;

                    foreach (Cell c in place.Cells)
                    {
                        int id = c.ID + i * a;
                        Vertex v1 = GetVertexByID(id);

                        foreach (Cell n in c.Neigh)
                        {
                            int id_n = n.ID + i * a;
                            Vertex v2 = GetVertexByID(id_n);

                            Edge e = GetEdge(v1, v2);
                            if (e == null)
                            {
                                e = new Edge(v1, v2);
                                graph.Edgies.Add(e);
                            }
                            
                            v1.Edges.Add(e);                            
                        }
                    }
                }
            }
        }

        private Edge GetEdge(Vertex a, Vertex b)
        {
            foreach (Edge v in graph.Edgies)
            {
                if ((v.VerA == a && v.VerB == b) || (v.VerA == b && v.VerB == a))
                    return v;
            }

            return null;
        }

        private Vertex GetVertexByID(int id)
        {
            foreach (Vertex v in graph.Vertices)
            {
                if (v.ID == id)
                    return v;
            }
            return null;
        }
         */
    }
}
