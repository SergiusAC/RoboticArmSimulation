using Accord.Math;
using HelixToolkit.Wpf;
using RoboticArmSimulation.Controls;
using RoboticArmSimulation.Kinematics;
using RoboticArmSimulation.Models;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RoboticArmSimulation
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ModelVisual3D roboticArm = new ModelVisual3D();
        private static double[,] DHT =
        {
            { 0, 0, 0, 0 },
            {  -Math.PI / 2, 0, 0, 0 },
            { 0, 612.7, 0, 0 },
            { 0, 571.6, 0, 163.9 },
            { -Math.PI / 2, 0, 0, 115.7 },
            { Math.PI / 2, 0, Math.PI, 92.2 }
        };

        private int linkCount = 0;

        public MainWindow()
        {
            InitializeComponent();
            viewport3D.Children.Add(roboticArm);
        }

        private double DegreesToRadians(double degrees)
        {
            return Math.PI / 180 * degrees;
        }

        private double RadiansToDegrees(double radians)
        {
            return radians * 180 / Math.PI;
        }

        private List<MDHParameters> GetMDHParameters()
        {
            List<MDHParameters> parameters = new List<MDHParameters>();
            
            foreach (var element in linksList.Children)
            {
                var linkEdit = element as LinkEdit;
                parameters.Add(
                    new MDHParameters(
                        DegreesToRadians(linkEdit.Alpha), linkEdit.A, 
                        DegreesToRadians(linkEdit.Theta), linkEdit.D, 
                        LinkType.REVOLUTE
                        )
                    );
            }
            
            return parameters;
        }

        private void MakeRoboticArm()
        {
            RoboticArm arm = DataContext as RoboticArm;
            arm.Clear();

            var parameters = GetMDHParameters();
            foreach (var parameter in parameters)
            {
                arm.AddLink(new Link(parameter));
            }

            var tipPose = RoboticMath.GetPositionVector(RoboticMath.GetTransformMatrix(arm.GetParameters()));
            tipPosition.Text = tipPose[0] + ", " + tipPose[1] + ", " + tipPose[2];
        }

        private void MakeControls()
        {
            linkControls.Children.Clear();
            for (int i = 0; i < linksList.Children.Count; i++)
            {
                var newlinkControl = new LinkControl();
                newlinkControl.LinkNum = i;
                newlinkControl.ValueChanged += LinkControl_OnValueChanged;
                linkControls.Children.Add(newlinkControl);
            }
        }

        private void LinkControl_OnValueChanged(object sender, RoutedEventArgs e)
        {
            var source = e.Source as LinkControl;
            var arm = DataContext as RoboticArm;
            var jointControlValue = DegreesToRadians(source.JointControl.Value);
            double initValue;
            double currentValue;
            if (arm.GetLink(source.LinkNum).Parameters.LinkType == LinkType.REVOLUTE)
            {
                initValue = (linksList.Children[source.LinkNum] as LinkEdit).Theta;
                currentValue = arm.GetLink(source.LinkNum).Parameters.Theta;
            }
            else
            {
                initValue = (linksList.Children[source.LinkNum] as LinkEdit).D;
                currentValue = arm.GetLink(source.LinkNum).Parameters.D;
            }
            arm.Displace(source.LinkNum, -currentValue + initValue + jointControlValue);
            arm.Render();

            var tipPose = RoboticMath.GetPositionVector(RoboticMath.GetTransformMatrix(arm.GetParameters()));
            tipPosition.Text = tipPose[0] + ", " + tipPose[1] + ", " + tipPose[2];
        }

        private void removeLinkBtn_Click(object sender, RoutedEventArgs e)
        {
            if (linkCount > 0)
                linksList.Children.RemoveAt(--linkCount);
        }

        private void addLinkBtn_Click(object sender, RoutedEventArgs e)
        {
            var newLinkEdit = new LinkEdit();
            newLinkEdit.LinkNum = "Link " + linkCount++.ToString();
            linksList.Children.Add(newLinkEdit);
        }

        private void createArmBtn_Click(object sender, RoutedEventArgs e)
        {
            MakeRoboticArm();
            (DataContext as RoboticArm).Render();
            MakeControls();
        }

        private void SearchAngles_Click(object sender, RoutedEventArgs e)
        {
            double[] target = { targetEdit.PointX, targetEdit.PointY, targetEdit.PointZ };
            var dht = (DataContext as RoboticArm).GetParameters();

            bool success = false;
            double[] angles = RoboticMath.InverseKinematics(dht, target, ref success);

            searchedAngles.Children.Clear();
            
            if (success)
            {
                for (int i = 0; i < angles.Length; i++)
                {
                    var text = new TextBlock();
                    text.Text = String.Format("Angle {0}: {1:0.00}", i, (float)RadiansToDegrees(angles[i]));
                    searchedAngles.Children.Add(text);
                }
            }
            else
            {
                searchedAngles.Children.Add(new TextBlock() { Text = "Angles Not Found" });
            }
        }

        private void DrawTarget_Click(object sender, RoutedEventArgs e)
        {
            var targetPoint = new Point3D(targetEdit.PointX, targetEdit.PointY, targetEdit.PointZ);
            (DataContext as RoboticArm).RenderTarget(targetPoint);
        }
    }
}
