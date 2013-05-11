using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanEditor.RegGrid
{
    public class RecognizeGrid
    {
        private Grid m_grid;
        private Entities.Building m_building;

        public RecognizeGrid(Grid grid, Entities.Building building)
        {
            m_grid = grid;
            m_building = building;
        }

        public void Recognize()
        {
            for (int i = 0; i < m_building.Stages; ++i)
            {
                foreach (var portal in m_building.Portals[i])
                {
                    foreach (var cell in m_grid.Cells[i])
                    {
                        if (cell.Owner != -1) continue;

                        if (MyMath.Geometry.IsCollide(cell.PosX, cell.PosY, portal.PointsX, portal.PointsY))
                        {
                            portal.Cells.Add(cell);
                            cell.Owner = portal.ID;
                        }
                    }
                }

                foreach (var place in m_building.Places[i])
                {
                    foreach (var cell in m_grid.Cells[i])
                    {
                        if (cell.Owner != -1 ) continue;

                        if (MyMath.Geometry.IsCollide(cell.PosX, cell.PosY, place.PointsX, place.PointsY))
                        {
                            ++place.CountNodes;
                            cell.Owner = place.ID;
                            ++m_building.PplCell;
                        }
                    }
                }
            }
        }
    }
}
