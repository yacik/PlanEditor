using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using PlanEditor.Helpers;

namespace PlanEditor.Entities
{
    [Serializable]
    public class Entity
    {
        protected Entity()
        {
            ID = Data.CurID;
            ++Data.CurID;
        }
        
        public int ID { get; private set; }
        public enum EntityType { Place, Portal, Stairway, Halfway, Lift }
        public EntityType Type { get; set; }

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
                    break;
                case EntityType.Halfway:
                    p.Fill = Colours.Green;
                    break;
                case EntityType.Stairway:
                    p.Fill = Colours.Violet;
                    break;
                case EntityType.Portal:
                    p.Fill = Colours.LightGray;
                    break;
            }
        }

        [NonSerialized]
        public Path UI;

        protected List<double> ExportX;
        protected List<double> ExportY;

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
                List<double> lst = new List<double>();
                PathGeometry pg = (PathGeometry)UI.Data;
                foreach (PathFigure pf in pg.Figures)
                {
                    lst.Add(pf.StartPoint.Y);

                    foreach (LineSegment ls in pf.Segments)
                    {
                        lst.Add(ls.Point.Y);
                    }
                }

                return lst;
            }
        }

        public void Select()
        {
            if (UI != null)
            {
                UI.Stroke = Colours.Red;
            }
        }
        public void Deselect()
        {
            if (UI != null)
            {
                UI.Stroke = Colours.Black;
            }
        }

        protected double distance(double a, List<double> list)
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

            return dist * Data.Sigma;
        }
    }
}
