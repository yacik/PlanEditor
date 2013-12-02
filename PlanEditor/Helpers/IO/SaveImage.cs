using System.Drawing;
using System.Linq;
using PlanEditor.Entities;

namespace PlanEditor.Helpers.IO
{
    public class SaveImage
    {
        public static void SaveFile(Entities.Building building, string folder)
        {
            int roomCount = 0;
            int strwCount = 0;
            int portalCount = 0;

            for (int i = 0; i < building.Stages; ++i)
            {
                int wide = (int) (building.Lx/Constants.Sigma);
                int height = (int) (building.Ly/Constants.Sigma);

                var bitmap = new Bitmap(wide, height);
                var graphics = Graphics.FromImage(bitmap);
                graphics.Clear(Color.DarkGray);
                
                if (building.Places[i] != null)
                {
                    foreach (var place in building.Places[i])
                    {
                        place.PrepareForSave();
                        
                        int xMin = (int)place.ExportX.Min(v => v);
                        int xMax = (int)place.ExportX.Max(v => v);
                        int yMin = (int)place.ExportY.Min(v => v);
                        int yMax = (int)place.ExportY.Max(v => v);

                        int x = xMax - xMin;
                        int y = yMax - yMin;

                        switch (place.Type)
                        {
                            case Entity.EntityType.Halfway:
                                graphics.FillRectangle(Brushes.DarkGreen, xMin, yMin, x, y);
                                break;
                            default:
                                graphics.FillRectangle(Brushes.DarkSlateBlue, xMin, yMin, x, y);
                                break;
                        }

                        ++roomCount;
                        graphics.DrawString(roomCount.ToString(), new Font("Verdana", 12), Brushes.Black, new PointF(xMin, yMin));
                    }
                }

                foreach (var stairway in building.Stairways)
                {
                    int stageFrom = stairway.StageFrom - 1;
                    int stageTo = stairway.StageTo - 1;

                    if (i >= stageFrom && i <=stageTo )
                    {
                        stairway.PrepareForSave();

                        int xMin = (int) stairway.ExportX.Min(v => v);
                        int xMax = (int) stairway.ExportX.Max(v => v);
                        int yMin = (int) stairway.ExportY.Min(v => v);
                        int yMax = (int) stairway.ExportY.Max(v => v);

                        int x = xMax - xMin;
                        int y = yMax - yMin;

                        graphics.FillRectangle(Brushes.DarkSlateGray, xMin, yMin, x, y);

                        ++strwCount;
                        graphics.DrawString(strwCount.ToString(), new Font("Verdana", 12), Brushes.Black, new PointF(xMin, yMin));
                    }
                }

                if (building.Portals[i] != null)
                {
                    foreach (var portal in building.Portals[i])
                    {
                        portal.PrepareForSave();

                        int xMin = (int)portal.ExportX.Min(v => v);
                        int xMax = (int)portal.ExportX.Max(v => v);
                        int yMin = (int)portal.ExportY.Min(v => v);
                        int yMax = (int)portal.ExportY.Max(v => v);

                        int x = xMax - xMin;
                        int y = yMax - yMin;

                        graphics.FillRectangle(Brushes.DarkSeaGreen, xMin, yMin, x, y);

                        ++portalCount;
                        graphics.DrawString(portalCount.ToString(), new Font("Verdana", 12), Brushes.Black, new PointF(xMin, yMin));
                    }
                }

                int stage = i + 1;
                bitmap.Save(folder + "\\stage_" + stage + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }
        }
    }
}
