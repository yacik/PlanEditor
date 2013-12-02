using System.Collections.Generic;
using PlanEditor.Entities;

namespace PlanEditor.Graph
{
    public class SimpleGraph
    {
        public SimpleGraph()
        { 
            Vertices = new List<Vertex>();
            Edgies = new List<Edge>();
        }

        public List<Vertex> Vertices { get; private set; }
        public List<Edge> Edgies { get; private set; }

        public bool? IsPortal(Portal portal)
        {
            if (portal == null) return null;

            foreach (var edge in Edgies) if (edge.Portal.Equals(portal)) return true;

            return false;
        }

        public bool? IsPlace(Place place)
        {
            if (place == null) return null;

            foreach (var vertex in Vertices)
            {
                if (vertex.Place != null)
                {
                    if (vertex.Place.Equals(place)) return true;
                }
            }
            
            return false;
        }

        public Vertex GetVertexByPlace(Place place)
        {
            if (place == null) return null;

            foreach (var vertex in Vertices)
            {
                if (vertex.Place != null)
                {
                    if (vertex.Place.Equals(place)) return vertex;
                }
            }

            return null;
        }

        public Edge GetEdgeByPortal(Portal portal)
        {
            if (portal == null) return null;

            foreach (var edge in Edgies)
            {
                if (edge.Portal.Equals(portal)) return edge;
            }

            return null;
        }
    }
}
