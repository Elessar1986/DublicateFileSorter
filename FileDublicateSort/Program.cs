using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

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


    class FileDublicateSort
    {

        FileTree fileTreeSort;

        List<string> fileCopies = new List<string>();
        SortedSet<string> fileOriginals = new SortedSet<string>();
        SortedSet<string> fileContent = new SortedSet<string>();

        private string DirName;

        XmlSerializer xmlSerializer = new XmlSerializer(typeof(FileTree));

        public FileDublicateSort(string dirName)
        {
            DirName = dirName;
            Directory.CreateDirectory(DirName + @"\COPY");
            fileTreeSort = new FileTree(DirName);
            DublicateSort(DirName, fileTreeSort);
            serializeFileLists();
        }

        public void ShowFinalReport()
        {
            Console.WriteLine($"Original : {fileOriginals.Count} files");
            Console.WriteLine($"Copies : {fileCopies.Count} files");
        }

        public void DublicateSort(string dirName, FileTree fileTree)
        {
            DirectoryInfo dir = new DirectoryInfo(dirName);
            List<DirectoryInfo> dirs = dir.GetDirectories().ToList();
            if (dirs.Count != 0)
            {
                foreach (var d in dirs)
                {
                    fileTree.folders.Add(new FileTree(d.Name));
                    DublicateSort(d.FullName, fileTree.folders.Last());
                }
            }
            List<FileInfo> files = dir.GetFiles().ToList();
            if (files.Count != 0)
            {
                foreach (var f in files)
                {
                    
                    if (fileOriginals.Contains(f.Name) )
                    {
                        fileCopies.Add(f.Name);
                        MoveCopyFile(f);
                    }
                    else
                    {
                        using (StreamReader fs = new StreamReader(f.FullName))
                        {
                            if (fileContent.Add(fs.ReadToEnd()))
                            {
                                fileOriginals.Add(f.Name);
                                fileTree.files.Add(f.Name);
                            }
                            else
                            {
                                fileCopies.Add(f.Name);
                                fs.Close();
                                MoveCopyFile(f);
                                
                            }
                        }
                        
                    }
                }
            }
            
        }

        private void MoveCopyFile(FileInfo file)
        {
            string pref = "";
            int i = 0;
            while (true) {
                try
                {
                    file.MoveTo(DirName + @"\COPY\" + pref + file.Name);
                    return;
                }
                catch (Exception ex)
                {
                    pref = (i++).ToString();
                }
            }
        }

        protected void serializeFileLists()
        {
            using(FileStream fs = new FileStream(DirName + @"\fileInfo.xml", FileMode.Create))
            {
                Console.WriteLine("Serialization done.");
                xmlSerializer.Serialize(fs, fileTreeSort);
            }
        }

    }


    class Program
    {
        static void Main(string[] args)
        {
            FileDublicateSort fds = new FileDublicateSort(@"E:\Programming\testSortingFiles");
            fds.ShowFinalReport();

            Console.ReadKey();
        }

    }
}
