using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;
using PlanEditor.Entities;

namespace PlanEditor.Helpers.IO
{
    public class LoadSvgFile
    {
        public static void LoadFile(String fileName, Entities.Building building)
        {
            var doc = new XmlDocument();
            doc.Load(fileName);

            var nodeList = doc.GetElementsByTagName("path");
            var layers = doc.GetElementsByTagName("g");

            int[] mas = new int[layers.Count];
            for (int i = 0; i < mas.Length; ++i)
                mas[i] = 0;


            List<Place> places = null;
            
            int placeId = 0;
            foreach (XmlNode node in nodeList)
            {
                for (int i = 0; i < layers.Count; ++i)
                {
                    var parent = layers[i];

                    if (node.ParentNode.Attributes.GetNamedItem("id").Value == parent.Attributes.GetNamedItem("id").Value)
                    {
                        if (mas[i] == 0)
                        {
                            places = new List<Place>();

                            building.Places.Add(places);
                            ++building.Stages;

                            mas[i] = 1;
                        }
                    }
                }

                if (node.Attributes == null) continue;

                foreach (var v in from XmlAttribute v in node.Attributes where v.Name.Equals("inkscape:label") select v)
                {
                    switch (v.Value)
                    {
                        case "place":
                        case "halfway":
                            var pl = new Place();
                            
                            var points = ConvertStrings.ConverteString(node.Attributes.GetNamedItem("d").Value);
                            pl.UI = GetPath(points);
                            pl.UI.StrokeThickness = 3;
                            pl.UI.Stroke = Colours.Black;
                            switch (v.Value)
                            {
                                case "place":
                                    pl.Type = Entity.EntityType.Place;
                                    pl.UI.Fill = Colours.Indigo;
                                    break;
                                case "halfway":
                                    pl.Type = Entity.EntityType.Halfway;
                                    pl.UI.Fill = Colours.Green;
                                    break;
                            }

                            if (places != null) places.Add(pl);

                            break;
                    }
                }
            }
        }

        private static Path GetPath(List<double> points)
        {
            var pg = new PathGeometry { FillRule = FillRule.Nonzero };

            var pf = new PathFigure();
            pg.Figures.Add(pf);

            var exportX = new List<double>();
            var exportY = new List<double>();
            for (int i = 0; i < points.Count; ++i)
            {
                if (i%2 == 0)
                    exportX.Add(points[i]);
                else
                    exportY.Add(points[i]);
            }

            pf.StartPoint = new Point(exportX[0], exportY[0]);
            var startPoint = pf.StartPoint;

            for (int i = 1, j = 1; i < exportX.Count && j < exportY.Count; ++i, ++j)
            {
                var ls = new LineSegment { Point = new Point(exportX[i], exportY[j]) };
                pf.Segments.Add(ls);
            }

            var p = new Path { StrokeThickness = 2, Stroke = Colours.Black, Data = pg };

            return p;
        }
    }
}
