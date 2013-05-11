using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PlanEditor.IO
{
    public class SaveFile
    {
        private string m_fileName;
        private Entities.Building m_Building;

        public SaveFile(string fileName, Entities.Building building)
        {
            m_fileName = fileName;
            m_Building = building;
        }

        public void Save()
        {
            using (FileStream fs = new FileStream(m_fileName, FileMode.OpenOrCreate, FileAccess.Write))
            {
                BinaryFormatter bf = new BinaryFormatter();
                m_Building.PrepareForExport();
                bf.Serialize(fs, m_Building);
                fs.Close();
            }
        }
    }
}
