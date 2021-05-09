using Device.Net;
using DualSense2Xbox;
using Hid.Net.Windows;
using JetControllerCHI21Interactivity.JetController;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Forms;
using Usb.Net.Windows;

namespace JetControllerCHI21Interactivity
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        HalfLife2_Manager HL2_Manager;
        DualSense_Base dualSense;
        HapticController hapticController;
        HardwareControl_1DoF JetControllerCtrl;
        void InitDualSense()
        {
            //Register the factory for creating Usb devices. This only needs to be done once.
            WindowsUsbDeviceFactory.Register(null, null);

            //Register the factory for creating Usb devices. This only needs to be done once.
            WindowsHidDeviceFactory.Register(null, null);

            //Check Available Joystick Count
            var SearchConnectedJoystickTask = DeviceManager.Current.GetConnectedDeviceDefinitionsAsync(new FilterDeviceDefinition { DeviceType = DeviceType.Hid, UsagePage = 0x01 });
            SearchConnectedJoystickTask.Wait();
            var DevicesList = (SearchConnectedJoystickTask.Result).Where(dev => (dev.Usage == 4 || dev.Usage == 5));

            if (DevicesList.Count() > 1)
            {
                StringBuilder stringBuilder = new StringBuilder("Threse are your joystick lists:\n\n", 16384);
                foreach (var device in DevicesList)
                    stringBuilder.Append(device.ProductName);
                MessageBox.Show("Please inseret DualSense only. Do not plug in other joysticks. Just plug in DualSense only!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show(stringBuilder.ToString(), "Detected Device List", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Process.GetCurrentProcess().Kill();
            }

            //Search DualSense Controller
            var DualSenseList = DevicesList.Where(dev => (dev.VendorId == 1356 && dev.ProductId == 3302)).ToArray();
            int DualSenseCount = DualSenseList.Length;
            if (DualSenseCount <= 0)
            {
                MessageBox.Show("Cannot find DualSense. Check the cable again!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Process.GetCurrentProcess().Kill();
            }
            var DualSenseDevice = DeviceManager.Current.GetDevice(DualSenseList[0]);

            if (DualSenseList[0].ReadBufferSize == 64)
                dualSense = new DualSense_USB(DualSenseDevice);
            else if (DualSenseList[0].ReadBufferSize == 78)
                dualSense = new DualSense_Bluetooth(DualSenseDevice);
            else
                throw new NotImplementedException("Unsupported Connection Type.");

            dualSense.SetLeftAdaptiveTrigger(DualSense_Base.NormalTrigger);
            dualSense.SetRightAdaptiveTrigger(DualSense_Base.NormalTrigger);
        }
        bool CheckLocalFileExists()
        {
            var FileNames = new string[] { "d1_canals_01.hl1", "d1_canals_01.hl2", "d1_canals_01.hl3", "half-life-000.sav", "half-life-000.tga" };
            bool Success = true;
            foreach (var FileName in FileNames)
            {
                bool IsFileExist = File.Exists(FileName);
                Success &= IsFileExist;
                if (!IsFileExist)
                    MessageBox.Show($"Cannot find file {FileName}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return Success;
        }
        private void RewriteSaveFile(HalfLife2_Manager halfLife2_Manager)
        {
            var Path = halfLife2_Manager.HalfLife2Path + @"\hl2\save\";
            var FileNames = Directory.GetFiles(Path);
            foreach (var FileName in FileNames)
                File.Delete(FileName);
            var LocalFileNames = new string[] { "d1_canals_01.hl1", "d1_canals_01.hl2", "d1_canals_01.hl3", "half-life-000.sav", "half-life-000.tga" };
            try
            {
                foreach (var FileName in LocalFileNames)
                    File.Copy(FileName, Path + FileName);
            }catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private void DeleteOtherSavedFile(HalfLife2_Manager halfLife2_Manager)
        {
            var Path = halfLife2_Manager.HalfLife2Path + @"\hl2\save\";
            var FileNames = Directory.GetFiles(Path);
            var ReservedFiles = new string[] { "d1_canals_01.hl1", "d1_canals_01.hl2", "d1_canals_01.hl3", "half-life-000.sav", "half-life-000.tga" };
            foreach (var FileName in FileNames)
            {
                bool IsContains = false;
                foreach (var ReservedFile in ReservedFiles)
                {
                    IsContains |= FileName.Contains(ReservedFile);
                }
                if (!IsContains)
                    File.Delete(FileName);
            }
        }
        [STAThread]
        private void MainForm_Load(object sender, EventArgs e)
        {
            if (!CheckLocalFileExists())
                throw new FileNotFoundException();
            
            //Set Adaptive Trigger as "Normal"
            Thread ThreadInitDualSense = new Thread(InitDualSense);
            ThreadInitDualSense.Start();
            ThreadInitDualSense.Join();
            using (SelectComPortForm sel_com_form = new SelectComPortForm())
            {
                sel_com_form.ShowDialog();
                if (sel_com_form.COM_Name == null)
                    radioButton_JetController.Enabled = false;
                else
                    JetControllerCtrl = new HardwareControl_1DoF(sel_com_form.COM_Name, 500000);
            }
            hapticController = new HapticController(ref dualSense, ref JetControllerCtrl);
            if (HalfLife2_Manager.GetSteamFolder() == null)
                MessageBox.Show("Cannot Find Steam!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void button_StartHalfLife_Click(object sender, EventArgs e)
        {
            try
            {
                HL2_Manager = new HalfLife2_Manager();
            }
            catch (FileNotFoundException)
            {
                var MsgBoxResult = MessageBox.Show("It seems that you don't have Steam / Half-Life 2 in your computer.\n" +
                    "The experience is based on Half-Life 2. Do you want to purchase it from Steam?\n" +
                    "(Press 'NO' to experience haptic feedback without Half-Life 2)", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (MsgBoxResult == DialogResult.No)
                {
                    groupBox_SelectHapticTechnique.Enabled = true;
                    groupBox_GameSelect.Enabled = false;
                    groupBox_GameSelect.Text = "Select Experience (Game). Close the application to re-select your experience.";
                    HapticController.IsHalfLifeNow = true;
                    new Thread(() =>
                    {
                        while (HapticController.IsHalfLifeNow)
                        {
                            if (dualSense.RightTrigger > 20)
                                HapticController.SignalFlow.Post(new HapticController.Status{ Byte1 = 127, Byte2 = 127, IsVibrationPacket = true });
                            else
                                HapticController.SignalFlow.Post(new HapticController.Status { Byte1 = 0, Byte2 = 0, IsVibrationPacket = true });

                            Thread.Sleep(100);
                        }
                    }).Start();
                    return;
                }
                else if (MsgBoxResult == DialogResult.Yes)
                    Process.Start("https://store.steampowered.com/app/220/HalfLife_2/");
                return;
            }
            MessageBox.Show("Since we only parse the firing signal from vibration channel, the latencies of Adaptive Trigger and JetController might be noticable.\nThis is the limitation of parsing haptic events from commercial games.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            groupBox_SelectHapticTechnique.Enabled = true;
            groupBox_GameSelect.Enabled = false;
            groupBox_GameSelect.Text = "Select Experience (Game). Close the application to re-select your experience.";
            HapticController.IsHalfLifeNow = true;
            RewriteSaveFile(HL2_Manager);
            if (!HL2_Manager.RunHalfLife2())
            {
                MessageBox.Show("Cannot launch Half-Life 2 properly!\nCheck your Steam configuration!\nPerhaps you are not login!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            new Thread(() =>
            {
                int Counter = 0;
                bool IsHalfLifeAlreadyStarted = false;
                while (true)
                {
                    if (++Counter < 60)
                        DeleteOtherSavedFile(HL2_Manager);
                    IsHalfLifeAlreadyStarted |= Process.GetProcessesByName("hl2").Length != 0;
                    Thread.Sleep(1000);
                    if (IsHalfLifeAlreadyStarted && Process.GetProcessesByName("hl2").Length == 0)  //Check is Half-Life 2 still running?
                        Application.Exit();
                }
            }).Start();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            HalfLife2_Manager.KillHalfLife2();
            foreach (var p in Process.GetProcessesByName("JetControllerCarRacing"))
                p.Kill();
            Process.GetCurrentProcess().Kill();
        }
        private void OnFeedbackApproachCheckedChanged(object sender, EventArgs e)
        {
            RadioButton btn = sender as RadioButton;
            if (btn.Checked == false)
                return;
            if (btn == radioButton_Vibration)
                HapticController.CurrentFeedbackApproach = HapticController.FeedbackApproach.Vibration;
            else if (btn == radioButton_AdaptiveTrigger)
                HapticController.CurrentFeedbackApproach = HapticController.FeedbackApproach.AdaptiveTrigger;
            else if (btn == radioButton_JetController && JetControllerCtrl != null)
                HapticController.CurrentFeedbackApproach = HapticController.FeedbackApproach.JetController;
            else if (btn == radioButton_Disable)
                HapticController.CurrentFeedbackApproach = HapticController.FeedbackApproach.NoFeedback;
            else
                throw new Exception("Unable to handle the problem");
            new Thread(hapticController.ResetAllHapticTechniques).Start();
        }

        private void button_StartDriving_Click(object sender, EventArgs e)
        {
            groupBox_SelectHapticTechnique.Enabled = true;
            groupBox_GameSelect.Enabled = false;
            groupBox_GameSelect.Text = "Select Experience (Game). Close the application to re-select your experience.";
            HapticController.IsHalfLifeNow = false;
            Process.Start("Driving\\JetControllerCarRacing.exe");
            new Thread(() => { 
                while (true)
                {
                    Thread.Sleep(1000);
                    if (Process.GetProcessesByName("JetControllerCarRacing").Length == 0)
                        Application.Exit();
                }
            }).Start();
        }
    }
}
