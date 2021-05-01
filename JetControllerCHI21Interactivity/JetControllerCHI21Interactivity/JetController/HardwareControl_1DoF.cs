using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace JetControllerCHI21Interactivity.JetController
{

    public class HardwareControl_1DoF
    {
        const int Max_PWM_Value = 180;
        const double Force_Ignore_Threshold = 0.05;
        static double One_Direction_Max_Force
        {
            get
            {
                return PWM_To_Force(Max_PWM_Value);
            }
        }
        SerialPort serialPort;
        object SerialPort_Lock = new object();
        long ValveNextStateChangeTime = 0;
        bool ValveNextState = false;
        bool ValveNowState = false;
        Thread thread;
        static readonly int[,] ForceMap =
            {
                    { 181, 4076 },
                    { 178, 4024 },
                    { 175, 3968 },
                    { 172, 3920 },
                    { 169, 3902 },
                    { 166, 3888 },
                    { 163, 3848 },
                    { 160, 3796 },
                    { 157, 3748 },
                    { 154, 3696 },
                    { 151, 3648 },
                    { 148, 3594 },
                    { 145, 3536 },
                    { 142, 3484 },
                    { 139, 3428 },
                    { 136, 3372 },
                    { 133, 3320 },
                    { 130, 3260 },
                    { 127, 3206 },
                    { 124, 3150 },
                    { 121, 3092 },
                    { 118, 3038 },
                    { 115, 2976 },
                    { 112, 2910 },
                    { 109, 2850 },
                    { 106, 2786 },
                    { 103, 2726 },
                    { 100, 2660 },
                    { 97, 2600 },
                    { 94, 2536 },
                    { 91, 2468 },
                    { 88, 2400 },
                    { 85, 2330 },
                    { 82, 2264 },
                    { 79, 2198 },
                    { 76, 2130 },
                    { 73, 2056 },
                    { 70, 1984 },
                    { 67, 1908 },
                    { 64, 1840 },
                    { 61, 1764 },
                    { 58, 1678 },
                    { 55, 1608 },
                    { 52, 1530 },
                    { 49, 1456 },
                    { 46, 1380 },
                    { 43, 1304 },
                    { 40, 1222 },
                    { 37, 1140 },
                    { 34, 1062 },
                    { 31, 982 },
                    { 28, 904 },
                    { 25, 828 },
                    { 22, 734 },
                    { 19, 656 },
                    { 16, 574 },
                    { 13, 480 },
                    { 10, 402 },
                    { 7, 320 },
                    { 4, 176 },
                    { 0, 0 }
                };
        private void CreateThread()
        {
            thread = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        long NowTicks = DateTime.Now.Ticks;
                        if (ValveNextStateChangeTime != 0 && NowTicks >= ValveNextStateChangeTime)
                        {
                            SendValveOnOff(ValveNextState);
                            ValveNextStateChangeTime = 0;
                        }
                        Thread.Sleep(1);
                    }
                }
                catch
                {

                }
            })
            {
                IsBackground = true,
                Priority = ThreadPriority.AboveNormal
            };
            thread.Start();
        }
        ~HardwareControl_1DoF()
        {
            thread.Abort();
        }
        public HardwareControl_1DoF(SerialPort serialPort)
        {
            this.serialPort = serialPort;
            CreateThread();
        }
        public HardwareControl_1DoF(string COMPortName, int BaudRate)
        {
            serialPort = new SerialPort(COMPortName);
            serialPort.BaudRate = BaudRate;
            serialPort.DataBits = 8;
            serialPort.StopBits = StopBits.One;
            serialPort.Parity = Parity.None;
            while (true)
            {
                try
                {
                    serialPort.Open();
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Cannot Open {COMPortName}.\n{e.Message}\nPress Enter to Retry");
                    Console.ReadLine();
                }
            }
            CreateThread();
        }
        private static int Force_To_PWM(double Force)
        {
            if (Force < Force_Ignore_Threshold)
                return 0;
            else if (double.IsNaN(Force) || double.IsInfinity(Force))
                Force = One_Direction_Max_Force;
            int Force_mN = (int)(Force * 1000);
            for (int i = 1; i < ForceMap.GetLength(0); ++i)
            {
                if (ForceMap[i, 1] <= Force_mN)
                {
                    int Force_Diff_mN = ForceMap[i - 1, 1] - ForceMap[i, 1];
                    int PWM_Diff = ForceMap[i - 1, 0] - ForceMap[i, 0];
                    double TargetPWM = ForceMap[i, 0] +
                        Math.Round((Force_mN - ForceMap[i, 1]) * PWM_Diff / (double)Force_Diff_mN);
                    return (int)TargetPWM;
                }
            }
            throw new NotImplementedException();
        }
        private static double PWM_To_Force(int PWM)
        {
            if (PWM > Max_PWM_Value)
                return ForceMap[0, 1] / 1000f;  //Default: Max Force
            else if (PWM <= 0)
                return 0;
            for (int i = 1; i < ForceMap.GetLength(0); ++i)
            {
                if (ForceMap[i, 0] <= PWM)
                {
                    int PWM_Diff = ForceMap[i - 1, 0] - ForceMap[i, 0];
                    int Force_Diff_mN = ForceMap[i - 1, 1] - ForceMap[i, 1];
                    int TargetForce_mN = ForceMap[i, 1] + ((PWM - ForceMap[i, 0]) * Force_Diff_mN) / PWM_Diff;
                    return TargetForce_mN / 1000f;
                }
            }
            throw new Exception("Cannot Find Suitable Force in ForceMap Table");
        }
        public void SendPredictForce(double RealForce)
        {
            SendForceValue(Force_To_PWM(RealForce));
        }
        public void ApplyForce(uint Duration, double RealForce)
        {
            CloseAllValve();
            SendForceValue(Force_To_PWM(RealForce));
            OpenValve(Duration);
        }
        private void SendForceValue(int PWM)
        {
            PWM = Math.Min(PWM, Max_PWM_Value);
            PWM = Math.Max(PWM, 0);
            lock (SerialPort_Lock)
                serialPort?.Write(new byte[] { 04, (byte)00, (byte)PWM, 00 }, 0, 4);
        }
        public void SendValveOnOff(bool OnOff)
        {
            if (ValveNowState == OnOff)
                return;
            lock (SerialPort_Lock)
                serialPort?.Write(new byte[] { 05, 00, (byte)00, (byte)(OnOff ? 1 : 0), 00 }, 0, 5);
            if (OnOff == false && ValveNextState == false)
                ValveNextStateChangeTime = 0;
            ValveNowState = OnOff;
            ValveNextStateChangeTime = 0;
        }
        public void OpenValve(uint millisecondDuration)
        {
            SendValveOnOff(true);
            ValveNextState = false;
            ValveNextStateChangeTime = DateTime.Now.AddMilliseconds(millisecondDuration).Ticks;
        }
        public void CloseAllValve()
        {
            if (ValveNowState != false)
                SendValveOnOff(false);
        }
    }
}
