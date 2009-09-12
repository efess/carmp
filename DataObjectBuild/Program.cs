using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataObjectBuild
{
    class Program
    {
        static void Main(string[] args)
        {
            
            TableGen tg = new TableGen();
            tg.Generate();
            Console.WriteLine("Data objects built. Press any key to exit");
            Console.ReadKey();
            return;
        }
    }
}
