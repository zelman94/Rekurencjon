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
        static public List<string> listPathTobuilds = new List<string> {


            @"\\10.128.3.1\DFS_Data_SSC_FS_GenieBuilds\Phoenix\Genie", // po kolei jak w cmb FS
            @"\\10.128.3.1\DFS_Data_SSC_FS_GenieBuilds\Phoenix\Oasis",
            @"\\10.128.3.1\DFS_Data_SSC_FS_GenieBuilds\Phoenix\ExpressFit",
            @"", // medical
            @"", //cumulus
            @"\\10.128.3.1\DFS_Data_KBN_RnD_FS_Programs\Fitting Applications\Genie\20",
            @"\\10.128.3.1\DFS_Data_KBN_RnD_FS_Programs\Fitting Applications\Oasis\20",
            @"\\10.128.3.1\DFS_Data_KBN_RnD_FS_Programs\Fitting Applications\ExpressFit\20",
            @"\\10.128.3.1\DFS_Data_KBN_RnD_FS_Programs\Fitting Applications\GenieMedical\20",
            @"\\10.128.3.1\DFS_Data_KBN_RnD_FS_Programs\Fitting Applications\Cumulus\20"

        };

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
        private List<pathAndDir> Paths_Dirs = new List<pathAndDir>();

        public void Savebuildsinfo()
        {
            List<pathAndDir> tmpList = Paths_Dirs;


            int j = 1;
            int k = 0;
            for (int i = 0; i < tmpList.Count; i++)
            {
                try
                {
                    //File.Delete(@"C:\Program Files\DGS - PAZE & MIBW\Data\" + listFilesName[k]);
                    //File.Delete(@"C:\Program Files\DGS - PAZE & MIBW\Data\" + listFilesName[j]);

                    //File.Create(@"C:\Program Files\DGS - PAZE & MIBW\Data\" + listFilesName[k]);
                    //File.Create(@"C:\Program Files\DGS - PAZE & MIBW\Data\" + listFilesName[j]);



                    using (StreamWriter outputFile = new StreamWriter(@"C:\Program Files\DGS - PAZE & MIBW\Data\" + listFilesName[k]))
                    {
                        foreach (string line in tmpList[i].dir)
                            outputFile.WriteLine(line);

                        outputFile.Close();
                    }

                    using (StreamWriter outputFile = new StreamWriter(@"C:\Program Files\DGS - PAZE & MIBW\Data\" + listFilesName[j]))
                    {
                        foreach (string line in tmpList[i].path)
                            outputFile.WriteLine(line);

                        outputFile.Close();
                    }


                    //System.IO.File.WriteAllLines(@"C:\Program Files\DGS - PAZE & MIBW\Data\" + listFilesName[k], tmpList[i].dir);
                    //System.IO.File.WriteAllLines(@"C:\Program Files\DGS - PAZE & MIBW\Data\" + listFilesName[j], tmpList[i].path);



                    j += 2; ;
                    k += 2; ;
                }
                catch (Exception x)
                {
                  //  MessageBox.Show("cannot write to file");
                }
            }

        }


        private pathAndDir GetBindDirNames(string path, List<string> exenames, pathAndDir tmp, int nr)
        {

            List<string> dir = null;
            bool flag = false;
            int dl;
            try
            {
                dir = System.IO.Directory.GetDirectories(path).ToList<string>();

                var tmp2 = dir.Skip(Math.Max(0, dir.Count() - 20));

                //var firstItems = dir.OrderBy(q => q).Take(20);
                dir = tmp2.ToList<string>();
            }
            catch (Exception)
            {

            }



            if (nr == 0 && dir != null)
            {
                foreach (var item in dir)
                {
                    tmp.dir.Add(new DirectoryInfo(item).Name);
                }
            }

            if (dir != null)
            {
                foreach (var item in dir)
                {
                    GetBindDirNames($"{item}", exenames, tmp, ++nr);
                    List<string> pliczki = System.IO.Directory.GetFiles(item).ToList<string>();

                    foreach (var item2 in exenames)
                    {
                        foreach (var item3 in pliczki)
                        {
                            dl = item3.Length - 1;

                            string tmp_item3 = item3.Substring(dl - item2.Length);
                            if (tmp_item3 == ("\\" + item2))
                            {
                                tmp.path.Add(item3);
                                flag = true;
                                break;
                            }
                        }
                        if (flag)
                        {
                            flag = false;
                            break;
                        }
                    }
                }

            }

            return tmp;

        }

        public List<pathAndDir> getAllDirPath(string release) // pobieram wszystkie sciezki i dir z path i podmieniam w glownym pliku 
        {
            List<pathAndDir> lista2 = new List<pathAndDir>();


            foreach (var item in listPathTobuilds)
            {
                pathAndDir tmp = new pathAndDir();
                //licznik_przejsc++;
                if (item.Contains("FS_Programs"))
                {

                    lista2.Add(GetBindDirNames(item + release + @"\Pre-releases", listExeFiles, tmp, 0));
                }
                else
                    lista2.Add(GetBindDirNames(item, listExeFiles, tmp, 0));


                //((MainWindow)System.Windows.Application.Current.MainWindow).progress.Value = ((MainWindow)System.Windows.Application.Current.MainWindow).progress.Value + (100 / listPathTobuilds.Count);
            }

            return lista2;

        }

        public void makeProgress(string release)
        {
            List<pathAndDir> tmplistapathdir = new List<pathAndDir>();
            Console.WriteLine("watek sobie dziala :)");
            bool warunek = true;

            //List<pathAndDir> tmp = new List<pathAndDir>();
            try
            {
                tmplistapathdir = getAllDirPath(release);      // pobierac z argumentu wywolawczeggo program                     
            }
            catch (Exception x)
            {
               // MessageBox.Show(x.ToString());
            }


            Paths_Dirs = tmplistapathdir;


            Savebuildsinfo();



        }
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        static void Main(string[] args)
        {
            var handle = GetConsoleWindow();

            // Hide
            ShowWindow(handle, SW_HIDE);
            Program tmp = new Program();

            // Get command line arguments
            try
            {
                Console.WriteLine(args[0]);
                tmp.makeProgress(args[0]);
            }
            catch (Exception)
            {
                ShowWindow(handle, SW_SHOW);
                Console.WriteLine("Error in Rekurencjon.exe :C");
                Console.WriteLine("Press ESC to stop");
                do
                {
                    while (!Console.KeyAvailable)
                    {
                        // Do something
                    }
                } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            }

         
        }
    }
}
