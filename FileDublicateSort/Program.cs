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
    public class Files
    {
        public List<string> fileCopies = new List<string>();
        public List<string> fileOriginals = new List<string>();

    }


    class FileDublicateSort
    {
        //SortedSet<string> fileNames = new SortedSet<string>();
        SortedSet<string> fileContent = new SortedSet<string>();
        Files sortedFiles = new Files();
        private string DirName;

        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Files));

        public FileDublicateSort(string dirName)
        {
            DirName = dirName;
            Directory.CreateDirectory(dirName + @"\COPY");
            DublicateSort(DirName);
        }

        public void ShowOriginalFiles()
        {
            Console.WriteLine($"Original files ({sortedFiles.fileOriginals.Count} files): \n");
            //foreach (var f in sortedFiles.fileOriginals)
            //{
            //    Console.WriteLine(f);
            //}
        }

        public void ShowCopies()
        {
            Console.WriteLine($"Copies files ({sortedFiles.fileCopies.Count} files): \n");
            //foreach (var f in sortedFiles.fileCopies)
            //{
            //    Console.WriteLine(f);
            //}
        }

        public void DublicateSort(string dirName)
        {
            DirectoryInfo dir = new DirectoryInfo(dirName);
            List<DirectoryInfo> dirs = dir.GetDirectories().ToList();
            if (dirs.Count != 0)
            {
                foreach (var d in dirs)
                {
                    DublicateSort(d.FullName);
                }
            }
            List<FileInfo> files = dir.GetFiles().ToList();
            if (files.Count != 0)
            {
                foreach (var f in files)
                {
                    if (sortedFiles.fileOriginals.Contains(f.Name))
                    {
                        //if (!sortedFiles.fileCopies.Contains(f.Name))
                            sortedFiles.fileCopies.Add(f.Name);
                            
                    }
                    else
                    {
                        using (StreamReader fs = new StreamReader(f.FullName))
                        {
                            if (fileContent.Add(fs.ReadToEnd()))
                            {
                                sortedFiles.fileOriginals.Add(f.Name);
                            }
                            else
                            {
                                sortedFiles.fileCopies.Add(f.Name);
                            }
                        }
                        
                    }
                }
            }
            
        }

        protected void serializeFileLists()
        {

        }

    }


    class Program
    {
        static void Main(string[] args)
        {
            FileDublicateSort fds = new FileDublicateSort(@"E:\Programming\testSortingFiles");
            fds.ShowOriginalFiles();
            fds.ShowCopies();
            Console.ReadKey();
        }

    }
}
