using HelixToolkit.Wpf;
using RoboticArmSimulation.Kinematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace RoboticArmSimulation.Models
{
    public class Link
    {
        public MDHParameters Parameters { get; set; }

        public Link(MDHParameters parameters)
        {
            Parameters = parameters;
        }
    }
}
