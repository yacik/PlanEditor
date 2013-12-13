using System.Collections.Generic;
using System.Linq;
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

            return Edgies.Any(edge => edge.Portal.Equals(portal));
        }

        public bool? IsPlace(Place place)
        {
            if (place == null) return null;

            return Vertices.Where(vertex => vertex.Place != null).Any(vertex => vertex.Place.Equals(place));
        }

        public Vertex GetVertexByPlace(Place place)
        {
            if (place == null) return null;

            return Vertices.Where(vertex => vertex.Place != null).FirstOrDefault(vertex => vertex.Place.Equals(place));
        }

        public Edge GetEdgeByPortal(Portal portal)
        {
            if (portal == null) return null;

            return Edgies.FirstOrDefault(edge => edge.Portal.Equals(portal));
        }
    }
}
