using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace PlanEditor.Helpers
{
    public class PELogger
    {
        private static PELog _logger;

        public static PELog GetLogger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = new PELog();
                    return _logger;
                }
                
                return _logger;
            }
        }
    }

    public class PELog
    {
        private StreamWriter File { get; set; }

        public PELog()
        {
            var fileName = Environment.CurrentDirectory;
            fileName += @"\log.txt";
            //const string fileName = @"C:\Users\Default\AppData\Local\log.txt";

            File = new StreamWriter(fileName);
        }

        public void WriteLn(string errorMessage, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null, [CallerFilePath] string file = null)
        {
            File.Write(file + " " + caller + " " + lineNumber + " " + errorMessage + "\n");
        }

        public void Write(string errorMessage, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null, [CallerFilePath] string file = null)
        {
            File.Write(file + " " + caller + " " + lineNumber + " " + errorMessage);
        }

        public void Close()
        {
            File.Close();
        }
    }
}
