namespace JetControllerCHI21Interactivity
{
    partial class SelectComPortForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.comboBox_ComPortList = new System.Windows.Forms.ComboBox();
            this.label_SelectJetController = new System.Windows.Forms.Label();
            this.button_Confirm = new System.Windows.Forms.Button();
            this.button_Skip = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // comboBox_ComPortList
            // 
            this.comboBox_ComPortList.FormattingEnabled = true;
            this.comboBox_ComPortList.Location = new System.Drawing.Point(15, 54);
            this.comboBox_ComPortList.Name = "comboBox_ComPortList";
            this.comboBox_ComPortList.Size = new System.Drawing.Size(205, 20);
            this.comboBox_ComPortList.TabIndex = 0;
            // 
            // label_SelectJetController
            // 
            this.label_SelectJetController.AutoSize = true;
            this.label_SelectJetController.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_SelectJetController.Location = new System.Drawing.Point(14, 23);
            this.label_SelectJetController.Name = "label_SelectJetController";
            this.label_SelectJetController.Size = new System.Drawing.Size(208, 18);
            this.label_SelectJetController.TabIndex = 1;
            this.label_SelectJetController.Text = "Select COM Port of JetController";
            // 
            // button_Confirm
            // 
            this.button_Confirm.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Confirm.Location = new System.Drawing.Point(15, 89);
            this.button_Confirm.Name = "button_Confirm";
            this.button_Confirm.Size = new System.Drawing.Size(205, 39);
            this.button_Confirm.TabIndex = 2;
            this.button_Confirm.Text = "&Confrim";
            this.button_Confirm.UseVisualStyleBackColor = true;
            this.button_Confirm.Click += new System.EventHandler(this.button_Confirm_Click);
            // 
            // button_Skip
            // 
            this.button_Skip.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Skip.Location = new System.Drawing.Point(15, 146);
            this.button_Skip.Name = "button_Skip";
            this.button_Skip.Size = new System.Drawing.Size(205, 39);
            this.button_Skip.TabIndex = 3;
            this.button_Skip.Text = "&Skip";
            this.button_Skip.UseVisualStyleBackColor = true;
            this.button_Skip.Click += new System.EventHandler(this.button_Skip_Click);
            // 
            // SelectComPortForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(234, 211);
            this.Controls.Add(this.button_Skip);
            this.Controls.Add(this.button_Confirm);
            this.Controls.Add(this.label_SelectJetController);
            this.Controls.Add(this.comboBox_ComPortList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectComPortForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select COM Port From List";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SelectComPortForm_FormClosing);
            this.Load += new System.EventHandler(this.SelectComPortForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox_ComPortList;
        private System.Windows.Forms.Label label_SelectJetController;
        private System.Windows.Forms.Button button_Confirm;
        private System.Windows.Forms.Button button_Skip;
    }
}