using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PS4000Lib;

namespace PS4000Lib.Example
{
    public class Program
    {
        static void Main()
        {
            var ps4000 = new PS4000();
            ps4000.Open();

            Console.ReadLine();
        }
    }
}
