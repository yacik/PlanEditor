using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PlanEditor.Helpers.IO
{
    public class LoadFile
    {
        private readonly string _fileName;

        public LoadFile(string fileName, Entities.Building building)
        {
            _fileName = fileName;
            Building = building;
        }

        public bool Load()
        {
            using (var fs = new FileStream(_fileName, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    var bf = new BinaryFormatter();
                    Building = bf.Deserialize(fs) as Entities.Building;
                    fs.Close();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return false;
                }
                
            }

            if (Building == null) return false;
            
            for (int i = 0; i < Building.Stages; ++i)
            {
                if (Building.Places.Count > i)
                    foreach (var p in Building.Places[i])
                        p.LoadUI();
                if (Building.Portals.Count > i)
                    foreach (var v in Building.Portals[i])
                        v.LoadUI();
            }

            foreach (var v in Building.Mines)
                v.LoadUI();

            return true;
        }

        public Entities.Building Building { get; private set; }
    }
}
