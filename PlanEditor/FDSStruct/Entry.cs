using PlanEditor.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PlanEditor.FDSStruct
{
    [DataContract]
    public class Entry
    {
        public Entry(Portal portal, int stage)
        {
            ID = portal.ID;

            x1 = portal.PointsX[0];
            y1 = portal.PointsY[0];
            x2 = portal.PointsX[1];
            y2 = portal.PointsY[2];
            z1 = stage;
            z2 = (portal.Height < 1) ? 3.0 : portal.Height; // meters

            isBlocked = portal.IsBlocked;

            if (portal.RoomA != null) roomA = portal.RoomA.ID;
            else roomA = "-1";
            
            if (portal.RoomB != null) roomB = portal.RoomB.ID;
            else roomB = "-1";            
        }

        [DataMember] public string ID;
        [DataMember] public double x1;
        [DataMember] public double y1;
        [DataMember] public double z1;
        [DataMember] public double x2;
        [DataMember] public double y2;
        [DataMember] public double z2;
        [DataMember] public bool isBlocked;
        [DataMember] public string roomA;
        [DataMember] public string roomB;

    }
}
