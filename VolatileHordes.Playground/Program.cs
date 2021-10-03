using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolatileHordes.Randomization;
using VolatileHordes.Utility;

namespace VolatileHordes.Playground
{

    class Program
    {

        static void Main(string[] args)
        {
            var randomSource = new RandomSource();
            var pointService = new PointService(randomSource);
            pointService.RandomlyAdjustAngle(10, 45)
                .PrintLn("RandomlyAdjustAngle 10, 45");
            pointService.RandomlyAdjustAngle(10, 45)
                .PrintLn("RandomlyAdjustAngle 10, 45");
            pointService.RandomlyAdjustAngle(10, 45)
                .PrintLn("RandomlyAdjustAngle 10, 45");
            pointService.RandomlyAdjustAngle(10, 45)
                .PrintLn("RandomlyAdjustAngle 10, 45");

            //for (int i = 0; i < 100; i++)
            //{
            //    randomSource.NextBool()
            //        .PrintLn("NextBool");
            //}

            // pointService.PointDistanceAway();

            Console.ReadLine();
        }
    }
}
