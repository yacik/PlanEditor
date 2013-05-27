using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PlanEditor.Helpers.IO
{
    public class SaveFile
    {
        public static void Save(string fileName, Entities.Building building)
        {
            using (var fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
            {
                var bf = new BinaryFormatter();
                building.PrepareForExport();
                bf.Serialize(fs, building);
                fs.Close();
            }
        }
    }
}
