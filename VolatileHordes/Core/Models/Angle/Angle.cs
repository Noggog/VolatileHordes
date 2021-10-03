using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolatileHordes.Core.Models
{
    class Angle
    {
        /*
         * Internally, angle is always [AngleType.Degree]
         */
        private double angle;

        public Angle(double angle, AngleType angleType)
        {
            switch (angleType)
            {
                case AngleType.Radian:
                    this.angle = angle * 180 / Math.PI;
                    break;
                case AngleType.Degree:
                    this.angle = angle;
                    break;
                default:
                    throw new Exception($"Angle type does not exist:{angleType}");
            }
        }

        public double Degree
        {
            get { return angle; }
        }

        public double Radian
        {
            get { return angle * Math.PI / 180; }
        }
    }
}
