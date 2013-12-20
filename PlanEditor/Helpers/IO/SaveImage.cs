using System.Drawing;
using System.Linq;
using PlanEditor.Entities;
using System.Collections.Generic;
using System;

namespace PlanEditor.Helpers.IO
{
    public class SaveImage
    {
        public static void SaveFile(EvacStruct.Building building, string folder)
        {
            folder += "\\" + building.Name;
            try
            {
                System.IO.Directory.CreateDirectory(folder);
            }
            catch (Exception ex)
            {
                PELogger.GetLogger.WriteLn(ex.ToString());
            }
            for (int i = 0; i < building.Stages; ++i)
            {
                int wide = (int) (building.Lx/Constants.Sigma);
                int height = (int) (building.Ly/Constants.Sigma);

                var bitmap = new Bitmap(wide, height);
                var graphics = Graphics.FromImage(bitmap);
                graphics.Clear(Color.White);

                foreach (var room in building.Rooms)
                {
                    if (room.z1 != i) continue;

                    int x = (int)(room.x2 - room.x1);
                    int y = (int)(room.y2 - room.y1);

                    graphics.FillRectangle(Brushes.LightGray, (int)room.x1, (int)room.y1, x, y);

                    graphics.DrawString(room.ID.ToString(), new Font("Verdana", 22), Brushes.Black, new PointF((int)room.x1, (int)room.y1));
                }

                foreach (var portal in building.Portals)
                {
                    if (portal.z1 != i) continue;

                    int x = (int)(portal.x2 - portal.x1);
                    int y = (int)(portal.y2 - portal.y1);

                    graphics.FillRectangle(Brushes.DarkSeaGreen, (int)portal.x1, (int)portal.y1, x, y);

                    //graphics.DrawString(portal.ID.ToString(), new Font("Verdana", 22), Brushes.Black, new PointF((int)portal.x1, (int)portal.y1));
                }
                
                int stage = i + 1;
                bitmap.Save(folder + "\\этаж_" + stage + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }
        }
    }
}
