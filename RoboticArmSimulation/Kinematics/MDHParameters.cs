using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboticArmSimulation.Kinematics
{
    public class MDHParameters : ICloneable
    {
        public double Alpha { get; set; }
        public double A { get; set; }
        public double Theta { get; set; }
        public double D { get; set; }
        public LinkType LinkType { get; set; }

        public MDHParameters(double alpha, double a, double theta, double d, LinkType linkType)
        {
            Alpha = alpha;
            A = a;
            Theta = theta;
            D = d;
            LinkType = linkType;
        }

        public object Clone()
        {
            return new MDHParameters(Alpha, A, Theta, D, LinkType);
        }
    }
}
