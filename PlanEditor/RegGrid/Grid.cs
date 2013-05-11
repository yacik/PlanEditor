using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanEditor.RegGrid
{
    public class Grid
    {
        private Entities.Building m_Building;

        public Grid(Entities.Building building)
        {
            m_Building = building;
        }

        public List<List<Cell>> Cells = new List<List<Cell>>();                
        public int[,,,] Matrix;	// Расчетная сетка в здании  [0][][][] - Код узла сетки   [1][][][] - Номер помещения
                
        public void CreateGrid()
        {
            //Matrix = new int[2, m_Building.Stages, m_Building.Row, m_Building.Col];

            m_Building.NumNodes = 1;
            for (int i = 0; i < m_Building.Stages; ++i)
            {
                int id = 0;
                List<Cell> cells = new List<Cell>();

                for (int n = 0; n < m_Building.Col; ++n)
                {
                    for (int m = 0; m < m_Building.Row; ++m)
                    {
                        double c_x = m * Data.GridStep + Data.GridStep / 2;
                        double c_y = n * Data.GridStep + Data.GridStep / 2;
                        Cell c = new Cell(c_x, c_y, m, n);     
                                                
                        c.ID = id;                         
                        cells.Add(c);

                        //Matrix[0, i, m, n] = -1;
                        //Matrix[1, i, m, n] = -1;

                        ++m_Building.NumNodes;
                        ++id;
                    }
                }
                Cells.Add(cells);
            }
        }
    }
}
