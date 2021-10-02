using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolatileHordes.Utility;

namespace VolatileHordes.Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(
                "result:{0}",
                new PointService(new Randomization.RandomSource()).RandomlyAdjustAngle(10, 45)
            );
            Console.ReadLine();
        }
    }
}
