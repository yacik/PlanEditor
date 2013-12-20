using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using PlanEditor.Entities;
using System.Diagnostics;
using PlanEditor.FDSStruct;

namespace PlanEditor.Helpers.IO
{
    public class SaveToFDS
    {
        private static List<List<Stairway>> Stairways = new List<List<Stairway>>();

        public static void Save(Entities.Building building, string fileName)
        {
            var bldg = new Bldg(building);
            
            for (int curStage = 0; curStage < building.Stages; ++curStage)
            {
                if (building.Places.Count > curStage)
                {
                    foreach (var place in building.Places[curStage])
                    {
                        bldg.Rooms.Add(new Accommodation(place, curStage));
                    }                    
                }
            }
                        
            for (int curStage = 0; curStage < building.Stages; ++curStage)
            {
                foreach (var portal in building.Portals[curStage])
                {
                    bldg.Portals.Add(new Entry(portal, curStage));
                }
            }

            foreach (var strws in building.Stairways)
            {
                bldg.Rooms.Add(new Accommodation(strws, strws.StageTo));
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
