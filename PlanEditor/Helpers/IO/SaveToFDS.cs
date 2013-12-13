using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using PlanEditor.Entities;

namespace PlanEditor.Helpers.IO
{
    #region Export DS

    [DataContract]
    internal struct Accommodation
    {
        public Accommodation(Place place, int id, double z)
        {
            ID = id;
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
                var strw = (Stairway) place;
                z2 = strw.Height * strw.StageTo;
                sFrom = strw.StageFrom;
                sTo = strw.StageTo;
            }
            else
            {
                z2 = place.Height; // meters
                sFrom = -1;
                sTo = -1;
            }

            scenario = place.FireType;
        }
        
        [DataMember] public int ID;
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
    };

    [DataContract]
    internal struct Entry
    {
        public Entry(Portal portal, int id, int stage)
        {
            ID = id;

            x1 = portal.PointsX[0];
            y1 = portal.PointsY[0];
            x2 = portal.PointsX[1];
            y2 = portal.PointsY[2];
            z1 = stage;
            z2 = (portal.Height < 1) ? 3.0 : portal.Height; // meters
        }

        [DataMember] public int ID;
        [DataMember] public double x1;
        [DataMember] public double y1;
        [DataMember] public double z1;
        [DataMember] public double x2;
        [DataMember] public double y2;
        [DataMember] public double z2;
    }

    [DataContract]
    internal struct Bldg
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
    };

    #endregion

    public class SaveToFDS
    {
        public static void Save(Entities.Building building, string fileName)
        {
            var bldg = new Bldg(building);
            int countPlaces = 0;
            int countPortals = 0;
            int countStairway = 0;

            for (int curStage = 0; curStage < building.Stages; ++curStage)
            {
                if (building.Places.Count > curStage)
                {
                    foreach (var place in building.Places[curStage])
                    {
                        bldg.Rooms.Add(new Accommodation(place, countPlaces, curStage));
                        ++countPlaces;
                    }

                    foreach (var portal in building.Portals[curStage])
                    {
                        bldg.Portals.Add(new Entry(portal, countPortals, curStage));
                        ++countPortals;
                    }

                    foreach (var strws in building.Stairways)
                    {
                        bldg.Rooms.Add(new Accommodation(strws, countStairway, curStage));
                        ++countStairway;
                    }
                }
            }

            var stream = new MemoryStream();
            var ser = new DataContractJsonSerializer(typeof(Bldg));
            ser.WriteObject(stream, bldg);
            stream.Position = 0;
            var sr = new StreamReader(stream, Encoding.UTF8);
            var read = sr.ReadToEnd();
            var sw = new StreamWriter(fileName);
            sw.Write(read);
            sw.Close();
        }
    }
}
