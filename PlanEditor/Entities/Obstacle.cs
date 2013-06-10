using System;
using System.Windows;

namespace PlanEditor.Entities
{
    [Serializable]
    public class Obstacle : Entity
    {
        public Obstacle(Place place)
        {
            Owner = place;
            Type = EntityType.Obstacle;
        }
        public Place Owner { get; private set; }
        public Point MinPoint
        {
            get
            {
                var point = new Point(double.MaxValue, double.MaxValue);
                var px = Owner.PointsX;
                var py = Owner.PointsY;
                for (int i = 0; i < px.Count; ++i)
                {
                    if (point.X > px[i]) point.X = px[i];
                    if (point.Y > py[i]) point.Y = py[i];
                }

                return point;
            }
        }
        public Point MaxPoint
        {
            get
            {
                var point = new Point(double.MinValue, double.MinValue);
                var px = Owner.PointsX;
                var py = Owner.PointsY;
                for (int i = 0; i < px.Count; ++i)
                {
                    if (point.X < px[i]) point.X = px[i];
                    if (point.Y < py[i]) point.Y = py[i];
                }

                return point;
            }
        }
    }
}
