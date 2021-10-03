using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
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

            pointService.PointDistanceAwayByAngle(
                point: new PointF(8, 4),
                angle: 0,
                distance: 5
            )
                .PrintLn("PointDistanceAwayByAngle angle: 0");
            pointService.PointDistanceAwayByAngle(
                point: new PointF(8, 4),
                angle: 45,
                distance: 5
            )
                .PrintLn("PointDistanceAwayByAngle angle: 45");
            pointService.PointDistanceAwayByAngle(
                point: new PointF(8, 4),
                angle: 90,
                distance: 5
            )
                .PrintLn("PointDistanceAwayByAngle angle: 90");
            pointService.PointDistanceAwayByAngle(
                point: new PointF(8, 4),
                angle: 180,
                distance: 5
            )
                .PrintLn("PointDistanceAwayByAngle angle: 180");

            Console.ReadLine();
        }
    }
}
