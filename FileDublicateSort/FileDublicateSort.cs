﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Diagnostics;

namespace FileDublicateSort
{
    class FileDublicateSort         // класс сортирующий файлы 
    {

        FileTree fileTreeSort;      // класс создающий дерево каталогов и файлов при сортировке


        // колекции для сортировки файлов
        List<string> fileCopies = new List<string>();
        SortedSet<string> fileOriginals = new SortedSet<string>();
        SortedSet<string> fileContent = new SortedSet<string>();

        private string DirName;

        XmlSerializer xmlSerializer = new XmlSerializer(typeof(FileTree));

        public FileDublicateSort(string dirName)
        {
            Stopwatch sw = new Stopwatch();
            DirName = dirName;
            Directory.CreateDirectory(DirName + @"\COPY");
            fileTreeSort = new FileTree(DirName);
            sw.Start();
            DublicateSort(DirName, fileTreeSort);
            sw.Stop();
            Console.WriteLine($"Sort time: {sw.ElapsedMilliseconds/10}");
            serializeFileLists();
        }

        public void ShowFinalReport()
        {
            Console.WriteLine($"Original : {fileOriginals.Count} files");
            Console.WriteLine($"Copies : {fileCopies.Count} files");
        }

        public void DublicateSort(string dirName, FileTree fileTree)        //метод сортировки
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

                    if (fileOriginals.Contains(f.Name))
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

        private void MoveCopyFile(FileInfo file)  // перемещает копии(при совпадении имен добавляет число перед названием)
        {
            string pref = "";
            int i = 0;
            while (true)
            {
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

        protected void serializeFileLists()         // сериализация дерева в xml
        {
            using (FileStream fs = new FileStream(DirName + @"\fileInfo.xml", FileMode.Create))
            {
                Console.WriteLine("Serialization done.");
                xmlSerializer.Serialize(fs, fileTreeSort);
            }
        }

    }
}
