using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Rekurencjon
{
    internal class Program
    {
        private const string ULTIMATE_CHANGER_PATH = @"C:\Program Files\UltimateChanger\";
        private const string ULTIMATE_CHANGER_DATA_PATH = @"C:\Program Files\UltimateChanger\Data\";
        private const string COMPOSITION_PATH = @"\\demant.com\data\KBN\RnD\SWS\Build\Arizona\Phoenix\Nightly-";

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;

        private static List<PathAndDir> GetListOfNightlyPaths(string rootPath, string release, string find) // pobieram liste paths do exe // find - MASTER albo RC
        {
            var listOfNightlyPaths_Master = new List<string>();
            var listAllPathsAndDir = new List<PathAndDir>();
            var directories = new List<string>();
            try
            {
               directories = Directory.GetDirectories(rootPath + release).ToList();
            }
            catch (Exception x)
            {
                Console.WriteLine(x.ToString() + " \nat: " + rootPath + release);
                Console.ReadKey();
            }

            var findString = find == "RC" ? "-rc-" : "Nightly-master";

            foreach (var item in directories)
            {
                if (item.Contains(findString))
                {
                    listOfNightlyPaths_Master.Add(item);
                }
            }
            var tmp = new PathAndDir();

            foreach (var item in listOfNightlyPaths_Master)
            {
                if (!Directory.Exists(item + "\\DevResults-" + release)) continue;
                tmp.path.Add(item);
                tmp.dir.Add(new DirectoryInfo(item).Name + " " + Directory.GetCreationTime(item));
            }

            tmp.path.Reverse();
            tmp.dir.Reverse();

            // TODO why 50 ???
            var tmp2 = tmp.path.Take(50);
            var tmp3 = tmp.dir.Take(50);

            listAllPathsAndDir.Add(new PathAndDir { path = tmp2.ToList(), dir = tmp3.ToList() });

            return listAllPathsAndDir;
        }


        private static List<PathAndDir> GetListOfFullPaths(string rootPath) // pobieram liste paths do exe
        {
            var IPs_paths = new List<string>();
            var directoriesFull = new List<string>();
            var directoriesMedium = new List<string>();
            try
            {
                if (Directory.Exists(rootPath))
                {
                    IPs_paths = Directory.GetDirectories(rootPath).ToList(); // tu zawarte sa paths do IPs

                    //directories = Directory.GetDirectories(RootPath,"setup.exe",SearchOption.AllDirectories).ToList();
                    directoriesFull = Directory.GetFiles(rootPath, "setup.exe", SearchOption.AllDirectories).ToList();
                    var EXEs = Directory.GetFiles(rootPath, "*.exe", SearchOption.AllDirectories).ToList();
                    directoriesMedium = EXEs.FindAll(s => s.Contains("Medium"));
                }
                else
                {
                    return null;
                }
            }
            catch (Exception x)
            {
                Console.WriteLine(x.ToString() + " \nat: " + rootPath);
                Console.ReadKey();
            }
            directoriesFull.AddRange(directoriesMedium);

            return new List<PathAndDir>(){ new PathAndDir { path = IPs_paths, dir = directoriesFull } };
        }

        public static void SaveBuildsInfo(string pathFile, string dirFile, PathAndDir dirsAndPaths) // pathFile - name file to save DIRS // dirFile - name file to save Paths, // disAndPaths - object contain list path and dirs to save
        {
            try
            {
                if (!Directory.Exists(ULTIMATE_CHANGER_DATA_PATH))
                {
                    if (!Directory.Exists(ULTIMATE_CHANGER_PATH))
                    {
                        Directory.CreateDirectory(ULTIMATE_CHANGER_PATH);
                        Directory.CreateDirectory(ULTIMATE_CHANGER_DATA_PATH);
                    }
                    else
                    {
                        Directory.CreateDirectory(ULTIMATE_CHANGER_DATA_PATH);
                    }
                }
                if (dirsAndPaths != null)
                {
                    using (var outputFile = new StreamWriter(ULTIMATE_CHANGER_DATA_PATH + dirFile))
                    {
                        foreach (var line in dirsAndPaths.dir)
                            outputFile.WriteLine(line);

                        outputFile.Close();
                    }

                    using (var outputFile = new StreamWriter(ULTIMATE_CHANGER_DATA_PATH + pathFile))
                    {

                        foreach (var line in dirsAndPaths.path)
                            outputFile.WriteLine(line);

                        outputFile.Close();
                    }
                }
                else
                {
                    using (var outputFile = new StreamWriter(ULTIMATE_CHANGER_DATA_PATH + dirFile))
                    {
                        outputFile.WriteLine("");
                        outputFile.Close();
                    }

                    using (var outputFile = new StreamWriter(ULTIMATE_CHANGER_DATA_PATH + pathFile))
                    {
                        outputFile.WriteLine("");
                        outputFile.Close();
                    }
                }
            }
            catch (Exception x)
            {
                //  MessageBox.Show("cannot write to file");
            }
        }
        
        private static void GetPathsOfFull(string name, string release, string version, Log logging)
        {
            var pathToFull = $"\\\\demant.com\\data\\KBN\\RnD\\FS_Programs\\Fitting Applications\\{name}\\20{release}\\{version}";
            var paths = GetListOfFullPaths(pathToFull);
            try
            {
                SaveBuildsInfo($"{name}_dir.txt", $"{name}_path.txt", paths[0]);
            }
            catch (Exception x)
            {
                logging.AddLog("Full : " + x.ToString());
                Console.WriteLine(x.ToString());
            }
        }

        private static void Main(string[] args)
        {
            var logging = new Log("Rekurencjon");

            var buildType = args[0];    // Full/Medium/Composition/Copy
            var release = args[1];      // release
            var pathFile = args[2];     // pathFile - only file name
            var dirFile = args[3];      // dirFile - only file name "test.txt" // full path
            var branch = args[4];       // RC/MASTER

            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);  // Hide

            logging.AddLog("Start as : " + buildType);


            switch(buildType)
            {
                case "Composition":
                    List<PathAndDir> tmp2;
                    try
                    {
                        if (Directory.Exists(COMPOSITION_PATH + release))
                        {
                            tmp2 = GetListOfNightlyPaths(COMPOSITION_PATH, release, branch);
                        }
                        else
                            break;
                    }
                    catch (Exception)
                    {
                        tmp2 = GetListOfNightlyPaths(COMPOSITION_PATH, "19.1", "");
                    }

                    SaveBuildsInfo(pathFile, dirFile, tmp2[0]);
                    Console.WriteLine((args[2]));
                    Console.WriteLine((args[3]));
                    break;

                case "Full":
                   
                    // 2 ściezki do release and pre-release
                    // config, baza na ściezki
                    if (Directory.Exists($"\\\\demant.com\\data\\KBN\\RnD\\FS_Programs\\Fitting Applications\\Genie\\20{release}\\Released"))
                    {
                        GetPathsOfFull("Genie", release, "Released", logging);
                        GetPathsOfFull("GenieMedical", release, "Released", logging);
                        GetPathsOfFull("Oasis", release, "Released", logging);
                        GetPathsOfFull("HearSuite", release, "Released", logging);
                        GetPathsOfFull("ExpressFit", release, "Released", logging);
                    }
                    else
                    {
                        GetPathsOfFull("Genie", release, "Pre-releases", logging);
                        GetPathsOfFull("GenieMedical", release, "Pre-releases", logging);
                        GetPathsOfFull("Oasis", release, "Pre-releases", logging);
                        GetPathsOfFull("HearSuite", release, "Pre-releases", logging);
                        GetPathsOfFull("ExpressFit", release, "Pre-releases", logging);
                    }
                    break;

                // TODO copy?? what does it do? is it used?
                case "Copy":
                    //Copy:
                    // args 1 from
                    // args 2 to
                    // args 3 name of file

                    // args 0 Copy
                    // args 1 from
                    // args 2 to
                    try
                    {
                        logging.AddLog("args.Length : " + args.Length.ToString());

                        for (int i = 0; i < args.Length; i++)
                        {
                            logging.AddLog($"args[{i}] : " + args[i].ToString());
                        }

                        if (args.Length == 4)
                        {
                            File.Copy(release, pathFile + " " + dirFile);                         
                        }
                        else
                        {
                            File.Copy(release, pathFile);
                        }
                    }
                    catch (Exception x)
                    {
                        logging.AddLog(x.ToString());
                        Console.WriteLine(x.ToString());
                        Console.ReadKey();
                    }
                    break;
            }
            // tmp.Savebuildsinfo($"Oticon_path_Composition.txt", $"Oticon_dir_Composition.txt", tmp2[0]);
        }
    }
}
