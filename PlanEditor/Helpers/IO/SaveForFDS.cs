using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace PlanEditor.Helpers.IO
{
    [DataContract]
    internal struct Cube
    {
        public Cube(int stage, double leng, double _x1, double _z1, double _x2, double _z2)
        {                                                                                   
            ++stage;                                                                        
                                                                                            
            y1 = stage*leng;                                                                
            y2 = y1 + leng;                                                                 

            x1 = _x1;
            x2 = _x1 + _x2;

            z1 = _z1;
            z2 = _z1 + _z2;
        }

        [DataMember] public double x1;
        [DataMember] public double y1;
        [DataMember] public double z1;
        [DataMember] public double x2;
        [DataMember] public double y2;
        [DataMember] public double z2;
    }

    public class SaveForFDS
    {
        public static void Save(Entities.Building building, string fileName)
        {
            var leng = building.HeightStage;
            var list = new List<Cube>();

            for (int i = 0; i < building.Stages; ++i)
            {
                if (building.Portals[i] != null)
                {
                    foreach (var p in building.Portals[i])
                    {
                        p.PrepareForSave();

                        var xMin = p.ExportX.Min(v => v);
                        var xMax = p.ExportX.Max(v => v);
                        var yMin = p.ExportY.Min(v => v);
                        var yMax = p.ExportY.Max(v => v);

                        list.Add(new Cube(i, leng, xMin, xMax, yMin, yMax));
                    }
                }

                if (building.Places[i] != null)
                {
                    foreach (var p in building.Places[i])
                    {
                        p.PrepareForSave();

                        var xMin = p.ExportX.Min(v => v);
                        var xMax = p.ExportX.Max(v => v);
                        var yMin = p.ExportY.Min(v => v);
                        var yMax = p.ExportY.Max(v => v);

                        list.Add(new Cube(i, leng, xMin, xMax, yMin, yMax));
                    }
                }
            }

            foreach (var strw in building.Stairways)
            {
                strw.PrepareForSave();

                var xMin = strw.ExportX.Min(v => v);
                var xMax = strw.ExportX.Max(v => v);
                var yMin = strw.ExportY.Min(v => v);
                var yMax = strw.ExportY.Max(v => v);

                var start = strw.StageFrom;
                var end = strw.StageTo*building.HeightStage;
                list.Add(new Cube(start, end, xMin, xMax, yMin, yMax));
            }

            var stream = new MemoryStream();
            var ser = new DataContractJsonSerializer(typeof(List<Cube>));
            ser.WriteObject(stream, list);
            stream.Position = 0;
            var sr = new StreamReader(stream, Encoding.UTF8);
            var read = sr.ReadToEnd();
            var sw = new StreamWriter(fileName);
            sw.Write(read); 
            stream.Close();
        }
    }
}
