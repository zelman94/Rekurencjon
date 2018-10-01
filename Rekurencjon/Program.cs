﻿using System;
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


        public List<string> listExeFiles = new List<string> {

            @"Setup.exe",
            @"EXPRESSfitMini.exe"// po kolei         
           // "Genie.exe"
        };

        public List<string> listFilesName = new List<string> {

            @"0Oticon_dir.txt", // 0FS_dir.txt
            @"0Oticon_path.txt", // 0FS_path.txt
            @"1Bernafon_dir.txt", // 0FS_dir.txt
            @"1Bernafon_path.txt",
            @"2Sonic_dir.txt", // 0FS_dir.txt
            @"2Sonic_path.txt",
            @"3GenieMedical_dir.txt", // 0FS_dir.txt
            @"3GenieMedical_path.txt",
            @"4Cumulus_dir.txt", // 0FS_dir.txt
            @"4Cumulus_path.txt",
            @"0Oticon_PRE_dir.txt", // 0FS_dir.txt
            @"0Oticon_PRE_path.txt", // 0FS_path.txt
            @"1Bernafon_PRE_dir.txt", // 0FS_dir.txt
            @"1Bernafon_PRE_path.txt",
            @"2Sonic_PRE_dir.txt", // 0FS_dir.txt
            @"2Sonic_PRE_path.txt",
            @"3GenieMedical_PRE_dir.txt", // 0FS_dir.txt
            @"3GenieMedical_PRE_path.txt",
            @"4Cumulus_PRE_dir.txt", // 0FS_dir.txt
            @"4Cumulus_PRE_path.txt"

        };

        public List<string> listFilesName_Compositions = new List<string> {

            @"0Oticon_dir_Compositions.txt", // 0FS_dir.txt
            @"0Oticon_path_Compositions.txt", // 0FS_path.txt
            @"1Bernafon_dir_Compositions.txt", // 0FS_dir.txt
            @"1Bernafon_path_Compositions.txt",
            @"2Sonic_dir_Compositions.txt", // 0FS_dir.txt
            @"2Sonic_path_Compositions.txt",
            @"3GenieMedical_dir_Compositions.txt", // 0FS_dir.txt
            @"3GenieMedical_path_Compositions.txt",
            @"4Cumulus_dir_Compositions.txt", // 0FS_dir.txt
            @"4Cumulus_path_Compositions.txt",

        };


        private List<pathAndDir> Paths_Dirs = new List<pathAndDir>();


        public List<string> ListOfNightliPathsComposition = new List<string>(); // kompozycje
        public List<string> ListOfNightliPathsMedium = new List<string>(); //medium installers

        public bool IsDirectoryEmpty(string path) // czy jest pusty folder
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        private List<pathAndDir> GetListOfNightliPaths(string RootPath, string release) // pobieram liste paths do exe
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
           

            foreach (var item in directories)
            {
                if (item.Contains("Nightly-master") /*&& !item.Contains("SkipTest")*/)
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
                IPs_paths = Directory.GetDirectories(RootPath ).ToList(); // tu zawarte sa paths do IPs


                //directories = Directory.GetDirectories(RootPath,"setup.exe",SearchOption.AllDirectories).ToList();
                directoriesFull = Directory.GetFiles(RootPath, "setup.exe", SearchOption.AllDirectories).ToList();
                var EXEs = Directory.GetFiles(RootPath, "*.exe" ,SearchOption.AllDirectories).ToList();
                directoriesMedium = EXEs.FindAll(s => s.Contains("Medium"));



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
            //Console.WriteLine((args[0]));
            //Console.WriteLine((args[1]));
            //Console.WriteLine((args[2]));




            // args 0 - Full/Medium/Composition/Copy
            // args 1 release
            // args 2 pathFile - only file name
            // args 3 dirFile - only file name "test.txt"


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





            // Get command line arguments
            List<pathAndDir> tmp2;
            switch (args[0])
            {
                case "Composition":

                    


                    try
                    {
                        tmp2 = tmp.GetListOfNightliPaths(@"\\demant.com\data\KBN\RnD\SWS\Build\Arizona\Phoenix\Nightly-", args[1]);
                    }
                    catch (Exception)
                    {
                        tmp2 = tmp.GetListOfNightliPaths(@"\\demant.com\data\KBN\RnD\SWS\Build\Arizona\Phoenix\Nightly-", "19.1");
                    }



                    // i * 2 = pierwsza nazwa pliku do zapisu // dir +1 = path  //listFilesName_Compositions

                    tmp.Savebuildsinfo(args[2], args[3], tmp2[0]);

                    Console.WriteLine((args[2]));
                    Console.WriteLine((args[3]));

                    break;


                //path to installer \\demant.com\data\KBN\RnD\SWS\Build\Arizona\Phoenix\FullInstaller-19.1


                case "Full":
                   
                    if (!Directory.Exists($"\\\\demant.com\\data\\KBN\\RnD\\FS_Programs\\Fitting Applications\\Genie\\20{args[1]}\\Released")) // jezeli nie ma released
                    {
                        tmp2 = tmp.GetListOfFullPaths($"\\\\demant.com\\data\\KBN\\RnD\\FS_Programs\\Fitting Applications\\Genie\\20{args[1]}\\Pre-releases");
                        tmp.Savebuildsinfo("Genie_dir.txt", "Genie_path.txt",  tmp2[0]);// zapis do pliku

                        tmp2 = tmp.GetListOfFullPaths($"\\\\demant.com\\data\\KBN\\RnD\\FS_Programs\\Fitting Applications\\GenieMedical\\20{args[1]}\\Pre-releases");
                        tmp.Savebuildsinfo("GenieMedical_dir.txt", "GenieMedical_path.txt",  tmp2[0]);// zapis do pliku

                        tmp2 = tmp.GetListOfFullPaths($"\\\\demant.com\\data\\KBN\\RnD\\FS_Programs\\Fitting Applications\\Oasis\\20{args[1]}\\Pre-releases");
                        tmp.Savebuildsinfo("Oasis_dir.txt", "Oasis_path.txt",  tmp2[0]);// zapis do plikuv

                        tmp2 = tmp.GetListOfFullPaths($"\\\\demant.com\\data\\KBN\\RnD\\FS_Programs\\Fitting Applications\\Philips\\20{args[1]}\\Pre-releases");
                        tmp.Savebuildsinfo("Philips_dir.txt", "Philips_path.txt",  tmp2[0]);// zapis do pliku

                        tmp2 = tmp.GetListOfFullPaths($"\\\\demant.com\\data\\KBN\\RnD\\FS_Programs\\Fitting Applications\\ExpressFit\\20{args[1]}\\Pre-releases");
                        tmp.Savebuildsinfo("ExpressFit_dir.txt", "ExpressFit_path.txt",  tmp2[0]);// zapis do pliku
                    }
                    else
                    {
                        tmp2 = tmp.GetListOfFullPaths($"\\\\demant.com\\data\\KBN\\RnD\\FS_Programs\\Fitting Applications\\Genie\\20{args[1]}\\Released");
                        tmp.Savebuildsinfo("Genie_dir.txt", "Genie_path.txt",  tmp2[0]);// zapis do pliku

                        tmp2 = tmp.GetListOfFullPaths($"\\\\demant.com\\data\\KBN\\RnD\\FS_Programs\\Fitting Applications\\GenieMedical\\20{args[1]}\\Released");
                        tmp.Savebuildsinfo("GenieMedical_dir.txt", "GenieMedical_path.txt",  tmp2[0]);// zapis do pliku

                        tmp2 = tmp.GetListOfFullPaths($"\\\\demant.com\\data\\KBN\\RnD\\FS_Programs\\Fitting Applications\\Oasis\\20{args[1]}\\Released");
                        tmp.Savebuildsinfo("Oasis_dir.txt", "Oasis_path.txt",  tmp2[0]);// zapis do plikuv

                        tmp2 = tmp.GetListOfFullPaths($"\\\\demant.com\\data\\KBN\\RnD\\FS_Programs\\Fitting Applications\\Philips\\20{args[1]}\\Released");
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
                        if (args.Length == 4 )
                        {
                            File.Copy(args[1], args[2]+ " " +args[3]);
                        }
                        else
                        File.Copy(args[1], args[2]);
                    }
                    catch (Exception x)
                    {
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
