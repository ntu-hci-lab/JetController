using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

public class JetController
{
        /*
            Airflow Direction

                   Nozzle 0
                      *
                      *
                      *
                     * *
                    *   *
                   *     *
                  *       *
            Nozzle 2        Nozzle 1
        */

        /*
                Force Direction
                      | Up
            Nozzle 1        Nozzle 2
                  *       *
                   *     *
                    *   *
                     * *  -> Right
                      *
                      *
                      *
                      *
                  Nozzle 0
        */
    const int RegulatorCount = 4;
    const int ValveCount = 5;
    const int Max_PWM_Value = 181;
    /// Only Produce Force Larger than Force_Ignore_Threshold
    const double Force_Ignore_Threshold = 0.01;
    /// The maximum force this system can generate. (Unit: Newton)
    const double One_Direction_Max_Force = 5;   
    SerialPort serialPort;
    object SerialPort_Lock = new object();
    static readonly double[,] ForceVectors = new double[5, 3]
    {
                { Math.Cos(270 / 180f * Math.PI), Math.Sin(270 / 180f * Math.PI), 0 },
                { Math.Cos(150 / 180f * Math.PI), Math.Sin(150 / 180f * Math.PI), 0 },
                { Math.Cos(030 / 180f * Math.PI), Math.Sin(030 / 180f * Math.PI), 0 },
                { 0, 0, -1 },
                { 0, 0, 1 }
    };
    ~JetController()
    {
        serialPort.Close();
    }
    public JetController(SerialPort serialPort)
    {
        this.serialPort = serialPort;
    }
    public JetController(string COMPortName, int BaudRate)
    {
        serialPort = new SerialPort(COMPortName)
        {
            BaudRate = BaudRate,
            DataBits = 8,
            StopBits = StopBits.One,
            Parity = Parity.None
        };
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
    }
    /// Convert Force Magnitude to PWM Value
    private static int Force_To_PWM(double Force)
    {
        if (Force < Force_Ignore_Threshold)
            return 0;
        else if (double.IsNaN(Force) || double.IsInfinity(Force))
            Force = One_Direction_Max_Force;

        // Please write your Force-PWM mapping function here.
        throw new NotImplementedException("Please write your Force-PWM mapping function here!");
    }
    /// Convert PWM Value to Force Magnitude
    private static double PWM_To_Force(int PWM)
    {
        if (PWM > Max_PWM_Value)
            return One_Direction_Max_Force;  // Handle the force exceeds the maximum force of the system
        else if (PWM <= 0)
            return 0;   //Handle the unexcepted value of PWM

        // Please write your Force-PWM mapping function here.
        throw new NotImplementedException("Please write your Force-PWM mapping function here!");
    }
    /// Convert XYZ Force Vectors to 5-Nozzle Vectors
    private void MapForceToNozzle(double RightLeft, double FrontRear, double UpDown, out int[] PWM_5Nozzle, ref double[] RealForce)
    {
    	/*Map force to 5-nozzle domain*/
        double[] VectorLength = new double[5];
        // Calculate Force Direction on XY-Plane
        double Angle = (Math.Atan2(FrontRear, RightLeft) * 180f / Math.PI + 360) % 360;
        // If No Backward Force
        if (Angle >= 30 && Angle <= 150)
        {
            VectorLength[2] = (2 * FrontRear + RightLeft / ForceVectors[2, 0]) / 2f;
            VectorLength[1] = 2 * FrontRear - VectorLength[2];
        }
        else
        {
            VectorLength[1] = Math.Max(RightLeft / ForceVectors[1, 0], 0);
            VectorLength[2] = Math.Max(RightLeft / ForceVectors[2, 0], 0);
            VectorLength[0] = 0.5 * (VectorLength[1] + VectorLength[2]) - FrontRear;
        }
        // Map the Z-Axis Force
        VectorLength[3] = Math.Max(-UpDown, 0);
        VectorLength[4] = Math.Max(UpDown, 0);

        PWM_5Nozzle = new int[5];
        for (int i = 0; i < 5; ++i)
        {
            // Convert Force to PWM
            PWM_5Nozzle[i] = Force_To_PWM(VectorLength[i]);
            // Re-map the 5-Nozzle Force to XYZ Force
            if (RealForce != null && RealForce.Length >= 3)
                for (int j = 0; j < 3; ++j)
                    RealForce[i] += ForceVectors[i, j] * PWM_To_Force(PWM_5Nozzle[i]);	//Output the exact force produced by JetController
        }
    }
    public void SendPredictForce(double RightLeft, double FrontRear, double UpDown, ref double[] RealForce)
    {
        int[] PWM_Value;
        // Map to 5-Nozzle Domain
        MapForceToNozzle(RightLeft, FrontRear, UpDown, out PWM_Value, ref RealForce);
        for (int i = 0; i < 5; ++i)
        	// If the regulated pressure should be adjusted
            if (PWM_Value[i] > 0)
            {
            	// Is Z-Axis?
                if (i != 4)
                    SendForceValue(i, PWM_Value[i]);
                else
                    SendForceValue(3, PWM_Value[4]);    //Z-Axis Share the same regulator
            }
    }
    public void ApplyForce(double RightLeft, double FrontRear, double UpDown, ref double[] RealForce)
    {
        int[] PWM_Value;
        MapForceToNozzle(RightLeft, FrontRear, UpDown, out PWM_Value, ref RealForce);	//RealForce return the excal force magnitude JetController produced
        CloseAllValve();	//Close all valves first
        for (int i = 0; i < 5; ++i)
            // If the regulated pressure should be adjusted
            if (PWM_Value[i] > 0)
            {
            	// Is Z-Axis?
                if (i != 4)
                    SendForceValue(i, PWM_Value[i]);
                else
                    SendForceValue(3, PWM_Value[4]);    //Z-Axis Share the same regulator
                SendValveOnOff(i, true);
            }
    }

    private void SendForceValue(int ID, int PWM)
    {
        if (ID < 0 || ID >= RegulatorCount)
            return;
        PWM = Math.Min(PWM, Max_PWM_Value);
        PWM = Math.Max(PWM, 0);
        lock (SerialPort_Lock)
            serialPort?.Write(new byte[] { 04, (byte)ID, (byte)PWM, 00 }, 0, 4);
    }
    public void SendValveOnOff(int ID, bool OnOff)
    {
        if (ID < 0 || ID >= ValveCount)
            return;
        lock (SerialPort_Lock)
            serialPort?.Write(new byte[] { 05, 00, (byte)ID, (byte)(OnOff ? 1 : 0), 00 }, 0, 5);
    }
    public void CloseAllValve()
    {
        for (int i = 0; i < ValveCount; ++i)
        {
            SendValveOnOff(i, false); 
        }
    }
}
