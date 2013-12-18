using PlanEditor.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PlanEditor.EvacStruct
{
    [DataContract]
    public class Door
    {
        public Door(Portal portal, int id, int stage, int idRoom)
        {
            Wide = portal.Wide;
            Code = (portal.RoomB == null) ? 1 : 2;
            IsBlocked = portal.IsBlocked;
            IDScenario = IsBlocked ? idRoom : -1;

            ID = id;
            Cells = new List<int>();
            if (portal.Cells != null)
            {
                foreach (var cell in portal.Cells)
                {
                    Cells.Add(cell.M);
                    Cells.Add(cell.N);
                    Cells.Add(cell.K);
                }
            }
            // x - 1, y - 2
            directAper = (portal.Orientation == Portal.PortalOrient.Vertical) ? 1 : 2;

            x1 = portal.PointsX[0];
            y1 = portal.PointsY[0];
            x2 = portal.PointsX[1];
            y2 = portal.PointsY[2];
            z1 = stage;
            z2 = (portal.Height < 1) ? 3.0 : portal.Height; // meters
        }

        [DataMember] public double Wide;
        [DataMember] public int Code;
        [DataMember] public int ID;
        [DataMember] public List<int> Cells;
        [DataMember] public int directAper;
        [DataMember] public double x1;
        [DataMember] public double y1;
        [DataMember] public double z1;
        [DataMember] public double x2;
        [DataMember] public double y2;
        [DataMember] public double z2;
        [DataMember] public bool IsBlocked;
        [DataMember] public int IDScenario;
    }
}
