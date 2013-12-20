using System.IO;

namespace PlanEditor.Helpers.IO
{
    public class SaveToHtmlData
    {
        public static void Save(Entities.Building building, EvacStruct.Building buildingEvac, string fileName)
        {
            fileName += "\\" + building.Name + "\\помещения.html";            
            using (TextWriter writer = File.CreateText(fileName))
            {
                writer.WriteLine("<html> \n <head><meta content=\"text/html; charset=UTF-8\" http-equiv=\"content-type\" />");

                writer.WriteLine("<h2>Помещения</h2>");
                                
                for (int i = 0; i < building.Stages; ++i)
                {
                    int stage = i + 1;
                    writer.WriteLine("<h3> Этаж " + stage + "</h3>");

                    writer.WriteLine("<table border=\"0\">");

                    writer.WriteLine("<tr>");
                    writer.WriteLine("<td>Номер помещения</td>");
                    writer.WriteLine("<td>Название помещения</td>");
                    writer.WriteLine("<td>Ширина</td>");
                    writer.WriteLine("<td>Длина</td>");
                    writer.WriteLine("<td>Высота</td>");
                    writer.WriteLine("<td>Ширина пути эвакуации</td>");
                    writer.WriteLine("<td>Количество людей</td>");
                    writer.WriteLine("<td>Тип помещения</td>");
                    writer.WriteLine("</tr>");

                    for (int num = 0; num < building.Places[i].Count; ++num)
                    {
                        var place = building.Places[i][num];

                        int id = -1;
                        foreach (var r in buildingEvac.Rooms)
                        {
                            if (r.parent.ID == place.ID) { id = r.ID; break; }
                        }
                        
                        writer.WriteLine("<tr>");

                        writer.WriteLine("<td>" + id + "</td>");
                        writer.WriteLine("<td>" + place.Name + "</td>");
                        writer.WriteLine("<td>" + place.Wide + "</td>");
                        writer.WriteLine("<td>" + place.Length + "</td>");
                        writer.WriteLine("<td>" + place.Height + "</td>");
                        writer.WriteLine("<td>" + place.EvacWide + "</td>");
                        writer.WriteLine("<td>" + place.Ppl + "</td>");
                        writer.WriteLine("<td>" + place.MainType + ", " + place.SubType + "</td>");

                        writer.WriteLine("</tr>");
                    }

                    writer.WriteLine("</table>");
                }

                writer.WriteLine("<h2>Лестницы</h2>");
                writer.WriteLine("<table border=\"1\">");

                writer.WriteLine("<tr>");
                writer.WriteLine("<td>Номер</td>");
                writer.WriteLine("<td>Начальный этаж</td>");
                writer.WriteLine("<td>Конечныый этаж</td>");
                writer.WriteLine("<td>Ширина</td>");
                writer.WriteLine("<td>Длина</td>");
                writer.WriteLine("<td>Высота</td>");
                writer.WriteLine("</tr>");

                for (int i = 0; i < building.Stairways.Count; ++i)
                {
                    var stairway = building.Stairways[i];

                    writer.WriteLine("<tr>");

                    writer.WriteLine("<td>" + i + "</td>");
                    writer.WriteLine("<td>" + stairway.StageFrom + "</td>");
                    writer.WriteLine("<td>" + stairway.StageTo + "</td>");
                    writer.WriteLine("<td>" + stairway.Wide + "</td>");
                    writer.WriteLine("<td>" + stairway.Length + "</td>");
                    writer.WriteLine("<td>" + stairway.Height + "</td>");

                    writer.WriteLine("</tr>");
                }
                writer.WriteLine("</table>");

                writer.WriteLine("<h2>Дверные проемы</h2>");

                for (int i = 0; i < building.Stages; ++i)
                {
                    int stage = i + 1;
                    writer.WriteLine("<h3> Этаж " + stage + "</h3>");

                    writer.WriteLine("<table border=\"1\">");

                    writer.WriteLine("<tr>");
                                        
                    writer.WriteLine("<td>Ширина</td>");
                    writer.WriteLine("<td>Высота</td>");
                    writer.WriteLine("<td>Глубина проема</td>");

                    writer.WriteLine("</tr>");

                    for (int num = 0; num < building.Portals[i].Count; ++num)
                    {
                        var portal = building.Portals[i][num];


                        double wide = (portal.Wide == 0) ? 0.8 : portal.Wide;
                        double height = (portal.Height == 0) ? 2.8 : portal.Height;
                        double depth = (portal.Depth == 0) ? 0.3 : portal.Depth;
                        
                        writer.WriteLine("<tr>");                        
                        writer.WriteLine("<td>" + wide + "</td>");
                        writer.WriteLine("<td>" + height + "</td>");
                        writer.WriteLine("<td>" + depth  + "</td>");

                        writer.WriteLine("</tr>");
                    }

                    writer.WriteLine("</table>");
                }

                writer.WriteLine("</html>");
            }
        }
    }
}
