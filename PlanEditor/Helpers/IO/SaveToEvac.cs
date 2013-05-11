using PlanEditor.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanEditor.IO
{
    public class SaveToEvac
    {
        public void Save(string fileName, RegGrid.Grid grid, Entities.Building building)
        {
            StreamWriter sw = new StreamWriter(fileName);

            sw.WriteLine(building.Name);
            sw.WriteLine(building.Stages);
            sw.WriteLine(building.Row);
            sw.WriteLine(building.Col);
            sw.WriteLine(building.GetNumPlaces);
            sw.WriteLine(building.GetNumMines);
            sw.WriteLine(building.NumNodes);
            sw.WriteLine(building.MaxPeople);
            sw.WriteLine(building.People);
            sw.WriteLine(building.PplCell);
            sw.WriteLine(building.Lx);
            sw.WriteLine(building.Ly);

            for (int i = 0; i < building.Stages; ++i)
            {
                sw.WriteLine("Grid");
                foreach (var cell in grid.Cells[i])
                {
                    sw.Write(cell.M + " " + cell.N + " " + cell.ID + " " + cell.Owner + "\n");
                }

                sw.WriteLine("Rooms");
                foreach (Place place in building.Places[i])
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (Portal portal in building.Portals[i])
                    {
                        if (portal.RoomA == place)
                        {
                            if (portal.RoomB != null)
                                sb.Append(portal.RoomB.ID + " ");
                        }
                        else if (portal.RoomB == place)
                        {
                            if (portal.RoomA != null)
                                sb.Append(portal.RoomA.ID + " ");
                        }
                    }
                    sw.WriteLine(place.ToString() + " ng:" + sb.ToString());
                }

                sw.WriteLine("Portals");
                foreach (Portal p in building.Portals[i])
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("1.001");

                    if (p.RoomA == null || p.RoomB == null)
                        sb.Append(" " + 1);
                    else
                        sb.Append(" " + 2);

                    sb.Append(" ng:");

                    foreach (PlanEditor.RegGrid.Cell c in p.Cells)
                    {
                        sb.Append(" " + c.ID);
                    }

                    sw.WriteLine(p.ID + " " + sb.ToString() + "\n");
                }
            }

            sw.Close();
        }
    }
}
