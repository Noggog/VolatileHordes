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
            var pointService = new PointService(new Randomization.RandomSource());
            pointService.RandomlyAdjustAngle(10, 45)
                .PrintLn("RandomlyAdjustAngle 10, 45");
            pointService.RandomlyAdjustAngle(10, 45)
                .PrintLn("RandomlyAdjustAngle 10, 45");
            pointService.RandomlyAdjustAngle(10, 45)
                .PrintLn("RandomlyAdjustAngle 10, 45");
            pointService.RandomlyAdjustAngle(10, 45)
                .PrintLn("RandomlyAdjustAngle 10, 45");

            // pointService.PointDistanceAway();

            Console.ReadLine();
        }
    }
}
