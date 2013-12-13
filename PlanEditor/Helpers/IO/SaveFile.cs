/*
 * Сохраняет файл во временный файл-проект
 */

using System;
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
                try
                {
                    var bf = new BinaryFormatter();
                    building.PrepareForExport();
                    bf.Serialize(fs, building);
                    fs.Close();
                }
                catch (Exception ex)
                {
                    PELogger.GetLogger.WriteLn(ex.Message); 
                }
            }
        }
    }
}
