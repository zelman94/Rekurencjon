using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rekurencjon
{
    public class PathsAndDirs
    {
        public List<string> Paths { get; set; }
        public List<string> Dirs { get; set; }

        public PathsAndDirs()
        {
            Paths = new List<string>();
            Dirs = new List<string>();
        }
    }
}
