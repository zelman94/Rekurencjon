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

        public FileStream LogFile;
        public string pathToFile;
        public void CreateNewLog(string name)
        {
            if (!Directory.Exists("C:\\Program Files\\UltimateChanger\\Logs"))
            {
                Directory.CreateDirectory("C:\\Program Files\\UltimateChanger\\Logs");
            }
            pathToFile = $"C:\\Program Files\\UltimateChanger\\Logs\\Log_{name}_{DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss")}.txt";
            LogFile = File.Create(pathToFile);
            LogFile.Close();


        }



        public void AddLog(string log)
        {
            File.AppendAllText(pathToFile, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\t" + log + Environment.NewLine);
        }


    }
}
