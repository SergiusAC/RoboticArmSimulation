using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RoboticArmSimulation.Controls
{
    /// <summary>
    /// Логика взаимодействия для TargetEdit.xaml
    /// </summary>
    public partial class TargetEdit : UserControl
    {
        public double PointX { get; set; }
        public double PointY { get; set; }
        public double PointZ { get; set; }

        public TargetEdit()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
