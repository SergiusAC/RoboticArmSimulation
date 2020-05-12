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
    /// Логика взаимодействия для LinkControl.xaml
    /// </summary>
    public partial class LinkControl : UserControl
    {
        public int LinkNum { get; set; }

        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
            "ValueChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(LinkControl));

        public event RoutedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        public LinkControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void RaiseValueChangedEvent()
        {
            RoutedEventArgs args = new RoutedEventArgs(ValueChangedEvent);
            args.Source = this;
            RaiseEvent(args);
        }

        private void JointControl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RaiseValueChangedEvent();
        }
    }
}
