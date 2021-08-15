using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Valve.VR;

namespace OpenVRInputTest
{
    public class CoordConverter
    {
        public static void ConvertVelocityFromWorldCoordToControllerCoord(HmdVector3_t vVelocity, HmdQuaternion_t qQuaternion, ref double[] LocalCoordVelocity_Normalized, ref double[] LocalCoordVelocity)
        {
            double[] _Velocity = new double[3] { vVelocity.v0, vVelocity.v1, vVelocity.v2 };
            double[] _Quaternion = new double[4] { qQuaternion.w, qQuaternion.x, qQuaternion.y, qQuaternion.z };
            double _VelocityScalar = Math.Sqrt(_Velocity[0] * _Velocity[0] + _Velocity[1] * _Velocity[1] + _Velocity[2] * _Velocity[2]);
            
            LocalCoordVelocity = new double[3];
            LocalCoordVelocity_Normalized = new double[3];

            Normalize(ref _Velocity);
            Rotate(_Velocity, _Quaternion[0], -_Quaternion[1], -_Quaternion[2], -_Quaternion[3], out LocalCoordVelocity_Normalized[0], out LocalCoordVelocity_Normalized[1], out LocalCoordVelocity_Normalized[2]);
            for (int i = 0; i < LocalCoordVelocity_Normalized.Length; ++i)
                LocalCoordVelocity[i] = LocalCoordVelocity_Normalized[i] * _VelocityScalar;
        }
        private static void Normalize(ref double[] vector)
        {
            double Total = 0;
            foreach (double d in vector)
                Total += d * d;
            Total = Math.Sqrt(Total);
            for (int i = 0; i < vector.Length; ++i)
                vector[i] /= Total;
        }
        //https://openhome.cc/Gossip/ComputerGraphics/images/quaternionsRotate-4.jpg
        private static void Rotate(double[] Position, double w, double x, double y, double z, out double px, out double py, out double pz)
        {
            double Unit = w * w + x * x + y * y + z * z;
            if (Math.Abs(Unit - 1) > 0.01)
                throw new Exception("");
            double _px = Position[0], _py = Position[1], _pz = Position[2];
            px = (1 - 2 * (y * y + z * z)) * _px
                + 2 * (x * y - w * z) * _py
                + 2 * (w * y + x * z) * _pz;

            py = -(2 * (x * y + w * z) * _px
                + (1 - 2 * (x * x + z * z)) * _py
                + 2 * (y * z - w * x) * _pz);

            pz = -(2 * (x * z - w * y) * _px
                + 2 * (y * z + w * x) * _py
                + (1 - 2 * (x * x + y * y)) * _pz);
        }
    }
    
}
