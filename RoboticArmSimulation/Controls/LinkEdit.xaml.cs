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
    /// Логика взаимодействия для LinkEdit.xaml
    /// </summary>
    public partial class LinkEdit : UserControl
    {
        public string LinkNum { get; set; }
        public int Alpha { get; set; }
        public int A { get; set; }
        public int Theta { get; set; }
        public int D { get; set; }

        public LinkEdit()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
