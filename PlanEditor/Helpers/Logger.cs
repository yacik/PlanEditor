using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanEditor
{
    public class Logger
    {
        private static StreamWriter file = new StreamWriter("C:\\Users\\Andrey\\Desktop\\log.txt");
        
        public Logger()
        {             
        }

        public static void WriteLn(string s)
        {
            file.Write(s + "\n");
        }
        
        public static void Write(string s)
        {
            file.Write(s);
        }
        
        public static void Close()
        {
            file.Close();
        }
    }
}
