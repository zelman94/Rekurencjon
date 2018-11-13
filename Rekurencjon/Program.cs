using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
namespace Rekurencjon
{

    class Program
    {


        private List<pathAndDir> Paths_Dirs = new List<pathAndDir>();
        public List<string> ListOfNightliPathsComposition = new List<string>(); // kompozycje
        public List<string> ListOfNightliPathsMedium = new List<string>(); //medium installers

        public bool IsDirectoryEmpty(string path) // czy jest pusty folder
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        private List<pathAndDir> GetListOfNightliPaths(string RootPath, string release, string find) // pobieram liste paths do exe // find - MASTER albo RC
        {
            List<string> ListOfNightliPaths_Master = new List<string>();
            List<string> ListOfNightliPaths = new List<string>();
            List<pathAndDir> listallpathsanddir = new List<pathAndDir>();
            List<string> directories = new List<string>();
            try
            {
               directories = Directory.GetDirectories(RootPath + release).ToList();
            }
            catch (Exception x)
            {
                Console.WriteLine(x.ToString() + " \nat: " + RootPath + release);
                Console.ReadKey();
            }
            string FindStirng = ""; 
            if (find == "RC")
            {
                FindStirng = "-rc-";
            }
            else
            {
                FindStirng = "Nightly-master";
            }

            foreach (var item in directories)
            {
                if (item.Contains(FindStirng) /*&& !item.Contains("SkipTest")*/)
                {
                    ListOfNightliPaths_Master.Add(item);
                }
            }
            pathAndDir tmp = new pathAndDir();

            foreach (var item in ListOfNightliPaths_Master)
            {
                if (/*!IsDirectoryEmpty(item) && */Directory.Exists(item + "\\DevResults-" + release)) // lezeli jest kompozycja to dodaje do listy 
                {
                    tmp.path.Add(item);
                    tmp.dir.Add(new DirectoryInfo(item).Name + " " + Directory.GetCreationTime(item));
                }
            }

            tmp.path.Reverse();
            tmp.dir.Reverse();


            var tmp2 = tmp.path.Take(50);
            var tmp3 = tmp.dir.Take(50);

            pathAndDir tmpinstance = new pathAndDir();

            tmpinstance.path = tmp2.ToList();
            tmpinstance.dir = tmp3.ToList();

            listallpathsanddir.Add(tmpinstance);

            return listallpathsanddir;
        }



        private List<pathAndDir> GetListOfFullPaths(string RootPath) // pobieram liste paths do exe
        {
            List<pathAndDir> listallpathsanddir = new List<pathAndDir>();

            List<string> ListOfNightliPaths_Master = new List<string>();
            List<string> ListOfNightliPaths = new List<string>();

            List<string> IPs_paths = new List<string>();
            List<string> directoriesFull = new List<string>();
            List<string> directoriesMedium = new List<string>();
            try
            {
                if (Directory.Exists(RootPath))
                {
                    IPs_paths = Directory.GetDirectories(RootPath).ToList(); // tu zawarte sa paths do IPs


                    //directories = Directory.GetDirectories(RootPath,"setup.exe",SearchOption.AllDirectories).ToList();
                    directoriesFull = Directory.GetFiles(RootPath, "setup.exe", SearchOption.AllDirectories).ToList();
                    var EXEs = Directory.GetFiles(RootPath, "*.exe", SearchOption.AllDirectories).ToList();
                    directoriesMedium = EXEs.FindAll(s => s.Contains("Medium"));
                }
                else
                {
                    return null;
                }
                



            }
            catch (Exception x)
            {
                Console.WriteLine(x.ToString() + " \nat: " + RootPath);
                Console.ReadKey();
            }


            directoriesFull.AddRange(directoriesMedium);

            pathAndDir tmpinstance = new pathAndDir();

            tmpinstance.path = IPs_paths;
            tmpinstance.dir = directoriesFull;

            listallpathsanddir.Add(tmpinstance);



            return listallpathsanddir;

        }


            public void Savebuildsinfo(string pathFile, string dirFile, pathAndDir dirsAndPaths) // pathFile - name file to save DIRS // dirFile - name file to save Paths, // disAndPaths - object contain list path and dirs to save
        {

            try
            {
                if (!Directory.Exists(@"C:\Program Files\UltimateChanger\Data\"))
                {
                    if (!Directory.Exists(@"C:\Program Files\UltimateChanger\"))
                    {
                        Directory.CreateDirectory(@"C:\Program Files\UltimateChanger\");
                        Directory.CreateDirectory(@"C:\Program Files\UltimateChanger\Data\");
                    }
                    else
                    {
                        Directory.CreateDirectory(@"C:\Program Files\UltimateChanger\Data\");
                    }
                }
                if (dirsAndPaths != null)
                {
                    using (StreamWriter outputFile = new StreamWriter(@"C:\Program Files\UltimateChanger\Data\" + dirFile))
                    {
                        foreach (string line in dirsAndPaths.dir)
                            outputFile.WriteLine(line);

                        outputFile.Close();
                    }

                    using (StreamWriter outputFile = new StreamWriter(@"C:\Program Files\UltimateChanger\Data\" + pathFile))
                    {

                        foreach (string line in dirsAndPaths.path)
                            outputFile.WriteLine(line);

                        outputFile.Close();
                    }
                }
                else
                {
                    using (StreamWriter outputFile = new StreamWriter(@"C:\Program Files\UltimateChanger\Data\" + dirFile))
                    {
                        
                            outputFile.WriteLine("");

                        outputFile.Close();
                    }

                    using (StreamWriter outputFile = new StreamWriter(@"C:\Program Files\UltimateChanger\Data\" + pathFile))
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


        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        static void Main(string[] args)
        {
                     Log logging = new Log("Rekurencjon");
        //Console.WriteLine((args[0]));
        //Console.WriteLine((args[1]));
        //Console.WriteLine((args[2]));

        // args 0 - Full/Medium/Composition/Copy
        // args 1 release
        // args 2 pathFile - only file name
        // args 3 dirFile - only file name "test.txt"
        // arg 4 - RC/MASTER


        //Copy:

        // args 1 from
        // args 2 to
        // args 3 name of file

        //Console.WriteLine((args[0]));

        //Console.WriteLine((args[1]));

        var handle = GetConsoleWindow();

            //// Hide
            ShowWindow(handle, SW_HIDE);
            Program tmp = new Program();


            string arg = "Copy";


            // Get command line arguments
            List<pathAndDir> tmp2;

            logging.AddLog("Start as : " + args[0]);

            switch (args[0])
            //switch (arg)
            {
                case "Composition":

                    


                    try
                    {
                        //if (Directory.Exists($"\\\\demant.com\\data\\KBN\\RnD\\SWS\\Build\\Arizona\\Phoenix\\Nightly-19.1"))
                            if (Directory.Exists($"\\\\demant.com\\data\\KBN\\RnD\\SWS\\Build\\Arizona\\Phoenix\\Nightly-{args[1]}"))
                        {
                            //tmp2 = tmp.GetListOfNightliPaths(@"\\demant.com\data\KBN\RnD\SWS\Build\Arizona\Phoenix\Nightly-", "19.1", "RC");

                            tmp2 = tmp.GetListOfNightliPaths(@"\\demant.com\data\KBN\RnD\SWS\Build\Arizona\Phoenix\Nightly-", args[1], args[4]);

                        }

                        else
                            return;
                    }
                    catch (Exception)
                    {
                        tmp2 = tmp.GetListOfNightliPaths(@"\\demant.com\data\KBN\RnD\SWS\Build\Arizona\Phoenix\Nightly-", "19.1", "");
                    }



                    // i * 2 = pierwsza nazwa pliku do zapisu // dir +1 = path  //listFilesName_Compositions


                    // args 0 - Full/Medium/Composition/Copy
                    // args 1 release
                    // args 2 pathFile - only file name
                    // args 3 dirFile - only file name "test.txt"
                    // arg 4 - RC/MASTER

                    tmp.Savebuildsinfo(args[2], args[3], tmp2[0]);
                    //tmp.Savebuildsinfo("path_compo_test.txt", "dir_compo_test.txt", tmp2[0]);


                    Console.WriteLine((args[2]));
                    Console.WriteLine((args[3]));

                    break;


                //path to installer \\demant.com\data\KBN\RnD\SWS\Build\Arizona\Phoenix\FullInstaller-19.1


                case "Full":
                   
                    if (!Directory.Exists($"\\\\demant.com\\data\\KBN\\RnD\\FS_Programs\\Fitting Applications\\Genie\\20{args[1]}\\Released")) // jezeli nie ma released
                    {
                        tmp2 = tmp.GetListOfFullPaths($"\\\\demant.com\\data\\KBN\\RnD\\FS_Programs\\Fitting Applications\\Genie\\20{args[1]}\\Pre-releases");
                        try
                        {
                            tmp.Savebuildsinfo("Genie_dir.txt", "Genie_path.txt", tmp2[0]);// zapis do pliku

                        }
                        catch (Exception x)
                        {
                            logging.AddLog("Full : " + x.ToString());
                            Console.WriteLine(x.ToString());
                        }



                        tmp2 = tmp.GetListOfFullPaths($"\\\\demant.com\\data\\KBN\\RnD\\FS_Programs\\Fitting Applications\\GenieMedical\\20{args[1]}\\Pre-releases");
                        try
                        {
                            tmp.Savebuildsinfo("GenieMedical_dir.txt", "GenieMedical_path.txt", tmp2[0]);// zapis do pliku
                        }
                        catch (Exception x)
                        {
                            logging.AddLog("Full : " + x.ToString());
                            Console.WriteLine(x.ToString());
                        }


                        tmp2 = tmp.GetListOfFullPaths($"\\\\demant.com\\data\\KBN\\RnD\\FS_Programs\\Fitting Applications\\Oasis\\20{args[1]}\\Pre-releases");
                        try
                        {
                            tmp.Savebuildsinfo("Oasis_dir.txt", "Oasis_path.txt", tmp2[0]);// zapis do plikuv
                        }
                        catch (Exception x)
                        {
                            logging.AddLog("Full : " + x.ToString());
                            Console.WriteLine(x.ToString());
                        }


                        tmp2 = tmp.GetListOfFullPaths($"\\\\demant.com\\data\\KBN\\RnD\\FS_Programs\\Fitting Applications\\HearSuite\\20{args[1]}\\Pre-releases");
                        try
                        {
                            tmp.Savebuildsinfo("Philips_dir.txt", "Philips_path.txt", tmp2[0]);// zapis do pliku

                        }
                        catch (Exception x)
                        {
                            logging.AddLog("Full : " + x.ToString());
                            Console.WriteLine(x.ToString());
                        }
                       

                        tmp2 = tmp.GetListOfFullPaths($"\\\\demant.com\\data\\KBN\\RnD\\FS_Programs\\Fitting Applications\\ExpressFit\\20{args[1]}\\Pre-releases");
                        try
                        {
                            tmp.Savebuildsinfo("ExpressFit_dir.txt", "ExpressFit_path.txt", tmp2[0]);// zapis do pliku
                        }
                        catch (Exception x)
                        {
                            logging.AddLog("Full : " + x.ToString());
                            Console.WriteLine(x.ToString());
                        }
                        
                    }
                    else
                    {
                        tmp2 = tmp.GetListOfFullPaths($"\\\\demant.com\\data\\KBN\\RnD\\FS_Programs\\Fitting Applications\\Genie\\20{args[1]}\\Released");
                        tmp.Savebuildsinfo("Genie_dir.txt", "Genie_path.txt",  tmp2[0]);// zapis do pliku

                        tmp2 = tmp.GetListOfFullPaths($"\\\\demant.com\\data\\KBN\\RnD\\FS_Programs\\Fitting Applications\\GenieMedical\\20{args[1]}\\Released");
                        tmp.Savebuildsinfo("GenieMedical_dir.txt", "GenieMedical_path.txt",  tmp2[0]);// zapis do pliku

                        tmp2 = tmp.GetListOfFullPaths($"\\\\demant.com\\data\\KBN\\RnD\\FS_Programs\\Fitting Applications\\Oasis\\20{args[1]}\\Released");
                        tmp.Savebuildsinfo("Oasis_dir.txt", "Oasis_path.txt",  tmp2[0]);// zapis do plikuv

                        tmp2 = tmp.GetListOfFullPaths($"\\\\demant.com\\data\\KBN\\RnD\\FS_Programs\\Fitting Applications\\HearSuite\\20{args[1]}\\Released");
                        tmp.Savebuildsinfo("Philips_dir.txt", "Philips_path.txt",  tmp2[0]);// zapis do pliku

                        tmp2 = tmp.GetListOfFullPaths($"\\\\demant.com\\data\\KBN\\RnD\\FS_Programs\\Fitting Applications\\ExpressFit\\20{args[1]}\\Released");
                        tmp.Savebuildsinfo("ExpressFit_dir.txt", "ExpressFit_path.txt",  tmp2[0]);// zapis do pliku
                    }


                   


                    break;


                case "Copy":

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
                            File.Copy(args[1], args[2] + " " + args[3]);                         
                        }
                        else
                        {
                            File.Copy(args[1], args[2]);
                        }
                        
                    }
                    catch (Exception x)
                    {
                        logging.AddLog(x.ToString());
                        Console.WriteLine(x.ToString());
                        Console.ReadKey();

                    }
                    break;

                default:
                    break;
            }

            // tmp.Savebuildsinfo($"Oticon_path_Composition.txt", $"Oticon_dir_Composition.txt", tmp2[0]);
            return;
        }
    }
}
