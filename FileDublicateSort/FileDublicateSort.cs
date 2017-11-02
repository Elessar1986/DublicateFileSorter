﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Security.Cryptography;
using System.Diagnostics;

namespace FileDublicateSort
{
    class FileDublicateSort         // класс сортирующий файлы 
    {

        FileTree fileTreeSort;      // класс создающий дерево каталогов и файлов при сортировке


        // колекции для сортировки файлов
        SortedDictionary<string, string> fileOriginals = new SortedDictionary<string, string>();
        private static int Copies = 0;
        
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
            Console.WriteLine($"All sorted files: {fileOriginals.Count + Copies}");
            Console.WriteLine($"Original : {fileOriginals.Count} files");
            Console.WriteLine($"Copies : {Copies} files");
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
                    string hash = GetMD5Hash(f.FullName);
                    if (!fileOriginals.Values.Contains(f.Name) && !fileOriginals.Keys.Contains(hash))
                    {
                        fileOriginals.Add(hash, f.Name);
                        fileTree.files.Add(f.Name);
                    }
                    else
                    {
                        MoveCopyFile(f);
                        Copies++;
                    }
                }
            }

        }

        private string GetMD5Hash(string file)
        {
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] fileBytes = new byte[fs.Length];
                fs.Read(fileBytes, 0, (int)fs.Length);
                byte[] hash = md5.ComputeHash(fileBytes);
                return BitConverter.ToString(hash).Replace("-",String.Empty);
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
