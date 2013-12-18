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
    public class Room
    {
        public Room(Place place, int id, double z, int? FireType)
        {
            ID = id;
            parent = place;
            CountNodes = place.CountNodes;
            Ppl = place.Ppl;
            MaxPeople = place.MaxPeople;
            Name = place.Name;
            Wide = place.Wide;
            Height = place.Height;
            Length = place.Length;
            SHeigthRoom = place.SHeigthRoom;
            DifHRoom = place.DifHRoom;
            MainType = place.MainType;
            SubType = place.SubType;

            scenario = (FireType.HasValue) ? FireType.Value : -1;

            //timeblock = (scenario == 1) ? 95.00 : 0.00;
            timeblock = 0.00;

            x1 = place.PointsX[0];
            y1 = place.PointsY[0];
            z1 = z;
            x2 = place.PointsX[1];
            y2 = place.PointsY[2];
            z2 = Height; // meters

            Neigh = new List<int>();
        }

        public void SetScenario(int id)
        {
            scenario = id;
        }

        [DataMember] public int ID;
        [DataMember] public int CountNodes;
        [DataMember] public int Ppl;
        [DataMember] public int MaxPeople;
        [DataMember] public string Name;
        [DataMember] public double Wide;
        [DataMember] public double Height;
        [DataMember] public double Length;
        [DataMember] public double SHeigthRoom;
        [DataMember] public double DifHRoom;
        [DataMember] public int MainType;
        [DataMember] public int SubType;
        [DataMember] public List<int> Neigh;
        [DataMember] public double timeblock;
        [DataMember] public int scenario;
        [DataMember] public double x1;
        [DataMember] public double y1;
        [DataMember] public double z1;
        [DataMember] public double x2;
        [DataMember] public double y2;
        [DataMember] public double z2;

        public Place parent;
    };
}
