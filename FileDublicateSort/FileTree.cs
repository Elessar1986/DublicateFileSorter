using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDublicateSort
{

    [Serializable]
    public class FileTree
    {
        public string dirName;
        public List<FileTree> folders = new List<FileTree>();
        public List<string> files = new List<string>();

        public FileTree()
        {
        }

        public FileTree(string name)
        {
            dirName = name;
        }
    }
}
