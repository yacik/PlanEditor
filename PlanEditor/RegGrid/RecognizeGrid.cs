using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PlanEditor.Entities;

namespace PlanEditor.RegGrid
{
    public class RecognizeGrid
    {
        private readonly Grid _grid;
        private readonly Building _building;

        public RecognizeGrid(Grid grid, Building building)
        {
            _grid = grid;
            _building = building;
        }

        public Grid Grid
        {
            get { return _grid; }
        }

        public void Recognize()
        {
            for (int i = 0; i < _building.Stages; ++i)
            {
                if (_building.Portals.Count > i)
                {
                    foreach (var portal in _building.Portals[i])
                    {
                        if (_grid.Cells.Count > i)
                        {
                            foreach (var cell in _grid.Cells[i].Where(cell => cell.Owner == null))
                            {
                                if (MyMath.Helper.IsCollide(cell.CenterX, cell.CenterY, portal.PointsX, portal.PointsY))
                                {
                                    if (portal.Cells == null) portal.Cells = new List<Cell>();

                                    portal.Cells.Add(cell);
                                    cell.Owner = portal;
                                }
                            }
                        }
                    }
                }

                if (_building.Places.Count > i)
                {
                    foreach (var place in _building.Places[i])
                    {
                        if (_grid.Cells.Count > i)
                        {
                            var pointsX = place.PointsX;
                            var pointsY = place.PointsY;

                            foreach (var cell in _grid.Cells[i].Where(cell => cell.Owner == null).Where(cell => MyMath.Helper.IsCollide(cell.CenterX, cell.CenterY, pointsX, pointsY)))
                            {
                                ++place.CountNodes;
                                cell.Owner = place;
                                ++_building.PplCell;
                            }
                        }
                    }
                }
            }
            
            foreach (var stairway in _building.Stairways)
            {
                int start = stairway.StageFrom - 1;
                int end = stairway.StageTo;
                for (int i = start; i < end; ++i)
                {
                    var pointsX = stairway.PointsX;
                    var pointsY = stairway.PointsY;
                    foreach (var cell in _grid.Cells[i].Where(cell => cell.Owner == null).Where(cell => MyMath.Helper.IsCollide(cell.CenterX, cell.CenterY, pointsX, pointsY)))
                    {
                        cell.Owner = stairway;
                        if (stairway.Cells == null) stairway.Cells = new List<Cell>();
                        stairway.Cells.Add(cell);
                        ++stairway.CountNodes;
                    }
                }
               stairway.CountNodes /= 2;
            }
        }

        public static void DefineStairways(List<List<Stairway>> stairways, Grid grid)
        {
            for (int i = 0; i < stairways.Count; ++i)
            {
                foreach (var stairway in stairways[i])
                {
                    if (grid.Cells.Count > i) 
                    {
                        var pointsX = stairway.PointsX;
                        var pointsY = stairway.PointsY;
                        foreach (var cell in grid.Cells[i].Where(cell => cell.Owner == null || cell.Owner.Type == Entity.EntityType.Stairway).Where(cell => MyMath.Helper.IsCollide(cell.CenterX, cell.CenterY, pointsX, pointsY)))
                        {
                            cell.Owner = stairway;
                            if (stairway.Cells == null) stairway.Cells = new List<Cell>();
                            stairway.Cells.Add(cell);
                            //++stairway.CountNodes;
                        }
                    }
                }
            }
        }
    }
}
