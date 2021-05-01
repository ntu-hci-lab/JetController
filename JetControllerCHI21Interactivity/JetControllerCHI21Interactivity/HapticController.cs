using DualSense2Xbox;
using JetControllerCHI21Interactivity.JetController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace JetControllerCHI21Interactivity
{
    class HapticController
    {
        const byte ActuatedVelocity = 5;
        public struct Status
        {
            public byte Byte1, Byte2;
            public bool IsVibrationPacket;
        }; 
        public enum FeedbackApproach
        {
            Vibration,
            AdaptiveTrigger,
            JetController,
            NoFeedback
        }

        private enum GroundSurface
        {
            Studs,
            Grass,
            Stone,
            None
        }
        public static BufferBlock<Status> SignalFlow = new BufferBlock<Status>();
        public static bool IsHalfLifeNow = false;
        public static FeedbackApproach CurrentFeedbackApproach = FeedbackApproach.NoFeedback;
        Socket CurrentGameSocket;
        DualSense_Base dualSense;
        HardwareControl_1DoF JetController_Ctrl;
        Thread DrivingVibrationThread;
        volatile byte LeftMotorVibration, RightMotorVibration;
        volatile int VibrationOnDuration, VibrationOffDuration;
        volatile GroundSurface CurrentGroundSurface = GroundSurface.None;

        volatile int JetControllerOnDuration = 0,
            JetControllerOffDuration = 0;
        double JetControllerForceFeedback = 0;
        Thread JetControllerControlThread;
        void GameCommunicationSocketHandler(object _socket)
        {
            Socket socket = _socket as Socket;
            try
            {
                while (!IsHalfLifeNow && socket.Connected)
                {
                    byte[] ReceivedData = new byte[2];
                    socket.Receive(ReceivedData);
                    SignalFlow.Post(new Status { Byte1 = ReceivedData[0], Byte2 = ReceivedData[1], IsVibrationPacket = false });
                }
            }
            catch
            {
                socket.Dispose();
                return;
            }
        }
        void OnSocketNewConnectionAccepted(IAsyncResult result)
        {
            Socket ServerSocket = (Socket)result.AsyncState;
            CurrentGameSocket?.Close();
            CurrentGameSocket?.Dispose();
            SignalFlow.TryReceiveAll(out _);
            CurrentGameSocket = ServerSocket.EndAccept(result);
            new Thread(GameCommunicationSocketHandler).Start(CurrentGameSocket);
            ServerSocket.BeginAccept(new AsyncCallback(OnSocketNewConnectionAccepted), ServerSocket);
        }
        public HapticController(ref DualSense_Base dualSense, ref HardwareControl_1DoF JetController_Ctrl)
        {
            this.dualSense = dualSense;
            this.JetController_Ctrl = JetController_Ctrl;
            new Thread(SignalHandler).Start();
            DrivingVibrationThread = new Thread(DrivingVibrationControl);
            DrivingVibrationThread.Start();
            JetControllerControlThread = new Thread(JetControllerControl);
            JetControllerControlThread.Start();
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 62304);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ipe);
            socket.Listen(2);
            socket.BeginAccept(new AsyncCallback(OnSocketNewConnectionAccepted), socket);
        }
        public void ResetAllHapticTechniques()
        {
            dualSense.SetVibration(0, 0);
            dualSense.SetLeftAdaptiveTrigger(DualSense_Base.NormalTrigger);
            dualSense.SetRightAdaptiveTrigger(DualSense_Base.NormalTrigger);
            JetController_Ctrl?.SendValveOnOff(false);
        }
        void HalfLifeHandler(byte b0, byte b1)
        {
            if (b0 == b1 && b0 == 0)
                ResetAllHapticTechniques();
            if (b0 != 102 || b1 != 102)
                return; //Unknown Vibration Pattern

            switch (CurrentFeedbackApproach)
            {
                case FeedbackApproach.NoFeedback:
                    ResetAllHapticTechniques();
                    break;
                case FeedbackApproach.Vibration:
                    dualSense.SetVibration(127, 127);
                    break;
                case FeedbackApproach.AdaptiveTrigger:
                    dualSense.SetRightAdaptiveTrigger(DualSense_Base.VibrateTrigger_10Hz);
                    break;
                case FeedbackApproach.JetController:
                    JetControllerForceFeedback = 3;
                    JetControllerOnDuration = 50;
                    JetControllerOffDuration = 50;
                    JetControllerControlThread.Interrupt();
                    break;
            }
        }
        void JetControllerControl()
        {
            while (true)
            {
                try
                {
                    if (CurrentFeedbackApproach != FeedbackApproach.JetController)
                    {
                        Thread.Sleep(100);
                        continue;
                    }
                    if (!IsHalfLifeNow && CurrentGroundSurface == GroundSurface.None)
                    {
                        JetController_Ctrl?.SendValveOnOff(false);
                        Thread.Sleep(1);
                        continue;
                    }
                    JetController_Ctrl?.ApplyForce((uint)JetControllerOnDuration, JetControllerForceFeedback);
                    Thread.Sleep(JetControllerOnDuration);
                    JetController_Ctrl?.SendValveOnOff(false);
                    Thread.Sleep(JetControllerOffDuration);
                    if (CurrentGroundSurface == GroundSurface.Studs)
                        CurrentGroundSurface = GroundSurface.None;
                }
                catch (ThreadInterruptedException)
                {
                    JetController_Ctrl?.SendValveOnOff(false);
                    continue;
                }
            }
        }
        void DrivingVibrationControl()
        {
            while (true)
            {
                try
                {
                    if (IsHalfLifeNow || CurrentFeedbackApproach != FeedbackApproach.Vibration)
                    {
                        Thread.Sleep(100);
                        continue;
                    }
                    if (CurrentGroundSurface == GroundSurface.None)
                    {
                        dualSense.SetVibration(0, 0);
                        Thread.Sleep(1);
                        continue;
                    }
                    dualSense.SetVibration(LeftMotorVibration, RightMotorVibration);
                    dualSense.SetVibration(LeftMotorVibration, RightMotorVibration);
                    Thread.Sleep(VibrationOnDuration);
                    dualSense.SetVibration(0, 0);
                    dualSense.SetVibration(0, 0);
                    Thread.Sleep(VibrationOffDuration);
                    if (CurrentGroundSurface == GroundSurface.Studs)
                        CurrentGroundSurface = GroundSurface.None;
                }
                catch (ThreadInterruptedException)
                {
                    dualSense.SetVibration(0, 0);
                    continue;
                }
            }
        }
        void DrivingGameHandler(byte _GroundTexture, byte Speed)
        {
            GroundSurface TextureID = (GroundSurface)_GroundTexture;
            long AdaptiveTriggerPatternLeft = DualSense_Base.NormalTrigger;
            long AdaptiveTriggerPatternRight = DualSense_Base.NormalTrigger;
            JetControllerOnDuration = 0;
            JetControllerOffDuration = 0;
            JetControllerForceFeedback = 0;
            byte HeavyMotorValue = 0;
            byte LightMotorValue = 0;
            int VibrationOnDuration = 0;
            int VibrationOffDuration = 0;
            if (Speed <= ActuatedVelocity)
                goto ApplyForceFeedback;
            switch (TextureID)
            {
                case GroundSurface.Studs:
                    float SpeedPerSec = (Speed * 1000f / 3600);
                    byte HzPerStuds = (byte)Math.Ceiling(SpeedPerSec / 3);
                    AdaptiveTriggerPatternLeft = DualSense_Base.VibrateTrigger(HzPerStuds);
                    AdaptiveTriggerPatternRight = DualSense_Base.VibrateTrigger(HzPerStuds);
                    HeavyMotorValue = 255;
                    LightMotorValue = 34;
                    VibrationOnDuration = 20;
                    VibrationOffDuration = 65;
                    JetControllerForceFeedback = 3;
                    JetControllerOnDuration = 50;
                    JetControllerOffDuration = 1000 / HzPerStuds - JetControllerOnDuration;
                    break;
                case GroundSurface.Grass:
                    byte GrassFeedbackHz = 30;
                    AdaptiveTriggerPatternLeft = DualSense_Base.VibrateTrigger(GrassFeedbackHz);
                    AdaptiveTriggerPatternRight = DualSense_Base.VibrateTrigger(GrassFeedbackHz);
                    HeavyMotorValue = 32;
                    LightMotorValue = 127;
                    VibrationOnDuration = 10;
                    VibrationOffDuration = 25;
                    JetControllerForceFeedback = 1;
                    JetControllerOnDuration = 18;
                    JetControllerOffDuration = 15;
                    break;
                case GroundSurface.Stone:
                    //0-120 -> 10-20
                    byte StoneFeedbackHz = (byte)Math.Min(Math.Ceiling(Speed / 12f) + 10, 20);
                    AdaptiveTriggerPatternLeft = DualSense_Base.VibrateTrigger(StoneFeedbackHz);
                    AdaptiveTriggerPatternRight = DualSense_Base.VibrateTrigger(StoneFeedbackHz);
                    HeavyMotorValue = 255;
                    LightMotorValue = 35;
                    VibrationOnDuration = 25;
                    VibrationOffDuration = 1000 / StoneFeedbackHz - 20;
                    JetControllerForceFeedback = 1.6f;
                    JetControllerOnDuration = 25;
                    JetControllerOffDuration = 1000 / StoneFeedbackHz - JetControllerOnDuration;
                    break;
                case GroundSurface.None:
                    break;
            }
        ApplyForceFeedback:
            switch (CurrentFeedbackApproach)
            {
                case FeedbackApproach.AdaptiveTrigger:
                    dualSense.SetLeftAdaptiveTrigger(AdaptiveTriggerPatternLeft);
                    dualSense.SetRightAdaptiveTrigger(AdaptiveTriggerPatternRight);
                    break;
                case FeedbackApproach.Vibration:
                    this.VibrationOnDuration = VibrationOnDuration;
                    this.VibrationOffDuration = VibrationOffDuration;
                    this.LeftMotorVibration = HeavyMotorValue;
                    this.RightMotorVibration = LightMotorValue;
                    CurrentGroundSurface = TextureID;
                    if (CurrentGroundSurface > TextureID)
                        DrivingVibrationThread.Interrupt();
                    break;
                case FeedbackApproach.JetController:
                    JetControllerControlThread.Interrupt();
                    break;
                case FeedbackApproach.NoFeedback:
                    ResetAllHapticTechniques();
                    break;
            }

        }

        public void SignalHandler()
        {
            Status IncomingPacket;
            while (true)
            {
                if (SignalFlow.TryReceive(out IncomingPacket))
                {
                    if (IncomingPacket.IsVibrationPacket == true && IsHalfLifeNow == true)
                        HalfLifeHandler(IncomingPacket.Byte1, IncomingPacket.Byte2);
                    else if (IncomingPacket.IsVibrationPacket == false && IsHalfLifeNow == false)
                        DrivingGameHandler(IncomingPacket.Byte1, IncomingPacket.Byte2);
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
        }
    }
}
