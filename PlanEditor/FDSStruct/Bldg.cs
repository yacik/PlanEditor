using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PlanEditor.FDSStruct
{
    [DataContract]
    public class Bldg
    {
        public Bldg(Entities.Building building)
        {
            Name = building.Name;
            Stages = building.Stages;
            Rooms = new List<Accommodation>();
            Portals = new List<Entry>();

            Wide = MyMath.Helper.MetersToPxl(building.Lx);
            Length = MyMath.Helper.MetersToPxl(building.Ly);
            Height = building.HeightStage; // meters
        }

        [DataMember] public string Name;
        [DataMember] public int Stages;
        [DataMember] public double Wide;
        [DataMember] public double Length;
        [DataMember] public double Height;
        [DataMember] public List<Accommodation> Rooms;
        [DataMember] public List<Entry> Portals;
    }
}
