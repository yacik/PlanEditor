using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace PlanEditor.IO
{
    public class LoadFile
    {
        private string m_fileName;
        private Entities.Building m_Building;

        public LoadFile(string fileName, Entities.Building building)
        {
            m_fileName = fileName;
            m_Building = building;
        }

        public void Load()
        {
            using (FileStream fs = new FileStream(m_fileName, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter bf = new BinaryFormatter();
                m_Building = bf.Deserialize(fs) as Entities.Building;
                fs.Close();
            }
                        
            for (int i = 0; i < m_Building.Stages + 1; ++i)
            {
                Debug.WriteLine(m_Building.Places[i].Count);
                foreach (var p in m_Building.Places[i])
                {
                    p.CreateUI();
                }
            }
        }

        public Entities.Building GetBuilding { get { return m_Building; } }
    }
}
