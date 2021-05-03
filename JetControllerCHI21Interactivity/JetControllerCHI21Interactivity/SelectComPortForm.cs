using JetControllerCHI21Interactivity.JetController;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JetControllerCHI21Interactivity
{
    public partial class SelectComPortForm : Form
    {
        bool IsCloseFormDirectly = false;
        public string COM_Name;
        public SelectComPortForm()
        {
            InitializeComponent();
        }
        private void SelectComPortForm_Load(object sender, EventArgs e)
        {
            var AllPossibleList = SerialPort.GetPortNames();
            if (AllPossibleList != null && AllPossibleList.Length != 0)
            {
                comboBox_ComPortList.Items.AddRange(AllPossibleList);
                return;
            }
            var Result = MessageBox.Show("Cannot find any COM Port.\nDo you want to experience our demonstration without JetController?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (Result == DialogResult.No)
            {
                IsCloseFormDirectly = true;
                Application.Exit();
            }
            else
            {
                IsCloseFormDirectly = true;
                COM_Name = null;
                this.Close();
            }
        }
        private void SelectComPortForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsCloseFormDirectly)
                return;
            var Result = MessageBox.Show("You have not chose a COM Port.\nDo you want to experience our demonstration without JetController?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (Result == DialogResult.No)
                e.Cancel = true;
            else
                return;
        }

        private void button_Confirm_Click(object sender, EventArgs e)
        {
            if (comboBox_ComPortList.Text.Length == 0)
                return;
            SerialPort serialPort = new SerialPort(comboBox_ComPortList.Text, 500000);
            try
            {
                serialPort.Open();
                serialPort.Close();
                serialPort.Dispose();
                COM_Name = comboBox_ComPortList.Text;
                IsCloseFormDirectly = true;
                this.Close();
            }
            catch
            {
                MessageBox.Show($"Cannot Open COM Port {comboBox_ComPortList.Text}!\nSelect other COM Ports or Skip!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void button_Skip_Click(object sender, EventArgs e)
        {
            IsCloseFormDirectly = true;
            COM_Name = null;
            this.Close();
        }
    }
}
