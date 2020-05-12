using Accord.Math;
using Accord.Math.Optimization;
using Accord.Math.Differentiation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboticArmSimulation.Kinematics
{
    class RoboticMath
    {
        public static double[,] ForwardKinematics(List<MDHParameters> dht, List<double> displaces = null)
        {
            if (displaces != null && dht.Count != displaces.Count)
            {
                throw new ArgumentException();
            }

            List<double> saved = new List<double>();
            if (displaces != null)
            {
                for (int i = 0; i < dht.Count; i++)
                {
                    if (dht[i].LinkType == LinkType.REVOLUTE)
                    {
                        saved.Add(dht[i].Theta);
                        dht[i].Theta += displaces[i];
                    }
                    else if (dht[i].LinkType == LinkType.PRISMATIC)
                    {
                        saved.Add(dht[i].D);
                        dht[i].D += displaces[i];
                    }
                    else
                        throw new ArgumentException();
                }
            }

            var result = GetTransformMatrix(dht);

            // restore DHT
            if (displaces != null)
            {
                for (int i = 0; i < dht.Count; i++)
                {
                    if (dht[i].LinkType == LinkType.REVOLUTE)
                    {
                        dht[i].Theta = saved[i];
                    }
                    else if (dht[i].LinkType == LinkType.PRISMATIC)
                    {
                        dht[i].D = saved[i];
                    }
                }
            }

            return result;
        }

        public static double[] InverseKinematics(List<MDHParameters> dht, double[] target, ref bool success)
        {
            Func<double[], double> f = x => Distance(dht, target, x);
            var calculator = new FiniteDifferences(dht.Count, f);
            Func<double[], double[]> g = calculator.Gradient;
            
            var optimizer = new BroydenFletcherGoldfarbShanno(numberOfVariables: dht.Count, function: f, gradient: g);
            
            optimizer.Minimize();
            success = !(optimizer.Value > 0.01);

            return optimizer.Solution;
        }

        private static double Distance(List<MDHParameters> dht, double[] target, double[] angles)
        {
            var currentPose = GetPositionVector(ForwardKinematics(dht, angles.ToList()));
            var result = Norm.Euclidean(target.Subtract(currentPose));
            return result;
        }

        private static double[] DistanceGradient(List<MDHParameters> dht, double[] target, double[] angles)
        {
            List<double> gradient = new List<double>();
            for (int i = 0; i < dht.Count; i++)
            {
                gradient.Add(PartialGradient(dht, target, angles, i));
            }
            return gradient.ToArray();
        }

        private static double PartialGradient(List<MDHParameters> dht, double[] target, double[] angles, int index)
        {
            double dx = 0.01;
            double savedAngle = angles[index];

            double fx = Distance(dht, target, angles);

            angles[index] += dx;
            double dfx = Distance(dht, target, angles);

            double gradient = (dfx - fx) / dx;

            angles[index] = savedAngle;
            return gradient;
        }

        /*
        public static double[] InverseKinematics(List<MDHParameters> dht, double[] target)
        {
            int jointsCount = dht.Count;
            double[] angles = new double[jointsCount];
            int iterations = 15000;
            double accuracy = 0.9;
            double learningRate = 0.000000001;

            if (Distance(dht, target, angles) < accuracy)
                return angles;

            for (;true;)
            {
                for (int jointNum = 0; jointNum < dht.Count; jointNum++)
                {
                    double gradient = PartialGradient(dht, target, angles, jointNum);
                    angles[jointNum] -= learningRate * gradient;
                }

                if (Distance(dht, target, angles) <= accuracy)
                    break;
            }

            return angles;
        }

        private static double[] DistanceGradient(List<MDHParameters> dht, double[] target, double[] angles)
        {
            List<double> gradient = new List<double>();
            for (int i = 0; i < dht.Count; i++)
            {
                gradient.Add(PartialGradient(dht, target, angles, i));
            }
            return gradient.ToArray();
        }

        private static double PartialGradient(List<MDHParameters> dht, double[] target, double[] angles, int index)
        {
            double dx = 0.01;
            double savedAngle = angles[index];
            
            double fx = Distance(dht, target, angles);
            
            angles[index] += dx;
            double dfx = Distance(dht, target, angles);

            double gradient = (dfx - fx) / dx;
            
            angles[index] = savedAngle;
            return gradient;
        }
        */

        public static double[,] GetTransformMatrix(MDHParameters parameters)
        {
            double crx = Math.Cos(parameters.Alpha);
            double srx = Math.Sin(parameters.Alpha);
            double crz = Math.Cos(parameters.Theta);
            double srz = Math.Sin(parameters.Theta);
            double a = parameters.A;
            double d = parameters.D;

            double[,] result =
            {
                { crz, -srz, 0, a },
                { crx * srz, crx * crz, -srx, -d * srx },
                { srx * srz, crz * srx, crx, d * crx },
                { 0, 0, 0, 1 }
            };

            return result;
        }

        public static double[,] GetTransformMatrix(List<MDHParameters> parametersList)
        {
            double[,] matrix = Matrix.Diagonal<double>(4, 1);
            
            foreach (var parameters in parametersList)
                matrix = matrix.Dot(GetTransformMatrix(parameters));
            
            return matrix;
        }

        public static double[] GetPosition(MDHParameters parameters)
        {
            return GetPositionVector(GetTransformMatrix(parameters));
        }

        public static List<double[]> GetPositions(List<MDHParameters> parametersList)
        {
            List<double[]> positions = new List<double[]>();

            double[,] eye = Matrix.Diagonal<double>(4, 1);
            foreach (var parameters in parametersList)
            {
                eye = eye.Dot(GetTransformMatrix(parameters));
                positions.Add(GetPositionVector(eye));
            }

            return positions;
        }

        public static double[] GetPositionVector(double[,] transformMatrix)
        {
            return new double[] { transformMatrix[0, 3], transformMatrix[1, 3], transformMatrix[2, 3] };
        }

        public static double[,] GetRotationMatrix(double[,] transformMatrix)
        {
            return new double[,]
            {
                { transformMatrix[0, 0], transformMatrix[0, 1], transformMatrix[0, 2] },
                { transformMatrix[1, 0], transformMatrix[1, 1], transformMatrix[1, 2] },
                { transformMatrix[2, 0], transformMatrix[2, 1], transformMatrix[2, 2] }
            };
        }

    }
}
