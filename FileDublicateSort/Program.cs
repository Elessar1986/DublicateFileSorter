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
    class Program
    {
        static void Main(string[] args)
        {
            FileDublicateSort fds = new FileDublicateSort(Console.ReadLine());
            fds.ShowFinalReport();

            Console.ReadKey();
        }

    }
}
