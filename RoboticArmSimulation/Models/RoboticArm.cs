using HelixToolkit.Wpf;
using RoboticArmSimulation.Kinematics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace RoboticArmSimulation.Models
{
    public class RoboticArm : INotifyPropertyChanged
    {
        private List<Link> links;
        private Model3DGroup model;
        private Model3DGroup targetPointModel;

        public Model3DGroup Model { get => model; }
        public Model3DGroup TargetPointModel { get => targetPointModel; }

        public RoboticArm()
        {
            links = new List<Link>();
            model = new Model3DGroup();
        }

        public void AddLink(Link link)
        {
            links.Add(link);
        }

        public Link GetLink(int idx)
        {
            return links[idx];
        }

        public void RemoveLink(int idx)
        {
            links.RemoveAt(idx);
        }

        public void RemoveLastLink()
        {
            links.RemoveAt(links.Count - 1);
        }

        public void RemoveFirstLink()
        {
            if (links.Count > 0)
                links.RemoveAt(0);
        }

        public void Clear()
        {
            links.Clear();
        }

        public int GetLinksCount()
        {
            return links.Count;
        }

        public void Displace(int linkNum, double displace)
        {
            if (linkNum < links.Count)
            {
                Link link = links[linkNum];
                MDHParameters parameters = link.Parameters;
                
                if (parameters.LinkType == LinkType.REVOLUTE)
                {
                    parameters.Theta += displace;
                }
                else if (parameters.LinkType == LinkType.PRISMATIC)
                {
                    parameters.D += displace;
                }
            }
        }

        public void RenderTarget(Point3D target)
        {
            MeshBuilder builder = new MeshBuilder();
            builder.AddSphere(target, 0.6);

            Model3DGroup group = new Model3DGroup();
            group.Children.Add(new GeometryModel3D()
            {
                Geometry = builder.ToMesh(),
                Material = MaterialHelper.CreateMaterial(Colors.Red),
                BackMaterial = MaterialHelper.CreateMaterial(Colors.Red)
            });
            targetPointModel = group;

            NotifyPropertyChanged("TargetPointModel");
        }

        public void Render()
        {
            MeshBuilder builder = new MeshBuilder();

            var parameters = links.Select(link => link.Parameters).ToList();
            var positions = RoboticMath.GetPositions(parameters);
            //var transforms = parameters.Select(parameter => RoboticMath.GetTransformMatrix(parameter)).ToList();

            double[] prev = null;
            foreach (var pose in positions)
            {
                builder.AddSphere(new Point3D(pose[0], pose[1], pose[2]), 0.5);
                
                if (prev != null)
                {
                    builder.AddCylinder(
                        new Point3D(prev[0], prev[1], prev[2]),
                        new Point3D(pose[0], pose[1], pose[2]),
                        0.25);
                }

                prev = pose;
            }

            Model3DGroup group = new Model3DGroup();
            group.Children.Add(new GeometryModel3D()
            {
                Geometry = builder.ToMesh(),
                Material = MaterialHelper.CreateMaterial(Colors.Green),
                BackMaterial = MaterialHelper.CreateMaterial(Colors.Green)
            });
            model = group;

            NotifyPropertyChanged("Model");
        }

        public List<MDHParameters> GetParameters()
        {
            return links.Select(link => link.Parameters).ToList();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string info = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
