using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rekurencjon
{
    class Log
    {
        public Log( string name)
        {
            CreateNewLog(name);
        }

        public const string LOG_PATH = @"..\Logs\";

        public FileStream LogFile;
        public string pathToFile;
        public void CreateNewLog(string name)
        {
            if (!Directory.Exists(LOG_PATH))
            {
                Directory.CreateDirectory(LOG_PATH);
            }
            pathToFile = LOG_PATH + $"Log_{name}_{DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss")}.txt";
            LogFile = File.Create(pathToFile);
            LogFile.Close();
        }

        public void AddLog(string log)
        {
            File.AppendAllText(pathToFile, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\t" + log + Environment.NewLine);
        }
    }
}
