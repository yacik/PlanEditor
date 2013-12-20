using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using PlanEditor.Helpers;
using System.Diagnostics;

namespace PlanEditor.Entities
{
    [Serializable]
    public class Entity
    {
        public enum EntityType { Place, Portal, Stairway, Halfway, Lift, Obstacle }
        public EntityType Type { get; set; }

        [NonSerialized] 
        public List<Entity> Collisions = new List<Entity>();

        private Guid _id;
        public string ID 
        { 
            get 
            {
                if (_id.ToString().Contains("00000000"))
                {
                    _id = Guid.NewGuid();                    
                }

                return _id.ToString().Substring(0, 8);                
            }         
        }        
        
        public void Show()
        {
            if (UI != null) UI.Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            if (UI != null) UI.Visibility = Visibility.Hidden;
        }

        public void LoadUI()
        {
            var pg = new PathGeometry { FillRule = FillRule.Nonzero };

            var pf = new PathFigure();
            pg.Figures.Add(pf);

            pf.StartPoint = new Point(ExportX[0], ExportY[0]);

            for (int i = 1, j = 1; i < ExportX.Count && j < ExportY.Count; ++i, ++j)
            {
                var ls = new LineSegment { Point = new Point(ExportX[i], ExportY[j]) };
                pf.Segments.Add(ls);
            }

            var p = new Path { StrokeThickness = 2, Stroke = Colours.Black, Data = pg };
            UI = p;

            switch (Type)
            {
                case EntityType.Place:
                    p.Fill = Colours.Indigo;
                    //p.Fill = Colours.Room;
                    break;
                case EntityType.Halfway:
                    //p.Fill = Colours.Halfway;
                    p.Fill = Colours.Green;
                    break;
                case EntityType.Stairway:
                    p.Fill = Colours.Violet;
                    //p.Fill = Colours.Stairway;
                    break;
                case EntityType.Portal:
                    p.Fill = Colours.LightGray;
                    break;
                case EntityType.Obstacle:
                    p.Stroke = Colours.LightGray;
                    p.Fill = Colours.Gray;
                    break;
            }
        }

        [NonSerialized]
        public Path UI;

        public List<double> ExportX;
        public List<double> ExportY;

        public void PrepareForSave()
        {
            ExportX = PointsX;
            ExportY = PointsY;
        }

        public List<double> PointsX 
        { 
            get 
            {
                var pg = (PathGeometry)UI.Data;
                var lst = new List<double>();          
      
                foreach (var pf in pg.Figures)
                {
                    lst.Add(pf.StartPoint.X);
                    lst.AddRange(from LineSegment ls in pf.Segments select ls.Point.X);
                }

                return lst;
            }         
        }
        public List<double> PointsY
        {
            get 
            {
                var lst = new List<double>();
                var pg = (PathGeometry)UI.Data;

                foreach (var pf in pg.Figures)
                {
                    lst.Add(pf.StartPoint.Y);
                    lst.AddRange(from LineSegment ls in pf.Segments select ls.Point.Y);
                }

                return lst;
            }
        }

        public void Select()
        {
            if (UI != null) UI.Stroke = Colours.Red;
        }
       
        public void Deselect()
        {
            if (UI != null) UI.Stroke = Colours.Black;
        }

        public double Length // длина
        {
            get
            {
                return distance(PointsY[0], PointsY);
            }
        }

        public double Wide	// ширина
        {
            get
            {
                return distance(PointsX[0], PointsX);
            }
        }

        private double distance(double a, List<double> list)
        {
            double a1 = list[0];
            double dist = 0;
            for (int i = 1; i < list.Count; ++i)
            {
                double a2 = list[i];
                double d = Math.Sqrt((a1 - a2) * (a1 - a2));
                if (d > dist)
                    dist = d;
            }

            return dist * Constants.Sigma;
        }
    }
}
