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
    public class Accommodation
    {
        public Accommodation(Place place, double z)
        {
            ID = place.ID;
            Name = place.Name;
            MainType = place.MainType;
            SubType = place.SubType;

            timeblock = 0.00;

            x1 = place.PointsX[0];
            y1 = place.PointsY[0];
            z1 = z;
            x2 = place.PointsX[1];
            y2 = place.PointsY[2];

            if (place.Type == Entity.EntityType.Stairway)
            {
                var strw = (Stairway)place;
                z2 = strw.Height * strw.StageTo;
                sFrom = strw.StageFrom;
                sTo = strw.StageTo;
                MainType = 7;
            }
            else
            {
                z2 = place.Height; // meters
                sFrom = -1;
                sTo = -1;
            }

            scenario = place.FireType;
        }

        [DataMember] public string ID;
        [DataMember] public string Name;
        [DataMember] public int MainType;
        [DataMember] public int SubType;
        [DataMember] public double timeblock;
        [DataMember] public int scenario;
        [DataMember] public double x1;
        [DataMember] public double y1;
        [DataMember] public double z1;
        [DataMember] public double x2;
        [DataMember] public double y2;
        [DataMember] public double z2;
        [DataMember] public double sFrom;
        [DataMember] public double sTo;
    }
}
