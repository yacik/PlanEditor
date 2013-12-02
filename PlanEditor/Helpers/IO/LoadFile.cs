using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using PlanEditor.Entities;

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
                    PELogger.GetLogger.WriteLn(ex.Message);
                    return false;
                }
            }

            if (Building == null) return false;

            for (int i = 0; i < Building.Stages; ++i)
            {
                if (Building.Places.Count > i)
                {
                    foreach (var p in Building.Places[i])
                    {
                        p.LoadUI();
                        p.Collisions = new List<Entity>();
                        if (p.Obstacles == null) p.Obstacles = new List<Obstacle>();
                        else
                        {
                            foreach (var obstacle in p.Obstacles) obstacle.LoadUI();
                        }
                    }
                }
                if (Building.Portals.Count > i)
                {
                    foreach (var v in Building.Portals[i])
                    {
                        v.LoadUI();
                    }
                }
            }

            if (Building.Stairways != null)
            {
                foreach (var v in Building.Stairways)
                {
                    if (v.Obstacles == null) v.Obstacles = new List<Obstacle>();
                    v.Collisions = new List<Entity>();
                    v.LoadUI();
                }
            }
            else
            {
                Building.Stairways = new List<Stairway>();
            }
            
            return true;
        }

        public Entities.Building Building { get; private set; }
    }
}
