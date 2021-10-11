using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using VolatileHordes.Probability;
using VolatileHordes.Utility;

namespace VolatileHordes.Playground
{

    class Program
    {

        static void Main(string[] args)
        {
            var randomSource = new RandomSource();
            var pointService = new PointService(randomSource);

            var point1 = new PointF(-2910.056f, -1633.051f);
            var point2 = new PointF(-2907.456f, -1632.18f);

            point1.PrintLn("point1");
            pointService.RandomlyAdjustPoint(
                point1,
                5
            )
                .PrintLn("aaa");
            pointService.RandomlyAdjustPoint(
                point1,
                5
            )
                .PrintLn("bbb");
            pointService.RandomlyAdjustPoint(
                point1,
                5
            )
                .PrintLn("ccc");

            (360 * randomSource.NextDouble()).PrintLn("ddd");
            (360 * randomSource.NextDouble()).PrintLn("eee");
            (360 * randomSource.NextDouble()).PrintLn("fff");
            (360 * randomSource.NextDouble()).PrintLn("ggg");
            (360 * randomSource.NextDouble()).PrintLn("hhh");
            (360 * randomSource.NextDouble()).PrintLn("iii");

            //pointService.AngleBetween(
            //    start: point1,
            //    end: point2
            //)
            //    .PrintLn("aaa");

            //pointService.PointDistanceAwayByAngle(
            //    point2,
            //    6.7,
            //    5
            //)
            //    .PrintLn("bbb");


            //pointService.PointDistanceAwayByAngle(
            //    point: new PointF(5, 5),
            //    angle: 90,
            //    distance: 5
            //)
            //    .PrintLn("aaa");

            //pointService.AngleBetween(
            //    start: new PointF(5, 5),
            //    end: new PointF(5, 10)
            //)
            //    .PrintLn("aaa");

            //pointService.PointDistanceAwayByAngle(
            //    point: new PointF(8, 4),
            //    angle: 0,
            //    distance: 5
            //)
            //    .PrintLn("PointDistanceAwayByAngle angle: 0");
            //pointService.PointDistanceAwayByAngle(
            //    point: new PointF(8, 4),
            //    angle: 45,
            //    distance: 5
            //)
            //    .PrintLn("PointDistanceAwayByAngle angle: 45");
            //pointService.PointDistanceAwayByAngle(
            //    point: new PointF(8, 4),
            //    angle: 90,
            //    distance: 5
            //)
            //    .PrintLn("PointDistanceAwayByAngle angle: 90");
            //pointService.PointDistanceAwayByAngle(
            //    point: new PointF(8, 4),
            //    angle: 180,
            //    distance: 5
            //)
            //    .PrintLn("PointDistanceAwayByAngle angle: 180");


            //2021 - 10 - 03T22: 11:36 475.295 INF[VolatileHordes] oldTarget: { X = -3164.685, Y = -1353.029}
            //2021 - 10 - 03T22: 11:36 475.295 INF[VolatileHordes] group.Target:{ X = -3168.046, Y = -1352.861}


            Console.ReadLine();
        }
    }
}
