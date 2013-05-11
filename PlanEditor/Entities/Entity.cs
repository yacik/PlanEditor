using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PlanEditor.Entities
{
    [Serializable]
    public class Entity
    {
        public Entity()
        {
            ID = curID;
            ++curID;
        }
        
        public int ID { get; private set; }
        public enum EntityType { Place, Portal }
        public EntityType Type { get; set; }
        private static int curID = 0;

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
                PathGeometry pg = (PathGeometry)UI.Data;
                List<double> lst = new List<double>();                
                foreach (PathFigure pf in pg.Figures)
                {
                    lst.Add(pf.StartPoint.X);
                                        
                    foreach (LineSegment ls in pf.Segments)
                    {
                        lst.Add(ls.Point.X);
                    }                    
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
